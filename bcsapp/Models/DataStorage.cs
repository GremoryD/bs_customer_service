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
        public List<UserClass> UserList { set; get; } = new List<UserClass>();
        public List<JobClass> JobList { set; get; } = new List<JobClass>();
        public List<RoleClass> RoleList { set; get; } = new List<RoleClass>();
        public List<RoleClass> UsersRoles{ set; get; } = new List<RoleClass>();

        public void ClearData()
        {
            Login = null;
            UserInformation = null;
        }


        public List<AccessRolesData> accessRolesData = new List<AccessRolesData>()
        {
            new AccessRolesData(){ Name = "Пользователи" , Create = true, Delete= false, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Пользователи" , Create = true, Delete= true, Read = true, Edit = true}
        };
    }
}
