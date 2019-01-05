using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    public class UserInformationClass : BaseResponseClass
    {
        [JsonProperty(PropertyName = "first_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; } = null;

        [JsonProperty(PropertyName = "last_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; } = null;

        [JsonProperty(PropertyName = "midle_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string MidleName { get; set; } = null;

        [JsonProperty(PropertyName = "job", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Job { get; set; } = null;

        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        public UserInformationClass()
        {
            Command = Enums.Commands.user_information;
        }
    }
}
