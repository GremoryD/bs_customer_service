using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CLProject;

namespace ServerLib.JTypes.Server
{
    public class RoleObjectClass : BaseItemClass
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        [JsonProperty(PropertyName = "role_id", Required = Required.Always)]
        public long RoleID { get; set; }

        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        [JsonProperty(PropertyName = "object_id", Required = Required.Always)]
        public long ObjectID { get; set; }

        /// <summary>
        /// Доступная операция над объектом
        /// </summary>
        [JsonProperty(PropertyName = "operation", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.Operations Operation { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + RoleID.ToString() + ObjectID.ToString() + Operation.ToString());
    }
}
