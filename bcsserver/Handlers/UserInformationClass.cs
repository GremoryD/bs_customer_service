using System;
using CLProject;
using Newtonsoft.Json;
using System.Threading;

namespace bcsserver.Handlers
{
    public class UserInformationClass : ServerLib.JTypes.Server.UserInformationClass
    {
        private UserSessionClass UserSession;

        /// <summary>
        /// Таймер обновления данных пользователя
        /// </summary>
        private System.Threading.Timer RefreshDataTimer;

        public UserInformationClass()
        {
            RefreshDataTimer = new System.Threading.Timer(RefreshDataProcessing, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Привязка сессии пользователя 
        /// </summary>
        /// <param name="AUserSession"></param>
        public void SetUserSession(UserSessionClass AUserSession)
        {
            UserSession = AUserSession;
        }

        /// <summary>
        /// Поток обновления данных пользователя
        /// </summary>
        /// <param name="state"></param>
        private void RefreshDataProcessing(object state)
        {
            if (UserSession.IsAuthenticated)
            {
                Thread th = new Thread(() =>
                    {
                        try
                        {
                            DatabaseParameterValuesClass Param = new DatabaseParameterValuesClass();
                            Param.CreateParameterValue("Token", UserSession.Login.Token);
                            Param.CreateParameterValue("UserId", UserSession.Login.UserId);
                            Param.CreateParameterValue("FirstName");
                            Param.CreateParameterValue("LastName");
                            Param.CreateParameterValue("MidleName");
                            Param.CreateParameterValue("Job");
                            Param.CreateParameterValue("Active");
                            Param.CreateParameterValue("State");
                            Param.CreateParameterValue("ErrorText");
                            UserSession.Project.Database.Execute("UserInformation", ref Param);
                            if(Param.ParameterByName("State").Value.ToString() == "ok")
                            {
                                string OldMessage = JsonConvert.SerializeObject(this);
                                FirstName = Param.ParameterByName("FirstName").Value.ToString();
                                LastName = Param.ParameterByName("LastName").Value.ToString();
                                MidleName = Param.ParameterByName("MidleName").Value.ToString();
                                Job = Param.ParameterByName("Job").Value.ToString();
                                Active = Convert.ToInt32(Param.ParameterByName("Active").Value.ToString());
                                State = ServerLib.JTypes.Enums.ResponseState.ok;
                                if(JsonConvert.SerializeObject(this) != OldMessage)
                                {
                                    UserSession.OutputQueueAddObject(this);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            UserSession.Project.Log.Error(string.Format("UserInformation.RefreshDataProcessing > {0}", ex.Message));
                        }
                    });
                th.Start();
            }
            RefreshDataTimer.Change(5000, Timeout.Infinite);
        }

        /// <summary>
        /// Чтение из базы данных и отправка данных пользователя клиенту
        /// </summary>
        public void SendUserInformation()
        {
            RefreshDataTimer.Change(1, Timeout.Infinite);
        }
    }
}
