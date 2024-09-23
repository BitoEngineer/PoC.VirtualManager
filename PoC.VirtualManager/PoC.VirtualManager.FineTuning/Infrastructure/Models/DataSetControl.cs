using PoC.VirtualManager.Utils.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Infrastructure.Models
{
    public class DataSetControl : MongoDbEntity
    {
        public string Version { get; set; }
        public string FileName { get; set; }
        public string OpenAiFileId { get; set; }
        public string OpenAiJobId { get; set; }
        public string Status { get; set; }
    }
}
