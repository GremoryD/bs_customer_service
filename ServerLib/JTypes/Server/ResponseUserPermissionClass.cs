using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Объект системы с правом доступа к операции над ним
    /// </summary>
    public class ResponseUserPermissionClass
    {
        /// <summary>
        /// Наименование объекта системы
        /// </summary>
        [JsonProperty(PropertyName = "object_name", Required = Required.Always)]
        public string ObjectName { get; set; }

        /// <summary>
        /// Операция доступная для выполнения над объектом системы
        /// </summary>
        [JsonProperty("operation", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.ObjectOperations Operation { get; set; }
    }
}
