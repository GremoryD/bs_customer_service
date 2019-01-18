using System;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    public class HandlerUsersRolesClass : BaseHandlerClass
    {
        /// <summary>
        /// Словарь ролей пользователей
        /// </summary>
        private ConcurrentDictionary<long, ServerLib.JTypes.Server.UsersRolesClass> UsersRolesCollection;

        public HandlerUsersRolesClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            UsersRolesCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.UsersRolesClass>();
        }
    }
}
