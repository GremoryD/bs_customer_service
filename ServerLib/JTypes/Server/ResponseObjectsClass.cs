using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список объектов системы
    /// </summary>
    public class ResponseObjectsClass : ResponseBaseClass
    {
        /// <summary>
        /// Массив должностей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "objects", Required = Required.Always)]
        public List<ResponseObjectClass> Items;

        public ResponseObjectsClass() : base(Enums.Commands.objects)
        {
            Items = new List<ResponseObjectClass>();
        }
    }
}
