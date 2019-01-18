using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CLProject;

namespace ServerLib.JTypes.Server
{
    class UsersRoleClass
    {
        /// <summary>
        /// Идентификатор роли
        /// </summary>
        [JsonProperty(PropertyName = "role_id", Required = Required.Always)]
        public long RoleID { get; set; } = 0;

        /// <summary>
        /// Наименование роли
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; } = null;

        /// <summary>
        /// Наименование роли
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        /// <summary>
        /// Команда, которую необходимо выполнить на стороне клиента с данным объектом
        /// </summary>
        [JsonProperty(PropertyName = "command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.ListCommands Command { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash
        {
            get
            {
                return Utils.SHA256Base64(RoleID.ToString() + Name + Description);
            }
        }
    }
}
