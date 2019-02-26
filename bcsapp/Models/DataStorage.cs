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

        public ServerLib.JTypes.Client.RequestLoginClass LoginIN { set; get; }
        public ResponseLoginClass Login { set; get; }
        public ResponseUserInformationClass UserInformation { set; get; }
        public List<ResponseUserClass> UserList { set; get; } = new List<ResponseUserClass>();
        public List<ResponseJobClass> JobList { set; get; } = new List<ResponseJobClass>();
        public List<ResponseRoleClass> RoleList { set; get; } = new List<ResponseRoleClass>();
        public List<ResponseUserRoleClass> UsersRolesList{ set; get; } = new List<ResponseUserRoleClass>();
        public List<ResponseObjectClass> accessRolesObjectsData = new List<ResponseObjectClass>();
        public List<ResponseRoleObjectClass> accessRoleToObjectsData = new List<ResponseRoleObjectClass>();

        public List<AssetsRoleModel> accessListData = new List<AssetsRoleModel>();

        public void ClearData()
        {
            Login = null;
            UserInformation = null;
        }  
    }
}
