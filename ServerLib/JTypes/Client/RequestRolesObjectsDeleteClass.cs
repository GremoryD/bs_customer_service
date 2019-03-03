using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Client
{
    public class RequestRolesObjectsDeleteClass : RequestBaseClass
    {
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
        /// Удаляемая операция над объектом системы
        /// </summary>
        [JsonProperty(PropertyName = "object_operation", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.ObjectOperations ObjectOperation { get; set; }

        public RequestRolesObjectsDeleteClass() : base(Enums.Commands.roles_objects_delete) { }
    }
}
