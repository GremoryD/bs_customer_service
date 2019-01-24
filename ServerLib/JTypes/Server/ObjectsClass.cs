using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список объектов системы
    /// </summary>
    public class ObjectsClass : BaseResponseClass
    {
        /// <summary>
        /// Массив должностей пользователей
        /// </summary>
        [JsonProperty(PropertyName = "objects", Required = Required.Always)]
        public List<ObjectClass> Items;

        public ObjectsClass() : base(Enums.Commands.objects)
        {
            Items = new List<ObjectClass>();
        }
    }
}
