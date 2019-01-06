using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос редактирования пользователя
    /// </summary>
    public class UserEditClass : BaseRequestClass
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [JsonProperty(PropertyName = "password", Required = Required.Always)]
        public string Password { get; set; } = null;

        /// <summary>
        /// Имя
        /// </summary>
        [JsonProperty(PropertyName = "first_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; } = null;

        /// <summary>
        /// Фамилия
        /// </summary>
        [JsonProperty(PropertyName = "last_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; } = null;

        /// <summary>
        /// Отчество
        /// </summary>
        [JsonProperty(PropertyName = "midle_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string MidleName { get; set; } = null;

        /// <summary>
        /// Идентификатор должности
        /// </summary>
        [JsonProperty(PropertyName = "job_id", Required = Required.Always)]
        public long JobId { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        public UserEditClass()
        {
            Command = Enums.Commands.user_edit;
        }
    }
}
