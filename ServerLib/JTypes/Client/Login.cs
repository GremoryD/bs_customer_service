using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib.JTypes.Client
{
    public class Login : AbsAll
    {
        [JsonProperty("login")]
        [JsonRequired]
        public string UserName { get; set; }

        [JsonProperty("password")]
        [JsonRequired]
        public string Password { get; set; }

        public Login()
        {
            // RequestType = Enums.RequestType.Login;
        }
    }
}
