using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура добавления роли пользователей
    /// </summary>
    public class ResponseRoleAddClass : ResponseBaseClass
    {
        /// <summary>
        /// Идетификатор роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; } = 0;

        /// <summary>
        /// Наименование роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; } = null;

        /// <summary>
        /// Описание роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        public ResponseRoleAddClass() : base(Enums.Commands.roles_add) { }
    }
}
