using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Объект аутентификации пользователя в системе
    /// </summary>
    public class LoginClass : BaseResponseClass
    {
        /// <summary>
        /// Ключ доступа к данным
        /// </summary>
        [JsonProperty(PropertyName = "token", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; } = null;

        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "user_id", Required = Required.Always)]
        public long UserId { get; set; } = 0;

        /// <summary>
        /// Признак активности пользователя
        /// </summary>
        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        public LoginClass() : base(Enums.Commands.login) { }
    }
}
