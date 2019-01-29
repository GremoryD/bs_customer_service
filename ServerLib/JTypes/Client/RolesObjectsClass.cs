namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос списка прав доступа ролей пользователей к операциям над объектами сиситемы
    /// </summary>
    public class RolesObjectsClass : BaseRequestClass
    {
        public RolesObjectsClass() : base(Enums.Commands.roles_objects) { }
    }
}
