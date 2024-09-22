using PoC.VirtualManager.FineTuning.Models;
using PoC.VirtualManager.Utils.MongoDb;
using PoC.VirtualManager.Utils.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Infrastructure
{
    /// <summary>
    /// Data sourcing pattern.
    /// </summary>
    public class DataSetControRepository : MongoDbRepository<DataSetControl>
    {
        //TODO keep track of scripts ran
        public DataSetControRepository(string mongoDbConnectionString) 
            : base(new MongoDbSettings
            {
                ConnectionString = mongoDbConnectionString,
                DatabaseName = "VirtualManager", //TODO as input
                CollectionName = "FineTuningDataSetControl"
            })
        {
        }
    }
}
