using Newtonsoft.Json;
using CLProject;

namespace ServerLib.JTypes.Server
{
    public class ObjectClass : BaseItemClass
    {
        /// <summary>
        /// Наименование объекта
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Описание объекта
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

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
        public string Hash => Utils.SHA256Base64(ID.ToString() + Name + Description + OperationRead.ToString() + OperationAdd.ToString() + OperationEdit.ToString() + OperationDelete.ToString());
    }
}
