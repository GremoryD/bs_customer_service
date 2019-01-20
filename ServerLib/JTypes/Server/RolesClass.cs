using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список всех ролей пользователей
    /// </summary>
    public class RolesClass : BaseResponseClass
    {
        /// <summary>
        /// Массив всех ролей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "roles", Required = Required.Always)]
        public List<RoleClass> Items { get; set; }

        public RolesClass() : base(Enums.Commands.roles)
        {
            Items = new List<RoleClass>();
        }
    }
}
