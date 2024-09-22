using MongoDB.Driver;
using PoC.VirtualManager.Utils.MongoDb.Models;
using System.Threading;

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

        public virtual async Task<T> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, id);

            return await (await GetOrCreateCollectionAsync(cancellationToken))
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> GetByIdAsync(string id, IClientSessionHandle session, CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, id);

            return await (await GetOrCreateCollectionAsync(cancellationToken))
                .Find(session, filter)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> GetByFilterAsync(
            FilterDefinition<T> filter, 
            CancellationToken cancellationToken)
        {
            return await (await GetOrCreateCollectionAsync(cancellationToken))
                .Find(filter)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T> InsertAsync(T item, 
            CancellationToken cancellationToken)
        {
            item.CreatedAt = DateTime.Now;
            item.UpdatedAt = DateTime.Now;
            await (await GetOrCreateCollectionAsync(cancellationToken)).InsertOneAsync(item, new InsertOneOptions
            {
            }, cancellationToken);

            return item;
        }

        public virtual async Task<T> UpdateAsync(T item,
            CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, item.Id);

            item.UpdatedAt = DateTime.UtcNow;
            var result = (await GetOrCreateCollectionAsync(cancellationToken)).ReplaceOneAsync(
                filter,
                item,
                new ReplaceOptions { IsUpsert = false },
                cancellationToken
            );

            return item;
        }

        public virtual async Task<T> UpdateAsync(T item,
            IClientSessionHandle session,
            CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, item.Id);

            item.UpdatedAt = DateTime.UtcNow;
            var result = (await GetOrCreateCollectionAsync(cancellationToken)).ReplaceOneAsync(
                session,
                filter,
                item,
                new ReplaceOptions { IsUpsert = false },
                cancellationToken
            );

            return item;
        }

        public virtual async Task<T> UpsertAsync(T item,
            CancellationToken cancellationToken)
        {
            var filter = Builders<T>.Filter.Eq(t => t.Id, item.Id);

            item.UpdatedAt = DateTime.UtcNow;
            var result = (await GetOrCreateCollectionAsync(cancellationToken)).ReplaceOneAsync(
                filter,
                item,
                new ReplaceOptions { IsUpsert = true },
                cancellationToken
            );

            return item;
        }

        protected async Task<IMongoCollection<T>> GetOrCreateCollectionAsync(CancellationToken cancellationToken)
        {
            if (_collection != null)
            {
                return _collection;
            }

            var collection = Database.Value.GetCollection<T>(Settings.CollectionName);

            if (collection == null)
            {
                await Database.Value.CreateCollectionAsync(Settings.CollectionName, cancellationToken: cancellationToken);
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
