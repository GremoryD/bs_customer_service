using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос на добавление роли пользователей
    /// </summary>
    public class RequestRoleAddClass : RequestBaseClass
    {
        /// <summary>
        /// Наименование роли
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; } = null;

        /// <summary>
        /// Описание роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        public RequestRoleAddClass() : base(Enums.Commands.roles_add) { }
    }
}
