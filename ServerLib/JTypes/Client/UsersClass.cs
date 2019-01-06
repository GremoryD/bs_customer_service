namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка пользователей
    /// </summary>
    class UsersClass : BaseRequestClass
    {
        public UsersClass()
        {
            Command = Enums.Commands.users;
        }
    }
}
