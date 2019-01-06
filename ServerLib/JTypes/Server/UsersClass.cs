using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    public class UsersClass : BaseResponseClass
    {
        public List<JTypes.Server.UserClass> Users;

        public UsersClass()
        {
            Command = Enums.Commands.users;
            Users = new List<UserClass>();
        }
    }
}
