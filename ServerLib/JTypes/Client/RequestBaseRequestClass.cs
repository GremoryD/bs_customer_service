using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerLib.JTypes.Enums;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Базовый класс запросов поступающих от клиента
    /// </summary>
    public class RequestBaseRequestClass
    {
        /// <summary>
        /// Команда запроса
        /// </summary>
        [JsonProperty("command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Commands Command { get; set; } = Commands.none;

        /// <summary>
        /// Ключ аутентификации
        /// </summary>
        [JsonProperty("token", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; } = null;

        public RequestBaseRequestClass() { }

        public RequestBaseRequestClass(Commands ACommand)
        {
            Command = ACommand;
        }
    }
}
