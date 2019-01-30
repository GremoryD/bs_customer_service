namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Структура удаления роли пользователя
    /// </summary>
    public class ResponseUserRoleDeleteClass : ResponseBaseClass
    {
        public ResponseUserRoleDeleteClass() : base(Enums.Commands.users_roles_delete) { }
    }
}
