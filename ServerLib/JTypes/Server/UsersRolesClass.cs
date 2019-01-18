using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список ролей пользователей
    /// </summary>
    public class UsersRolesClass : BaseResponseClass
    {
        /// <summary>
        /// Массив ролей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "roles", Required = Required.Always)]
        public List<UsersRoleClass> Roles;

        public UsersRolesClass() : base(Enums.Commands.users_roles)
        {
            Roles = new List<UsersRoleClass>();
        }
    }
}
