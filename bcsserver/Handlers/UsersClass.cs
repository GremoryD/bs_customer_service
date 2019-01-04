using System;
using CLProject;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;

using System.Collections.Generic;

namespace bcsserver.Handlers
{
    public class UsersClass
    {
        private UserSessionClass UserSession;

        /// <summary>
        /// Таймер обновления данных 
        /// </summary>
        private System.Threading.Timer RefreshDataTimer;

        private ConcurrentDictionary<long, ServerLib.JTypes.Server.UserClass> UsersCollection;

        /// <summary>
        /// Признак запуска потока чтения данных
        /// </summary>
        private bool IsRunning = false;

        public UsersClass(UserSessionClass AUserSession)
        {
            UsersCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.UserClass>();
            RefreshDataTimer = new System.Threading.Timer(RefreshDataProcessing, null, Timeout.Infinite, Timeout.Infinite);
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
                        ServerLib.JTypes.Server.UsersClass UsersList = new ServerLib.JTypes.Server.UsersClass();
                        System.Data.DataTable UsersTable;
                        DatabaseParameterValuesClass Param = new DatabaseParameterValuesClass();
                        Param.CreateParameterValue("Token", UserSession.Login.Token);
                        UsersTable = (System.Data.DataTable)UserSession.Project.Database.Execute("Users", ref Param);

                        foreach (System.Data.DataRow row in UsersTable.Rows)
                        {
                            ServerLib.JTypes.Server.UserClass User = new ServerLib.JTypes.Server.UserClass
                            {
                                Id = Convert.ToInt64(row["ID"].ToString()),
                                Login = row["LOGIN"].ToString(),
                                FirstName = row["FIRST_NAME"] == DBNull.Value ? null : row["FIRST_NAME"].ToString(),
                                LastName = row["LAST_NAME"] == DBNull.Value ? null : row["LAST_NAME"].ToString(),
                                MidleName = row["MIDLE_NAME"] == DBNull.Value ? null : row["MIDLE_NAME"].ToString(),
                                Job = row["POSITION"].ToString(),
                                Active = Convert.ToInt32(row["ACTIVE"].ToString()) == 1 ? ServerLib.JTypes.Enums.UserActive.activated : ServerLib.JTypes.Enums.UserActive.blocked
                            };
                            if (UsersCollection.TryGetValue(User.Id, out ServerLib.JTypes.Server.UserClass ExistUser))
                            {
                                if (ExistUser.Hash != User.Hash)
                                {
                                    User.Command = ServerLib.JTypes.Enums.ListCommands.edit;
                                    UsersCollection.TryUpdate(User.Id, User, ExistUser);
                                    UsersList.Users.Add(User);
                                }
                            }
                            else
                            {
                                User.Command = ServerLib.JTypes.Enums.ListCommands.add;
                                UsersCollection.TryAdd(User.Id, User);
                                UsersList.Users.Add(User);
                            }
                        }

                        foreach (var User in UsersCollection)
                        {
                            bool IsExist = false;
                            foreach (System.Data.DataRow row in UsersTable.Rows)
                            {
                                if (User.Value.Id == Convert.ToInt64(row["ID"].ToString()))
                                {
                                    IsExist = true;
                                    break;
                                }
                            }
                            if(!IsExist)
                            {
                                User.Value.Command = ServerLib.JTypes.Enums.ListCommands.delete;
                                UsersList.Users.Add(User.Value);
                                UsersCollection.TryRemove(User.Value.Id, out ServerLib.JTypes.Server.UserClass DeletingUser);
                            }
                        }

                        if (UsersList.Users.Count > 0)
                        {
                            UserSession.OutputQueueAddObject(UsersList);
                        }
                    }
                    catch (Exception ex)
                    {
                        UserSession.Project.Log.Error(string.Format("Users.RefreshDataProcessing {0}", ex.Message));
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
