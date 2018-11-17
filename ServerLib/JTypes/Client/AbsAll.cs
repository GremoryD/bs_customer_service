using Newtonsoft.Json;
using ServerLib.JTypes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.JTypes.Client
{
    public abstract class AbsAll
    {
        /*
         * Переход на схемные проверки
         * 
        [JsonProperty("request_type")]
        [JsonRequired]
        public RequestType RequestType { get; set; }
        */

        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = null;
    }
}
