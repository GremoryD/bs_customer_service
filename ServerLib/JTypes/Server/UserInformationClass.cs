using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    public class UserInformationClass : BaseResponseClass
    {
        [JsonProperty(PropertyName = "token", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; } = null;

        [JsonProperty(PropertyName = "first_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; } = null;

        [JsonProperty(PropertyName = "last_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; } = null;

        [JsonProperty(PropertyName = "midle_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string MidleName { get; set; } = null;

        [JsonProperty(PropertyName = "user_id", Required = Required.Always)]
        public long UserId { get; set; } = 0;

        [JsonProperty(PropertyName = "job", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Job { get; set; } = null;

        [JsonProperty(PropertyName = "last_login_date", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LastLoginDate { get; set; } = null;

        [JsonProperty(PropertyName = "active", Required = Required.Always)]
        public int Active { get; set; } = 0;

        public UserInformationClass()
        {
            Command = Enums.Commands.login;
        }
    }
}
