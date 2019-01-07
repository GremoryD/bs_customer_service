using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerLib.JTypes.Enums;

namespace ServerLib.JTypes.Server
{
    public class BaseResponseClass
    {
        /// <summary>
        /// Команда объекта выдачи
        /// </summary>
        [JsonProperty("command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.Commands Command { get; set; }

        /// <summary>
        /// Статус обработки запроса к базе данных
        /// </summary>
        [JsonProperty("state", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public ResponseState State { get; set; }

        /// <summary>
        /// Описание ошибки обработки запроса к базе данных
        /// </summary>
        [JsonProperty("description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;
    }
}
