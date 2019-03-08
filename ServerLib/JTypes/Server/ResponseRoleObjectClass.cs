using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CLProject;

namespace ServerLib.JTypes.Server
{
    public class ResponseRoleObjectClass : ResponseBaseItemClass
    {
        /// <summary>
        /// Идентификатор роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "role_id", Required = Required.Always)]
        public long RoleID { get; set; }

        /// <summary>
        /// Идентификатор объекта системы
        /// </summary>
        [JsonProperty(PropertyName = "object_id", Required = Required.Always)]
        public long ObjectID { get; set; }

        /// <summary>
        /// Доступность чтения объекта
        /// </summary>
        [JsonProperty(PropertyName = "read", Required = Required.Always)]
        public bool OperationRead { get; set; }

        /// <summary>
        /// Доступность создания объекта
        /// </summary>
        [JsonProperty(PropertyName = "add", Required = Required.Always)]
        public bool OperationAdd { get; set; }

        /// <summary>
        /// Доступность изменения объекта
        /// </summary>
        [JsonProperty(PropertyName = "edit", Required = Required.Always)]
        public bool OperationEdit { get; set; }

        /// <summary>
        /// Доступность удаления объекта
        /// </summary>
        [JsonProperty(PropertyName = "delete", Required = Required.Always)]
        public bool OperationDelete { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + RoleID.ToString() + ObjectID.ToString() + OperationRead.ToString() + OperationAdd.ToString() + OperationEdit.ToString() + OperationDelete.ToString());
    }
}
