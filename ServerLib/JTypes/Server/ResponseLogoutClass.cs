namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Объект выхода пользователя из системы
    /// </summary>
    public class ResponseLogoutClass : ResponseBaseClass
    {
        public ResponseLogoutClass() : base(Enums.Commands.logout) { }
    }
}
