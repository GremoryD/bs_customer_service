using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    public class RequestUserPasswordChangeClass : RequestBaseClass
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; }

        /// <summary>
        /// Пароль пользователя
        /// </summary>
        [JsonProperty(PropertyName = "password", Required = Required.Always)]
        public string Password { get; set; } = null;

        public RequestUserPasswordChangeClass() : base(Enums.Commands.user_password_change) { }
    }
}
