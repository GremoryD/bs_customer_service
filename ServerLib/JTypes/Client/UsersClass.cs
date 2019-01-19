namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка пользователей
    /// </summary>
    public class UsersClass : BaseRequestClass
    {
        public UsersClass() : base(Enums.Commands.users) { }
    }
}
