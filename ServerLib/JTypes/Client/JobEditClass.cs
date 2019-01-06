using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос на изменение должности
    /// </summary>
    public class JobEditClass : BaseRequestClass
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

        public JobEditClass()
        {
            Command = Enums.Commands.job_edit;
        }
    }
}
