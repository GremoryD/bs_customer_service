using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    public class RequestRolesObjectsDeleteClass : RequestBaseRequestClass
    {
        /// <summary>
        /// Идентификатор разрешения роли пользователей к операции над объектом системы
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long RolesObjectsPermissionID { get; set; }

        public RequestRolesObjectsDeleteClass() : base(Enums.Commands.roles_objects_delete) { }
    }
}
