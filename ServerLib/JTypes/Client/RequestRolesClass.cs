namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка всех ролей пользователей
    /// </summary>
    public class RequestRolesClass : RequestBaseRequestClass
    {
        public RequestRolesClass() : base(Enums.Commands.roles) { }
    }
}