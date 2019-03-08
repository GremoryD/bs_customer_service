using Newtonsoft.Json;
using CLProject;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Роль пользователя
    /// </summary>
    public class ResponseUserRoleClass : ResponseBaseItemClass
    {
         /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "user_id", Required = Required.Always)]
        public long UserID { get; set; }

        /// <summary>
        /// Идентификатор роли
        /// </summary>
        [JsonProperty(PropertyName = "role_id", Required = Required.Always)]
        public long RoleID { get; set; }

        /// <summary>
        /// Наименование роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "role_name", Required = Required.Always)]
        public string RoleName { get; set; } = null;

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + UserID.ToString() + RoleID.ToString());
    }
}
