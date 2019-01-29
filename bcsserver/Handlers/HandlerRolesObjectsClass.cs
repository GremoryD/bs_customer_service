using System;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    public class HandlerRolesObjectsClass : HandlerBaseClass
    {
        /// <summary>
        /// Список разрешений ролей пользователей
        /// </summary>
        private readonly ConcurrentDictionary<long, ServerLib.JTypes.Server.JobClass> ReadCollection;

        public HandlerRolesObjectsClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.JobClass>();
        }
    }
}
