using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Security.Cryptography;
using System;

namespace ServerLib.JTypes.Server
{
    public class UserClass
    {
        [JsonProperty(PropertyName = "id", Required = Required.Always)]
        public long Id { get; set; } = 0;

        [JsonProperty(PropertyName = "login", Required = Required.Always)]
        public string Login { get; set; } = null;

        [JsonProperty(PropertyName = "first_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; } = null;

        [JsonProperty(PropertyName = "last_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; } = null;

        [JsonProperty(PropertyName = "midle_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string MidleName { get; set; } = null;

        [JsonProperty(PropertyName = "job_id", Required = Required.Always)]
        public long JobId { get; set; }

        [JsonProperty(PropertyName = "job", Required = Required.Always)]
        public string Job { get; set; } = null;

        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }

        [JsonProperty(PropertyName = "command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public JTypes.Enums.ListCommands Command { get; set; }

        [JsonIgnore]
        public string Hash
        {
            get
            {
                SHA256 Sha = SHA256.Create();
                return Convert.ToBase64String(Sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Id.ToString() + Login + FirstName + LastName + MidleName + Job + Active.ToString()))); 
            }
        }
    }
}
