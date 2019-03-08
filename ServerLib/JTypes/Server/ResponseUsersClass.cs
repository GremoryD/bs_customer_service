using Newtonsoft.Json;
using System.Collections.Generic;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Список пользователей
    /// </summary>
    public class ResponseUsersClass : ResponseBaseClass
    {
        /// <summary>
        /// Массив пользователей
        /// </summary>
        [JsonProperty(PropertyName = "users", Required = Required.Always)]
        public List<ResponseUserClass> Items { get; set; }

        public ResponseUsersClass() : base(Enums.Commands.users)
        {
            Items = new List<ResponseUserClass>();
        }
    }
}
