using Newtonsoft.Json;

namespace ServerLib.JTypes.Client
{
    public class Login : BaseRequestClass
    {
        [JsonProperty("login", Required = Required.Always)]
        public string UserName { get; set; } = null;

        [JsonProperty("password", Required = Required.Always)]
        public string Password { get; set; } = null;

        public Login()
        {
            Command = Enums.Commands.login;
        }
    }
}
