namespace ServerLib.JTypes.Enums
{
    /// <summary>
    /// Команды выполняемые сервером
    /// </summary>
    public enum Commands
    {
        /// <summary>
        /// Неисполняемая команда
        /// </summary>
        none,

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        login,

        /// <summary>
        /// Выход из системы пользователя
        /// </summary>
        logout,

        /// <summary>
        /// Информация о пользователе
        /// </summary>
        user_information,

        /// <summary>
        /// Список пользователей
        /// </summary>
        users,

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        user_add,

        /// <summary>
        /// Изменение пользователя
        /// </summary>
        user_edit,

        /// <summary>
        /// Список должностей
        /// </summary>
        jobs,

        /// <summary>
        /// Добавление должности
        /// </summary>
        job_add,

        /// <summary>
        /// Изменение должности
        /// </summary>
        job_edit,
        
        /// <summary>
        /// Список всех ролей пользователей
        /// </summary>
        roles,                
        
        /// <summary>
        /// Добавление роли пользователей
        /// </summary>
        roles_add,
        
        /// <summary>
        /// Изменение роли пользователей
        /// </summary>
        roles_edit,

        /// <summary>
        /// Список ролей назначенных пользователям
        /// </summary>
        users_roles,

        /// <summary>
        /// Добавление роли пользователю
        /// </summary>
        users_roles_add,

        /// <summary>
        /// Удаление роли у пользователя
        /// </summary>
        users_roles_delete,

        /// <summary>
        /// Список объектов
        /// </summary>
        objects,

        /// <summary>
        /// Список прав доступа ролей к объектам системы
        /// </summary>
        roles_objects_access_rights
    }
}
