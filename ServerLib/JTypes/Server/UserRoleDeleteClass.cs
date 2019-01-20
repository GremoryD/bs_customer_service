namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура удаления роли пользователя
    /// </summary>
    public class UserRoleDeleteClass : BaseResponseClass
    {
        public UserRoleDeleteClass() : base(Enums.Commands.users_roles_delete) { }
    }
}
