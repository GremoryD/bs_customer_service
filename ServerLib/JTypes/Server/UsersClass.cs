using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список пользователей
    /// </summary>
    public class UsersClass : BaseResponseClass
    {
        /// <summary>
        /// Массив пользователей
        /// </summary>
        [JsonProperty(PropertyName = "users", Required = Required.Always)]
        public List<UserClass> Users;

        public UsersClass() : base(Enums.Commands.users)
        {
            Users = new List<UserClass>();
        }
    }
}
