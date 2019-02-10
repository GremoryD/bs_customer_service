using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    public class ResponseUserPasswordChangeClass : ResponseBaseClass
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; }

        public ResponseUserPasswordChangeClass() : base(Enums.Commands.user_password_change) { }
    }
}
