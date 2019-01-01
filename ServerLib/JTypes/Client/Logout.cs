namespace ServerLib.JTypes.Client
{
    public class Logout : BaseRequestClass
    {
        public Logout()
        {
            Command = Enums.Commands.logout;
        }
    }
}
