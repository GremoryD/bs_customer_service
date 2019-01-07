using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcsapp.Handlers
{
    public class SessionClass
    {
        public ServerLib.JTypes.Server.LoginClass Login = new ServerLib.JTypes.Server.LoginClass();
        public ServerLib.JTypes.Server.UserInformationClass UserInformation = new ServerLib.JTypes.Server.UserInformationClass();

        public SessionClass()
        {

        }

        public void LoginProcessing(ServerLib.JTypes.Server.LoginClass ALogin)
        {
            Login.Active = ALogin.Active;
            Login.Token = ALogin.Token;
            Login.UserId = ALogin.UserId;
        }

        public void LoginErrorProcessing(ServerLib.JTypes.Server.ExceptionClass AException)
        {
            Login.UserId = 0;
        }

        public void UserInformationProcessing(ServerLib.JTypes.Server.UserInformationClass AUserInformation)
        {
            if(AUserInformation.Active == ServerLib.JTypes.Enums.UserActive.blocked)
            {
                Login.UserId = 0;
            }
        }
    }
}
