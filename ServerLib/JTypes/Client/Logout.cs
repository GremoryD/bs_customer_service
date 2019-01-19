namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос выхода пользователя из системы
    /// </summary>
    public class Logout : BaseRequestClass
    {
        public Logout() : base(Enums.Commands.logout) { }
    }
}
