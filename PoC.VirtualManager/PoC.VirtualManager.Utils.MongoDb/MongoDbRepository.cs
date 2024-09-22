using MongoDB.Driver;
using PoC.VirtualManager.Utils.MongoDb.Models;

namespace PoC.VirtualManager.Utils.MongoDb
{
    public class MongoDbRepository<T> where T : MongoDbEntity
    {
        protected readonly MongoDbSettings Settings;
        protected readonly MongoClient Client;
        protected readonly Lazy<IMongoDatabase> Database;

        private IMongoCollection<T> _collection;

        public MongoDbRepository(MongoDbSettings settings)
            : this(MongoClientSettings.FromConnectionString(settings.ConnectionString))
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
            ArgumentNullException.ThrowIfNull(settings.CollectionName, nameof(settings.CollectionName));
            ArgumentNullException.ThrowIfNull(settings.DatabaseName, nameof(settings.DatabaseName));
            ArgumentNullException.ThrowIfNull(settings.ConnectionString, nameof(settings.ConnectionString));
        }

        public MongoDbRepository(MongoClientSettings settings)
            : this(new MongoClient(ApplyServerApi(settings)))
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        }

        public MongoDbRepository(MongoClient client)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            Client = client;
            Database = new Lazy<IMongoDatabase>(() => Client.GetDatabase(Settings.DatabaseName));
        }

        public async Task<T> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, id);

            return await (await GetOrCreateCollectionAsync()).Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> GetByFilterAsync(FilterDefinition<T> filter)
        {
            return await (await GetOrCreateCollectionAsync()).Find(filter).FirstOrDefaultAsync();
        }

        public async Task<T> InsertAsync(T item, CancellationToken cancellationToken)
        {
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
            (await GetOrCreateCollectionAsync()).InsertOneAsync(item, new InsertOneOptions
            {
            }, cancellationToken);

            return item;
        }

        public async Task<T> UpdateAsync(T item)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, item.Id);

            item.UpdatedAt = DateTime.UtcNow;
            var result = (await GetOrCreateCollectionAsync()).ReplaceOneAsync(
                filter,
                item,
                new ReplaceOptions { IsUpsert = false }
            );

            return item;
        }

        private async Task<IMongoCollection<T>> GetOrCreateCollectionAsync()
        {
            if(_collection != null)
            {
                return _collection;
            }

            var collection = Database.Value.GetCollection<T>(Settings.CollectionName);

            if (collection == null)
            {
                await Database.Value.CreateCollectionAsync(Settings.CollectionName);
            }

            _collection = collection ?? Database.Value.GetCollection<T>(Settings.CollectionName);
            return _collection;
        }

        private static MongoClientSettings ApplyServerApi(MongoClientSettings settings)
        {
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            return settings;
        }
    }
}
