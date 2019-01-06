using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerLib.JTypes.Enums;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Абстрактный класс запросов поступающих от клиента
    /// </summary>
    public abstract class BaseRequestClass
    {
        /// <summary>
        /// Команда запроса
        /// </summary>
        [JsonProperty("command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Commands Command { get; set; }

        /// <summary>
        /// Ключ аутентификации
        /// </summary>
        [JsonProperty("token", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; } = null;
    }
}
