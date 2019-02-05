using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура удаления разрешения у роли пользователей на операцию над объектом системы
    /// </summary>
    public class ResponseRolesObjectsDeleteClass : ResponseBaseClass
    {
        /// <summary>
        /// Идетификатор разрешения роли пользователей на операцию над объектом системы
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; } = 0;

        public ResponseRolesObjectsDeleteClass() : base(Enums.Commands.roles_objects_delete) { }
    }
}
