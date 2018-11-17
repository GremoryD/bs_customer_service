using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using ServerLib.JTypes.Enums;

namespace ServerLib.JTypes.Server
{
    public abstract class AbsResponseAll
    {
        [JsonProperty("response_state")]
        [JsonRequired]
        public ResponseState State { get; set; }
    }
}
