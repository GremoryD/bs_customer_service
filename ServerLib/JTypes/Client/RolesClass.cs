namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка всех ролей пользователей
    /// </summary>
    public class RolesClass : BaseRequestClass
    {
        public RolesClass() : base(Enums.Commands.roles) { }
    }
}