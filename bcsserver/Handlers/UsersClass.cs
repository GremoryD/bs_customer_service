using System;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    /// <summary>
    /// Пользователи
    /// </summary>
    public class UsersClass : BaseHandlerClass
    {
        /// <summary>
        /// Словарь пользователей
        /// </summary>
        private ConcurrentDictionary<long, ServerLib.JTypes.Server.UserClass> UsersCollection;

        public UsersClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            UsersCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.UserClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
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
                    UserID = UsersTable.AsInt64(row, "ID"),
                    Login = UsersTable.AsString(row, "LOGIN"),
                    FirstName = UsersTable.AsString(row, "FIRST_NAME"),
                    LastName = UsersTable.AsString(row, "LAST_NAME"),
                    MidleName = UsersTable.AsString(row, "MIDLE_NAME"),
                    JobId = UsersTable.AsInt64(row, "POSITION_ID"),
                    Job = UsersTable.AsString(row, "POSITION"),
                    Active = (UserActive)UsersTable.AsInt32(row, "ACTIVE")
                };

                if (UsersCollection.TryGetValue(User.UserID, out ServerLib.JTypes.Server.UserClass ExistUser))
                {
                    if (ExistUser.Hash != User.Hash)
                    {
                        User.Command = ListCommands.edit;
                        UsersCollection.TryUpdate(User.UserID, User, ExistUser);
                        UsersList.Users.Add(User);
                    }
                }
                else
                {
                    User.Command = ListCommands.add;
                    UsersCollection.TryAdd(User.UserID, User);
                    UsersList.Users.Add(User);
                }
            }

            foreach (var User in UsersCollection)
            {
                bool IsExist = false;

                foreach (System.Data.DataRow row in UsersTable.Table.Rows)
                {
                    if (User.Value.UserID == UsersTable.AsInt64(row, "ID"))
                    {
                        IsExist = true;
                        break;
                    }
                }

                if (!IsExist)
                {
                    User.Value.Command = ListCommands.delete;
                    UsersList.Users.Add(User.Value);
                    UsersCollection.TryRemove(User.Value.UserID, out ServerLib.JTypes.Server.UserClass DeletingUser);
                }
            }

            if (UsersList.Users.Count > 0)
            {
                UserSession.OutputQueueAddObject(UsersList);
            }
        }

        /// <summary>
        /// Обработчик добавления пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override void AddProcessing(string ARequest)
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
                        UserID = Convert.ToInt64(Params.ParameterByName("AccessUserId").Value.ToString()),
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
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_add, ErrorCodes.FatalError, ex.Message));
            }
        }

        /// <summary>
        /// Обработчик изменения пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override void EditProcessing(string ARequest)
        {
            try
            {
                ServerLib.JTypes.Client.UserEditClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.UserEditClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("UserId", Request.UserID);
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
                        UserID = Convert.ToInt64(Params.ParameterByName("UserId").Value.ToString()),
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
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_edit, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_edit, ErrorCodes.FatalError, ex.Message));
            }
        }
    }
}
