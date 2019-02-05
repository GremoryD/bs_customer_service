using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список назначенных ролей пользователей
    /// </summary>
    public class ResponseUsersRolesClass : ResponseBaseClass
    {
        /// <summary>
        /// Массив назначенных ролей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "users_roles", Required = Required.Always)]
        public List<ResponseUserRoleClass> Items { get; set; }

        public ResponseUsersRolesClass() : base(Enums.Commands.users_roles)
        {
            Items = new List<ResponseUserRoleClass>();
        }
    }
}
