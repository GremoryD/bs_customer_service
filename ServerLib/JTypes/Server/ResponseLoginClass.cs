using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Объект аутентификации пользователя в системе
    /// </summary>
    public class ResponseLoginClass : ResponseBaseClass
    {
        /// <summary>
        /// Ключ доступа к данным
        /// </summary>
        [JsonProperty(PropertyName = "token", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; } = null;

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; } = 0;

        /// <summary>
        /// Признак активности пользователя
        /// </summary>
        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        public ResponseLoginClass() : base(Enums.Commands.login) { }
    }
}
