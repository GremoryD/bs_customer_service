using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос на добавление должности пользователей
    /// </summary>
    public class JobAddClass : BaseRequestClass
    {
        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "job_name", Required = Required.Always)]
        public string JobName { get; set; } = null;

        public JobAddClass()
        {
            Command = Enums.Commands.job_add;
        }
    }
}
