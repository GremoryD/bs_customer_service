namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка ролей назначенных пользователям
    /// </summary>
    public class RequestUsersRolesClass : RequestBaseRequestClass
    {
        public RequestUsersRolesClass() : base(Enums.Commands.users_roles) { }
    }
}
