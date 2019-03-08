using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcsapp.Models
{
    public class AssetsRoleModel
    { 
        public long ID { set; get; }

        public string Name { get; set; }


        public string Description { get; set; } 
        public bool OperationRead { get; set; } 
        public bool OperationAdd { get; set; } 
        public bool OperationEdit { get; set; } 
        public bool OperationDelete { get; set; }


        public bool OperationReadAsset { get; set; } 
        public bool OperationAddAsset { get; set; }
        public bool OperationEditAsset { get; set; }
        public bool OperationDeleteAsset { get; set; } 


    }
}
