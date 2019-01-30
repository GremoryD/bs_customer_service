namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка пользователей
    /// </summary>
    public class RequestUsersClass : RequestBaseRequestClass
    {
        public RequestUsersClass() : base(Enums.Commands.users) { }
    }
}
