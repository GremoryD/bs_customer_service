﻿using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос на изменение должности
    /// </summary>
    public class RequestJobEditClass : RequestBaseClass
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

        public RequestJobEditClass() : base(Enums.Commands.job_edit) { }
    }
}
