using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerLib.JTypes.Enums;

namespace ServerLib.JTypes.Server
{
    public abstract class BaseResponseClass
    {
        /// <summary>
        /// Тип объекта в выдаче
        /// </summary>
        [JsonProperty("type", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.Commands Type { get; set; }

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
