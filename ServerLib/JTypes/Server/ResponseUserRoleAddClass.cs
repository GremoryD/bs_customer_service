using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура добавление роли пользователю
    /// </summary>
    public class ResponseUserRoleAddClass : ResponseBaseClass
    {
        /// <summary>
        /// Идетификатор роли пользователя
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; }

        /// <summary>
        /// Идетификатор пользователя
        /// </summary>
        [JsonProperty(PropertyName = "user_id", Required = Required.Always)]
        public long UserID { get; set; }

        /// <summary>
        /// Идетификатор роли
        /// </summary>
        [JsonProperty(PropertyName = "role_id", Required = Required.Always)]
        public long RoleID { get; set; }

        public ResponseUserRoleAddClass() : base(Enums.Commands.users_roles_add) { }
    }
}
