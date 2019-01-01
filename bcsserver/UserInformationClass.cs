using System;
using CLProject;
using Newtonsoft.Json;

namespace bcsserver
{
    public class UserInformationClass : ServerLib.JTypes.Server.UserInformationClass
    {
        private UserSessionClass UserSession;

        public void SetUserSession(UserSessionClass AUserSession)
        {
            UserSession = AUserSession;
        }

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void Login(string ARequest)
        {
            try
            {
                ServerLib.JTypes.Client.Login Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.Login>(ARequest);
                DatabaseParameterValuesClass Param = new DatabaseParameterValuesClass();
                Param.CreateParameterValue("WebSocketID", UserSession.WebSocketSessionID);
                Param.CreateParameterValue("Login", Request.UserName);
                Param.CreateParameterValue("Password", Request.Password);
                Param.CreateParameterValue("AccessToken");
                Param.CreateParameterValue("UserId");
                Param.CreateParameterValue("FirstName");
                Param.CreateParameterValue("LastName");
                Param.CreateParameterValue("MidleName");
                Param.CreateParameterValue("Job");
                Param.CreateParameterValue("LastLoginDate");
                Param.CreateParameterValue("Active");
                UserSession.Project.Database.Execute("Login", ref Param);
                long _UserId = Convert.ToInt64(Param.ParameterByName("UserId").Value.ToString());
                if (_UserId > 0)
                {
                    Token = Param.ParameterByName("AccessToken").Value.ToString();
                    FirstName = Param.ParameterByName("FirstName").Value.ToString();
                    LastName = Param.ParameterByName("LastName").Value.ToString();
                    MidleName = Param.ParameterByName("MidleName").Value.ToString();
                    Job = Param.ParameterByName("Job").Value.ToString();
                    UserId = _UserId;
                    LastLoginDate = Param.ParameterByName("LastLoginDate").Value.ToString();
                    Active = Convert.ToInt32(Param.ParameterByName("Active").Value.ToString());
                    State = ServerLib.JTypes.Enums.ResponseState.ok;
                    UserSession.OutputQueueAddObject(this);
                }
                else
                {
                    UserSession.OutputQueueAddObject(ServerLib.JTypes.Server.Exceptions.ErrorIncorrectLoginOrPassword);
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new { command = "login", code = ex.HResult, description = ex.Message });
            }
        }

        /// <summary>
        /// Выход пользователя из системы
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void Logout(string ARequest)
        {
            try
            { 
                ServerLib.JTypes.Client.Logout Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.Logout>(ARequest);
                UserSession.OutputQueueAddObject(KillSession(Request.Token));
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new { command = "logout", code = ex.HResult, description = ex.Message });
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
            UserId = 0;
            ServerLib.JTypes.Server.Logout Response = new ServerLib.JTypes.Server.Logout();
            if (Param.ParameterByName("State").Value.ToString() == "ok")
            {
                Response.State = ServerLib.JTypes.Enums.ResponseState.ok;
            }
            else
            {
                Response.State = ServerLib.JTypes.Enums.ResponseState.error;
                Response.Description = "Active session not found";
            }
            return Response;
        }
    }
}
