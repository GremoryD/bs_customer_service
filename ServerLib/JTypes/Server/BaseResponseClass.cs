using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ServerLib.JTypes.Enums;

namespace ServerLib.JTypes.Server
{
    public abstract class BaseResponseClass
    {
        [JsonProperty("type", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.Commands Type { get; set; }

        [JsonProperty("state", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public ResponseState State { get; set; }

        [JsonProperty("description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;
    }
}
