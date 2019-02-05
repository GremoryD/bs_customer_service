using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CLProject;

namespace ServerLib.JTypes.Server
{
    public class ResponseRoleObjectClass : ResponseBaseItemClass
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
        /// Доступная операция над объектом системы
        /// </summary>
        [JsonProperty(PropertyName = "object_operation", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.ObjectOperations ObjectOperation { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + RoleID.ToString() + ObjectID.ToString() + ObjectOperation.ToString());
    }
}
