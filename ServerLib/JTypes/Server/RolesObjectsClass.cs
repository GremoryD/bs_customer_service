using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Права доступа ролей пользователей к операциям над объектами системы
    /// </summary>
    public class RolesObjectsClass : BaseResponseClass
    {
        /// <summary>
        /// Массив прав доступа ролей пользователей к операциям над объектами системы
        /// </summary>
        [JsonProperty(PropertyName = "roles_objects", Required = Required.Always)]
        public List<RoleObjectClass> Items;

        public RolesObjectsClass() : base(Enums.Commands.roles_objects)
        {
            Items = new List<RoleObjectClass>();
        }

        public static implicit operator RolesObjectsClass(RolesClass v)
        {
            throw new NotImplementedException();
        }
    }
}
