namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос выхода пользователя из системы
    /// </summary>
    public class RequestLogoutClass : RequestBaseRequestClass
    {
        public RequestLogoutClass() : base(Enums.Commands.logout) { }
    }
}
