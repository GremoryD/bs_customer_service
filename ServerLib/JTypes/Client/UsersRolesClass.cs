namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка ролей назначенных пользователям
    /// </summary>
    public class UsersRolesClass : BaseRequestClass
    {
        public UsersRolesClass() : base(Enums.Commands.users_roles) { }
    }
}
