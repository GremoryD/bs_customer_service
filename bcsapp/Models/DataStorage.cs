using ServerLib.JTypes.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcsapp.Models
{
    public class DataStorage
    {
        private static DataStorage s_instance;
        public static DataStorage Instance { get { if (s_instance == null) { s_instance = new DataStorage(); } return s_instance; } }

        public LoginClass Login { set; get; }
        public UserInformationClass UserInformation { set; get; }



    }
}
