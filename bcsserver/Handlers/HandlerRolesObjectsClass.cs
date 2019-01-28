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
        /// Словарь должностей пользователей
        /// </summary>
        private readonly ConcurrentDictionary<long, ServerLib.JTypes.Server.JobClass> JobsCollection;

        public HandlerRolesObjectsClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            JobsCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.JobClass>();
        }
    }
}
