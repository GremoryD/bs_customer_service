namespace ServerLib.JTypes.Server
{
    public class LogoutClass : BaseResponseClass
    {
        public LogoutClass()
        {
            Type = Enums.Commands.logout;
        }
    }
}
