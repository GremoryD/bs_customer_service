using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    public class UserRoleDeleteClass : BaseRequestClass
    {
        /// <summary>
        /// Идентификатор роли пользователя
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long UserRoleID { get; set; }

        public UserRoleDeleteClass() : base(Enums.Commands.users_roles_delete) { }
    }
}
