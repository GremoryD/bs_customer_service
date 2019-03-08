using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура изменения должности пользователя
    /// </summary>
    public class ResponseJobEditClass : ResponseBaseClass
    {
        /// <summary>
        /// Идетификатор должности
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long ID { get; set; } = 0;

        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; } = null;

        public ResponseJobEditClass() : base(Enums.Commands.job_edit) { }
    }
}
