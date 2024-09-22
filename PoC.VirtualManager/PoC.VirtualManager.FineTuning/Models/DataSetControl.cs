using PoC.VirtualManager.Utils.MongoDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoC.VirtualManager.FineTuning.Models
{
    public class DataSetControl : MongoDbEntity
    {
        public string Version { get; set; }
        public string ScriptName { get; set; }
    }
}
