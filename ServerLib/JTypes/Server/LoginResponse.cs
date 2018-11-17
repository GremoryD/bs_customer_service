using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.JTypes.Server
{
    public class LoginResponse : AbsResponseAll
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("patronymic")]
        public string Patronymic { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("user_function")]
        public string Function { get; set; }

        [JsonProperty("last_login_date")]
        public DateTime LastLogin { get; set; }
    }
}
