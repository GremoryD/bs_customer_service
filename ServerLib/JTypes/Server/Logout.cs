using Newtonsoft.Json;

namespace ServerLib.JTypes.Server
{
    public class Logout : BaseResponseClass
    {
        public Logout()
        {
            Command = Enums.Commands.logout;
        }
    }
}
