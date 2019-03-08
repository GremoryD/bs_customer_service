namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка пользователей
    /// </summary>
    public class RequestUsersClass : RequestBaseClass
    {
        public RequestUsersClass() : base(Enums.Commands.users) { }
    }
}
