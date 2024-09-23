using MongoDB.Driver;
using PoC.VirtualManager.FineTuning.Infrastructure.Models;
using PoC.VirtualManager.Utils.MongoDb;
using PoC.VirtualManager.Utils.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Infrastructure
{
    public interface IDataSetControlRepository 
    {
        Task<DataSetControl> GetLatestByNameAsync(string id, CancellationToken cancellationToken);
        Task<DataSetControl> InsertAsync(DataSetControl dataSetControl, CancellationToken cancellationToken);
    }


    /// <summary>
    /// Data sourcing pattern.
    /// </summary>
    public class DataSetControlRepository : MongoDbRepository<DataSetControl>, IDataSetControlRepository
    {
        //TODO keep track of scripts ran
        public DataSetControlRepository(string mongoDbConnectionString,
            string databaseName) 
            : base(new MongoDbSettings
            {
                ConnectionString = mongoDbConnectionString,
                DatabaseName = databaseName,
                CollectionName = "FineTuningDataSetControl"
            })
        {
        }

        public async Task<DataSetControl> GetLatestByNameAsync(string teamName, CancellationToken cancellationToken)
        {
            var filter = Builders<DataSetControl>.Filter.Eq(t => t.FileName, teamName);

            return await (await GetOrCreateCollectionAsync(cancellationToken))
                .Find(filter)
                .SortByDescending(t => MongoDB.Bson.BsonRegularExpression.Create(@"(\d+)\.(\d+)\.(\d+)")) // Sort using a regex pattern for versions
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
