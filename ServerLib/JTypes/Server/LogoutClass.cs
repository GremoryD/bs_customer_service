namespace ServerLib.JTypes.Server
{
    public class LogoutClass : BaseResponseClass
    {
        public LogoutClass()
        {
            Command = Enums.Commands.logout;
        }
    }
}
