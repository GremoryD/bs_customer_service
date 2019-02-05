using System;
using CLProject;
using Newtonsoft.Json;

namespace bcsserver.Handlers
{
    public class LogoutClass : ServerLib.JTypes.Server.ResponseLogoutClass
    {
        private UserSessionClass UserSession;

        public void SetUserSession(UserSessionClass AUserSession)
        {
            UserSession = AUserSession;
        }

        /// <summary>
        /// Выход пользователя из системы
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void Logout(string ARequest)
        {
            try
            {
                ServerLib.JTypes.Client.RequestLogoutClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestLogoutClass>(ARequest);
                UserSession.OutputQueueAddObject(KillSession(Request.Token));
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(ServerLib.JTypes.Enums.Commands.logout, ServerLib.JTypes.Enums.ErrorCodes.FatalError, ex.Message));
            }
        }

        /// <summary>
        /// Удаление сессии пользователя
        /// </summary>
        /// <param name="ASession">Идентификатор сессии</param>
        /// <returns></returns>
        public object KillSession(string ASession)
        {
            DatabaseParameterValuesClass Param = new DatabaseParameterValuesClass();
            Param.CreateParameterValue("Token", ASession);
            Param.CreateParameterValue("State");
            UserSession.Project.Database.Execute("Logout", ref Param);
            UserSession.Login.ID = 0;
            ServerLib.JTypes.Server.ResponseLogoutClass Response = new ServerLib.JTypes.Server.ResponseLogoutClass();
            if (Param.ParameterByName("State").AsString== "ok")
            {
                Response.State = ServerLib.JTypes.Enums.ResponseState.ok;
            }
            else
            {
                Response.State = ServerLib.JTypes.Enums.ResponseState.error;
                Response.ErrorText = "Active session not found";
            }
            return Response;
        }
    }
}
