﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    public class LoginClass : BaseResponseClass
    {
        [JsonProperty(PropertyName = "token", Required = Required.AllowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Token { get; set; } = null;

        [JsonProperty(PropertyName = "user_id", Required = Required.Always)]
        public long UserId { get; set; } = 0;

        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        public LoginClass()
        {
            Command = Enums.Commands.login;
        }
    }
}
