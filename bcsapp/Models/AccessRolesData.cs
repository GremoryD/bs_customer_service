using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcsapp.Models
{
    public class AccessRolesData
    {
        public string Name { set; get; }
        public bool Read { set; get; }
        public bool Create { set; get; }
        public bool Edit { set; get; }
        public bool Delete { set; get; }
        public string Description { set; get; }
    }
}
