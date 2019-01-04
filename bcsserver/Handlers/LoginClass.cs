using System;
using CLProject;
using Newtonsoft.Json;

namespace bcsserver.Handlers
{
    public class LoginClass : ServerLib.JTypes.Server.LoginClass
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
                Token = null;
                UserId = 0;
                Active = 0;
                State = ServerLib.JTypes.Enums.ResponseState.error;
                DatabaseParameterValuesClass Param = new DatabaseParameterValuesClass();
                Param.CreateParameterValue("WebSocketID", UserSession.WebSocketSessionID);
                Param.CreateParameterValue("Login", Request.UserName);
                Param.CreateParameterValue("Password", Request.Password);
                Param.CreateParameterValue("AccessToken");
                Param.CreateParameterValue("AccessUserId");
                Param.CreateParameterValue("Active");
                UserSession.Project.Database.Execute("Login", ref Param);
                long _UserId = Convert.ToInt64(Param.ParameterByName("AccessUserId").Value.ToString());
                int _Active = Convert.ToInt32(Param.ParameterByName("Active").Value.ToString());
                if (_UserId > 0)
                {
                    Token = _Active == 1 ? Param.ParameterByName("AccessToken").Value.ToString() : null;
                    Active = _Active == 1 ? ServerLib.JTypes.Enums.UserActive.activated : ServerLib.JTypes.Enums.UserActive.blocked;
                    UserId = Active == ServerLib.JTypes.Enums.UserActive.activated ? _UserId : 0;
                    State = ServerLib.JTypes.Enums.ResponseState.ok;
                    UserSession.OutputQueueAddObject(this);
                    UserSession.UserInformation.SendData();
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
    }
}
