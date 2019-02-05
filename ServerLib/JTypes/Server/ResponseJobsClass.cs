using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список должностей пользователей
    /// </summary>
    public class ResponseJobsClass : ResponseBaseClass
    {
        /// <summary>
        /// Массив должностей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "jobs", Required = Required.Always)]
        public List<ResponseJobClass> Jobs;

        public ResponseJobsClass() : base(Enums.Commands.jobs)
        {
            Jobs = new List<ResponseJobClass>();
        }
    }
}
