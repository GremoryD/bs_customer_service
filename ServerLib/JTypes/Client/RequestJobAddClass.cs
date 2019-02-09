using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос на добавление должности пользователей
    /// </summary>
    public class RequestJobAddClass : RequestBaseClass
    {
        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; } = null;

        public RequestJobAddClass() : base(Enums.Commands.job_add) { }
    }
}
