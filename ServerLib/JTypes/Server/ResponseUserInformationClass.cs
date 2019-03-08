using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Информация о пользователе
    /// </summary>
    public class ResponseUserInformationClass : ResponseBaseClass
    {
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
        /// Должность
        /// </summary>
        [JsonProperty(PropertyName = "job_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string JobName { get; set; } = null;

        /// <summary>
        /// Признак активности
        /// </summary>
        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        /// <summary>
        /// Список разрешений пользователю на операции над объектами системы
        /// </summary>
        [JsonProperty(PropertyName = "permissions", Required = Required.Always)]
        public List<ResponseUserPermissionClass> Permissions { get; set; }

        public ResponseUserInformationClass() : base(Enums.Commands.user_information)
        {
            Permissions = new List<ResponseUserPermissionClass>();
        }
    }
}
