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
        public Commands Command { get; set; }

        /// <summary>
        /// Статус обработки запроса к базе данных
        /// </summary>
        [JsonProperty("state", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public ResponseState State { get; set; }

        /// <summary>
        /// Описание ошибки обработки запроса к базе данных
        /// </summary>
        [JsonProperty("error_text", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorText { get; set; } = null;

        public BaseResponseClass(Commands ACommand)
        {
            Command = ACommand;
        }

        public BaseResponseClass() { }
    }
}
