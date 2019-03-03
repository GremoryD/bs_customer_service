using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура добавленного разрешения роли пользователей на операцию над объектом системы
    /// </summary>
    public class ResponseRolesObjectsAddClass : ResponseBaseClass
    {
        /// <summary>
        /// Идетификатор разрешения роли пользователей на операцию над объектом системы
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; } = 0;

        /// <summary>
        /// Идентификатор роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "role_id", Required = Required.Always)]
        public long RoleID { get; set; }

        /// <summary>
        /// Идентификатор объекта системы
        /// </summary>
        [JsonProperty(PropertyName = "object_id", Required = Required.Always)]
        public long ObjectID { get; set; }

        /// <summary>
        /// Доступная операция над объектом системы
        /// </summary>
        [JsonProperty(PropertyName = "object_operation", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.ObjectOperations ObjectOperation { get; set; }

        public ResponseRolesObjectsAddClass() : base(Enums.Commands.roles_objects_add) { }
    }
}
