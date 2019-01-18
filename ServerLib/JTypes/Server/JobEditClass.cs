using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура изменения должности пользователя
    /// </summary>
    public class JobEditClass : BaseResponseClass
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

        public JobEditClass() : base(Enums.Commands.job_edit) { }
    }
}
