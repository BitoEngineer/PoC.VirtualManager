using MongoDB.Driver;
using PoC.VirtualManager.Interactions.Models;
using PoC.VirtualManager.Utils.MongoDb;
using PoC.VirtualManager.Utils.MongoDb.Models;

namespace PoC.VirtualManager.Interactions.Infrastructure
{
    public interface IInteractionsMetadataRepository
    {
        Task<InteractionMetadata> InsertAsync(
            InteractionMetadata interactionMetadata,
            CancellationToken cancellationToken);
    }

    public class InteractionsMetadataRepository : MongoDbRepository<InteractionMetadata>, IInteractionsMetadataRepository
    {
        public InteractionsMetadataRepository(string databaseName, string mongoDbConnectionString)
            : base(new MongoDbSettings
            {
                ConnectionString = mongoDbConnectionString,
                DatabaseName = databaseName,
                CollectionName = "InteractionsMetadata"
            })
        {
            ArgumentNullException.ThrowIfNull(mongoDbConnectionString, nameof(mongoDbConnectionString));
        }
    }
}
