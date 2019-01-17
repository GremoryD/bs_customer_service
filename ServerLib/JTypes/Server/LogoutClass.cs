namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Объект выхода пользователя из системы
    /// </summary>
    public class LogoutClass : BaseResponseClass
    {
        public LogoutClass() : base(Enums.Commands.logout) { }
    }
}
