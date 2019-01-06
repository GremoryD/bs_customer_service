using Newtonsoft.Json;
using CLProject;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Должность в составе списка
    /// </summary>
    public class JobClass
    {

        /// <summary>
        /// Идетификатор должности
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; } = 0;

        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "job_name", Required = Required.Always)]
        public string JobName { get; set; } = null;

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash
        {
            get
            {
                return Utils.SHA256Base64(Id.ToString() + JobName);
            }
        }
    }
}
