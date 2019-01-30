using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список всех ролей пользователей
    /// </summary>
    public class ResponseRolesClass : ResponseBaseClass
    {
        /// <summary>
        /// Массив всех ролей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "roles", Required = Required.Always)]
        public List<ResponseRoleClass> Items { get; set; }

        public ResponseRolesClass() : base(Enums.Commands.roles)
        {
            Items = new List<ResponseRoleClass>();
        }
    }
}
