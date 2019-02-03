using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Права доступа ролей пользователей к операциям над объектами системы
    /// </summary>
    public class ResponseRolesObjectsClass : ResponseBaseClass
    {
        /// <summary>
        /// Массив прав доступа ролей пользователей к операциям над объектами системы
        /// </summary>
        [JsonProperty(PropertyName = "roles_objects", Required = Required.Always)]
        public List<ResponseRoleObjectClass> Items;

        public ResponseRolesObjectsClass() : base(Enums.Commands.roles_objects)
        {
            Items = new List<ResponseRoleObjectClass>();
        }
    }
}
