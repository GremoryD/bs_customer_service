﻿using System;
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
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("WebSocketID", UserSession.WebSocketSessionID);
                Params.CreateParameterValue("Login", Request.UserName);
                Params.CreateParameterValue("Password", Request.Password);
                Params.CreateParameterValue("AccessToken");
                Params.CreateParameterValue("AccessUserId");
                Params.CreateParameterValue("Active");
                UserSession.Project.Database.Execute("Login", ref Params);
                long _UserId = Convert.ToInt64(Params.ParameterByName("AccessUserId").Value.ToString());
                int _Active = Convert.ToInt32(Params.ParameterByName("Active").Value.ToString());
                if (_UserId > 0)
                {
                    Token = _Active == 1 ? Params.ParameterByName("AccessToken").Value.ToString() : null;
                    Active = _Active == 1 ? ServerLib.JTypes.Enums.UserActive.activated : ServerLib.JTypes.Enums.UserActive.blocked;
                    UserId = Active == ServerLib.JTypes.Enums.UserActive.activated ? _UserId : 0;
                    State = ServerLib.JTypes.Enums.ResponseState.ok;
                    UserSession.OutputQueueAddObject(this);
                    UserSession.UserInformation.SendData();
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.login, ServerLib.JTypes.Enums.ErrorCodes.IncorrectLoginOrPassword));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.login, ServerLib.JTypes.Enums.ErrorCodes.FatalError, ex.Message));
            }
        }
    }
}
