using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    public class UserRoleAddClass : BaseRequestClass
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

        public UserRoleAddClass() : base(Enums.Commands.users_roles_add) { }
    }
}
