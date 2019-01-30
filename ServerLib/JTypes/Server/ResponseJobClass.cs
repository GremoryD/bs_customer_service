using Newtonsoft.Json;
using CLProject;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Должность в составе списка
    /// </summary>
    public class ResponseJobClass : ResponseBaseItemClass
    {
        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + Name);
    }
}
