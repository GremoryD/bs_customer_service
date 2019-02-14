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

        public void ClearData()
        {
            Login = null;
            UserInformation = null;
        } 



        public List<AccessRolesData> accessRolesData = new List<AccessRolesData>()
        {
            new AccessRolesData(){ Name = "Пользователи" , Create = true, Delete= false, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Должности пользователей" , Create = true, Delete= true, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Роли пользователей" , Create = true, Delete= true, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Сессии пользователей" , Create = false, Delete= true, Read = true, Edit = false},
            new AccessRolesData(){ Name = "Объекты" , Create = false, Delete= false, Read = true, Edit = false},
            new AccessRolesData(){ Name = "Клиенты" , Create = true, Delete= true, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Параметры клиентов" , Create = true, Delete= true, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Фото клиентов" , Create = false, Delete= false, Read = true, Edit = false},
            new AccessRolesData(){ Name = "HTTP-запросы" , Create = false, Delete= false, Read = true, Edit = false},
            new AccessRolesData(){ Name = "Части HTTP-запросов" ,Create = false, Delete= false, Read = true, Edit = false},
            new AccessRolesData(){ Name = "Мерчанты" , Create = true, Delete= true, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Маршруты HTTP-запросов" , Create = true, Delete= false, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Обработчики HTTP-запросов" , Create = true, Delete= true, Read = true, Edit = true},
            new AccessRolesData(){ Name = "Журнал HTTP-запросов" , Create = false, Delete= false, Read = true, Edit = false},
            new AccessRolesData(){ Name = "Журнал ошибок выполнения SQL-запросов" ,Create = false, Delete= false, Read = true, Edit = false},
            new AccessRolesData(){ Name = "Транзакции платежей клиентов" , Create = false, Delete= false, Read = true, Edit = false},
            new AccessRolesData(){ Name = "Журнал операций пользователей" ,Create = false, Delete= false, Read = true, Edit = false},
        };
    }
}
