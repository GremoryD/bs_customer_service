using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список должностей пользователей
    /// </summary>
    public class JobsClass : BaseResponseClass
    {
        /// <summary>
        /// Массив должностей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "jobs", Required = Required.Always)]
        public List<JobClass> Jobs;

        public JobsClass() : base(Enums.Commands.jobs)
        {
            Jobs = new List<JobClass>();
        }
    }
}
