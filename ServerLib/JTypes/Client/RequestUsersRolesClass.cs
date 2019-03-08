namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка ролей назначенных пользователям
    /// </summary>
    public class RequestUsersRolesClass : RequestBaseClass
    {
        public RequestUsersRolesClass() : base(Enums.Commands.users_roles) { }
    }
}
