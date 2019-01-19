﻿namespace ServerLib.JTypes.Enums
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
        /// Список ролей пользователей
        /// </summary>
        users_roles,                
        
        /// <summary>
        /// Добавление роли пользователей
        /// </summary>
        users_roles_add,
        
        /// <summary>
        /// Изменение роли пользователей
        /// </summary>
        users_roles_edit, 
    }
}
