using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    public class RequestUserRoleDeleteClass : RequestBaseRequestClass
    {
        /// <summary>
        /// Идентификатор роли пользователя
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long UserRoleID { get; set; }

        public RequestUserRoleDeleteClass() : base(Enums.Commands.users_roles_delete) { }
    }
}
