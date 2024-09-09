using MongoDB.Driver;
using PoC.VirtualManager.Interactions.Models;

namespace PoC.VirtualManager.Interactions.Infrastructure
{
    public interface IInteractionsRepository
    {
        Task<InteractionMetadata> InsertAsync(
            InteractionMetadata interactionMetadata,
            CancellationToken cancellationToken);
    }

    public class InteractionsRepository : IInteractionsRepository
    {
        private const string DatabaseName = "VirtualManager";
        private const string InteractionsCollectionName = "Interactions";

        private readonly MongoClient _client;
        private readonly Lazy<IMongoDatabase> _database;
        private readonly Lazy<Task<IMongoCollection<InteractionMetadata>>> _interactionsCollectionInitializer;

        private IMongoCollection<InteractionMetadata> _interactionsCollection => _interactionsCollectionInitializer.Value.Result;

        public InteractionsRepository(string mongoDbConnectionString)
            : this(MongoClientSettings.FromConnectionString(mongoDbConnectionString))
        {
            ArgumentNullException.ThrowIfNull(mongoDbConnectionString, nameof(mongoDbConnectionString));
        }

        public InteractionsRepository(MongoClientSettings settings)
            : this(new MongoClient(ApplyServerApi(settings)))
        {
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));
        }

        public InteractionsRepository(MongoClient client)
        {
            ArgumentNullException.ThrowIfNull(client, nameof(client));
            _client = client;
            _database = new Lazy<IMongoDatabase>(() => _client.GetDatabase(DatabaseName));
            _interactionsCollectionInitializer = new Lazy<Task<IMongoCollection<InteractionMetadata>>>(GetOrCreateCollectionAsync<InteractionMetadata>);
        }

        public async Task<InteractionMetadata> InsertAsync(
            InteractionMetadata interactionMetadata, 
            CancellationToken cancellationToken)
        {
            interactionMetadata.CreatedAt = DateTime.UtcNow;
            await _interactionsCollection.InsertOneAsync(interactionMetadata, new InsertOneOptions
            {
            }, cancellationToken);

            return interactionMetadata;
        }

        private async Task<IMongoCollection<T>> GetOrCreateCollectionAsync<T>()
        {
            var teamsCollection = _database.Value.GetCollection<T>(InteractionsCollectionName);

            if (teamsCollection == null)
            {
                await _database.Value.CreateCollectionAsync(InteractionsCollectionName);
            }

            return teamsCollection ?? _database.Value.GetCollection<T>(InteractionsCollectionName);
        }

        private static MongoClientSettings ApplyServerApi(MongoClientSettings settings)
        {
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            return settings;
        }
    }
}
