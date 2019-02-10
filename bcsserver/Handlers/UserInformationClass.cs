using System;
using CLProject;
using Newtonsoft.Json;
using System.Threading;

namespace bcsserver.Handlers
{
    public class UserInformationClass : ServerLib.JTypes.Server.ResponseUserInformationClass
    {
        private UserSessionClass UserSession;

        /// <summary>
        /// Таймер обновления данных 
        /// </summary>
        private System.Threading.Timer RefreshDataTimer;

        /// <summary>
        /// Признак запуска потока чтения данных
        /// </summary>
        private bool IsRunning = false;

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
        /// Поток обновления данных 
        /// </summary>
        /// <param name="state"></param>
        private void RefreshDataProcessing(object state)
        {
            if (UserSession.IsAuthenticated && !IsRunning)
            {
                Thread th = new Thread(() =>
                    {
                        try
                        {
                            IsRunning = true;
                            DatabaseParameterValuesClass Param = new DatabaseParameterValuesClass();
                            Param.CreateParameterValue("Token", UserSession.Login.Token);
                            Param.CreateParameterValue("UserId", UserSession.Login.ID);
                            Param.CreateParameterValue("FirstName");
                            Param.CreateParameterValue("LastName");
                            Param.CreateParameterValue("MidleName");
                            Param.CreateParameterValue("Job");
                            Param.CreateParameterValue("Active");
                            Param.CreateParameterValue("State");
                            Param.CreateParameterValue("ErrorText");
                            UserSession.Project.Database.Execute("UserInformation", ref Param);
                            if (Param.ParameterByName("State").AsString == "ok")
                            {
                                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                                Params.CreateParameterValue("Token", UserSession.Login.Token);
                                DatabaseTableClass ReadTable = new DatabaseTableClass
                                {
                                    Table = (System.Data.DataTable)UserSession.Project.Database.Execute("UserPermissions", ref Params)
                                };
                                Permissions.Clear();
                                foreach (System.Data.DataRow row in ReadTable.Table.Rows)
                                {
                                    Permissions.Add(new ServerLib.JTypes.Server.ResponseUserPermissionClass
                                    {
                                        ObjectName = ReadTable.AsString(row, "OBJECT"),
                                        Operation = (ServerLib.JTypes.Enums.ObjectOperations)ReadTable.AsInt32(row, "OPERATION")
                                    });
                                }

                                string OldMessage = JsonConvert.SerializeObject(this);
                                FirstName = Param.ParameterByName("FirstName").AsString;
                                LastName = Param.ParameterByName("LastName").AsString;
                                MidleName = Param.ParameterByName("MidleName").AsString;
                                JobName = Param.ParameterByName("Job").AsString;
                                Active = Param.ParameterByName("Active").AsInt32 == 1 ? ServerLib.JTypes.Enums.UserActive.activated : ServerLib.JTypes.Enums.UserActive.blocked;
                                State = ServerLib.JTypes.Enums.ResponseState.ok;
                                if (JsonConvert.SerializeObject(this) != OldMessage)
                                {
                                    UserSession.OutputQueueAddObject(this);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            UserSession.Project.Log.Error(string.Format("UserInformation.RefreshDataProcessing {0}", ex.Message));
                        }
                        finally
                        {
                            IsRunning = false;
                        }
                    });
                th.Start();
            }
            RefreshDataTimer.Change(5000, Timeout.Infinite);
        }

        /// <summary>
        /// Принудительное чтение из базы данных и отправка данных клиенту
        /// </summary>
        public void SendData()
        {
            RefreshDataTimer.Change(1, Timeout.Infinite);
        }
    }
}
