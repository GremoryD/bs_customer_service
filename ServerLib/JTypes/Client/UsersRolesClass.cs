namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка ролей пользователей
    /// </summary>
    public class UsersRolesClass : BaseRequestClass
    {
        public UsersRolesClass() : base(Enums.Commands.users_roles) { }
    }
}