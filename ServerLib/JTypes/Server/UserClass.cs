using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CLProject;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Пользователь в составе списка
    /// </summary>
    public class UserClass
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "user_id", Required = Required.Always)]
        public long UserID { get; set; } = 0;

        /// <summary>
        /// Логин
        /// </summary>
        [JsonProperty(PropertyName = "login", Required = Required.Always)]
        public string Login { get; set; } = null;

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
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "job", Required = Required.Always)]
        public string Job { get; set; } = null;

        /// <summary>
        /// Признак активности
        /// </summary>
        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        /// <summary>
        /// Команда, которую необходимо выполнить на стороне клиента с данным объектом
        /// </summary>
        [JsonProperty(PropertyName = "command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public JTypes.Enums.ListCommands Command { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash
        {
            get
            {
                return Utils.SHA256Base64(UserID.ToString() + Login + FirstName + LastName + MidleName + Job + Active.ToString());
            }
        }
    }
}
