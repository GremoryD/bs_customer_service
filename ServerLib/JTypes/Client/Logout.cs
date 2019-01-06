namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос выхода пользователя из системы
    /// </summary>
    public class Logout : BaseRequestClass
    {
        public Logout()
        {
            Command = Enums.Commands.logout;
        }
    }
}
