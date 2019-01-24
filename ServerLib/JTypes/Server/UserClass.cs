using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CLProject;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Пользователь в составе списка
    /// </summary>
    public class UserClass : BaseItemClass
    {
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
        public long JobID { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "job_name", Required = Required.Always)]
        public string JobName { get; set; } = null;

        /// <summary>
        /// Признак активности
        /// </summary>
        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + Login + FirstName + LastName + MidleName + JobName + Active.ToString());
    }
}
