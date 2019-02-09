namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос выхода пользователя из системы
    /// </summary>
    public class RequestLogoutClass : RequestBaseClass
    {
        public RequestLogoutClass() : base(Enums.Commands.logout) { }
    }
}
