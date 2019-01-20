using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список назначенных ролей пользователей
    /// </summary>
    public class UsersRolesClass : BaseResponseClass
    {
        /// <summary>
        /// Массив назначенных ролей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "users_roles", Required = Required.Always)]
        public List<UserRoleClass> Items { get; set; }

        public UsersRolesClass() : base(Enums.Commands.users_roles)
        {
            Items = new List<UserRoleClass>();
        }
    }
}
