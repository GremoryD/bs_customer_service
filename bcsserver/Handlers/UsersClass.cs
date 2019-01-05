using System;
using CLProject;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Concurrent;

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
            if (UserSession.IsAuthenticated)
            {
                if (!IsRunning)
                {
                    Thread th = new Thread(() =>
                    {
                        try
                        {
                            IsRunning = true;
                            ServerLib.JTypes.Server.UsersClass UsersList = new ServerLib.JTypes.Server.UsersClass();
                            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                            Params.CreateParameterValue("Token", UserSession.Login.Token);
                            DatabaseTableClass UsersTable = new DatabaseTableClass
                            {
                                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("Users", ref Params)
                            };

                            foreach (System.Data.DataRow row in UsersTable.Table.Rows)
                            {
                                ServerLib.JTypes.Server.UserClass User = new ServerLib.JTypes.Server.UserClass
                                {
                                    Id = UsersTable.AsInt64(row, "ID"),
                                    Login = UsersTable.AsString(row, "LOGIN"),
                                    FirstName = UsersTable.AsString(row, "FIRST_NAME"),
                                    LastName = UsersTable.AsString(row, "LAST_NAME"),
                                    MidleName = UsersTable.AsString(row, "MIDLE_NAME"),
                                    JobId = UsersTable.AsInt64(row, "POSITION_ID"),
                                    Job = UsersTable.AsString(row, "POSITION"),
                                    Active = (ServerLib.JTypes.Enums.UserActive)Convert.ToInt32(row["ACTIVE"].ToString())
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

                                foreach (System.Data.DataRow row in UsersTable.Table.Rows)
                                {
                                    if (User.Value.Id == UsersTable.AsInt64(row, "ID"))
                                    {
                                        IsExist = true;
                                        break;
                                    }
                                }

                                if (!IsExist)
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
        }

        /// <summary>
        /// Принудительное чтение из базы данных и отправка данных клиенту
        /// </summary>
        public void SendData()
        {
            RefreshDataTimer.Change(1, Timeout.Infinite);
        }

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void UserAdd(string ARequest)
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    ServerLib.JTypes.Client.UserAddClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.UserAddClass>(ARequest);
                    DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                    Params.CreateParameterValue("Token", Request.Token);
                    Params.CreateParameterValue("Login", Request.Login);
                    Params.CreateParameterValue("Password", Request.Password);
                    Params.CreateParameterValue("InFirstName", Request.FirstName);
                    Params.CreateParameterValue("InLastName", Request.LastName);
                    Params.CreateParameterValue("InMidleName", Request.MidleName);
                    Params.CreateParameterValue("InActive", Request.Active);
                    Params.CreateParameterValue("JobId", Request.JobId);
                    Params.CreateParameterValue("AccessUserId");
                    Params.CreateParameterValue("Job");
                    Params.CreateParameterValue("State");
                    Params.CreateParameterValue("ErrorText");
                    UserSession.Project.Database.Execute("UserAdd", ref Params);
                    if (Params.ParameterByName("State").Value.ToString() == "ok")
                    {
                        UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.UserAddClass
                        {
                            Id = Convert.ToInt64(Params.ParameterByName("AccessUserId").Value.ToString()),
                            Login = Request.Login,
                            FirstName = Request.FirstName,
                            LastName = Request.LastName,
                            MidleName = Request.MidleName,
                            Active = Request.Active,
                            JobId = Request.JobId,
                            Job = Params.ParameterByName("Job").Value.ToString()
                        });
                    }
                    else
                    {
                        UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.user_add, ServerLib.JTypes.Enums.ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.user_add, ServerLib.JTypes.Enums.ErrorCodes.FatalError, ex.Message));
                }
            });
            th.Start();
        }

        /// <summary>
        /// Изменение пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void UserEdit(string ARequest)
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    ServerLib.JTypes.Client.UserEditClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.UserEditClass>(ARequest);
                    DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                    Params.CreateParameterValue("Token", Request.Token);
                    Params.CreateParameterValue("UserId", Request.Id);
                    Params.CreateParameterValue("Password", Request.Password);
                    Params.CreateParameterValue("InFirstName", Request.FirstName);
                    Params.CreateParameterValue("InLastName", Request.LastName);
                    Params.CreateParameterValue("InMidleName", Request.MidleName);
                    Params.CreateParameterValue("InActive", Request.Active);
                    Params.CreateParameterValue("InActive", Request.Active);
                    Params.CreateParameterValue("JobId", Request.JobId);
                    Params.CreateParameterValue("Job");
                    Params.CreateParameterValue("State");
                    Params.CreateParameterValue("ErrorText");
                    UserSession.Project.Database.Execute("UserEdit", ref Params);
                    if (Params.ParameterByName("State").Value.ToString() == "ok")
                    {
                        UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.UserEditClass
                        {
                            Id = Convert.ToInt64(Params.ParameterByName("UserId").Value.ToString()),
                            FirstName = Request.FirstName,
                            LastName = Request.LastName,
                            MidleName = Request.MidleName,
                            Active = Request.Active,
                            JobId = Request.JobId,
                            Job = Params.ParameterByName("Job").Value.ToString()
                        });
                    }
                    else
                    {
                        UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.user_edit, ServerLib.JTypes.Enums.ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.user_edit, ServerLib.JTypes.Enums.ErrorCodes.FatalError, ex.Message));
                }
            });
            th.Start();
        }
    }
}
