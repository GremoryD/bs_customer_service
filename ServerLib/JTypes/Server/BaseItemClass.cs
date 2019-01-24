using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Базовый класс элемента обновляемого списка
    /// </summary>
    public class BaseItemClass
    {
        /// <summary>
        /// Идентификатор объекта
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; }

        /// <summary>
        /// Команда, которую необходимо выполнить на стороне клиента с данным объектом
        /// </summary>
        [JsonProperty(PropertyName = "command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.ListCommands Command { get; set; }
    }
}
