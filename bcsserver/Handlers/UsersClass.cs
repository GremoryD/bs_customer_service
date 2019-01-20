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
                    ID = UsersTable.AsInt64(row, "ID"),
                    Login = UsersTable.AsString(row, "LOGIN"),
                    FirstName = UsersTable.AsString(row, "FIRST_NAME"),
                    LastName = UsersTable.AsString(row, "LAST_NAME"),
                    MidleName = UsersTable.AsString(row, "MIDLE_NAME"),
                    JobID = UsersTable.AsInt64(row, "POSITION_ID"),
                    JobName = UsersTable.AsString(row, "POSITION"),
                    Active = (UserActive)UsersTable.AsInt32(row, "ACTIVE")
                };

                if (UsersCollection.TryGetValue(User.ID, out ServerLib.JTypes.Server.UserClass ExistUser))
                {
                    if (ExistUser.Hash != User.Hash)
                    {
                        User.Command = ListCommands.edit;
                        UsersCollection.TryUpdate(User.ID, User, ExistUser);
                        UsersList.Items.Add(User);
                    }
                }
                else
                {
                    User.Command = ListCommands.add;
                    UsersCollection.TryAdd(User.ID, User);
                    UsersList.Items.Add(User);
                }
            }

            foreach (var User in UsersCollection)
            {
                bool IsExist = false;

                foreach (System.Data.DataRow row in UsersTable.Table.Rows)
                {
                    if (User.Value.ID == UsersTable.AsInt64(row, "ID"))
                    {
                        IsExist = true;
                        break;
                    }
                }

                if (!IsExist)
                {
                    User.Value.Command = ListCommands.delete;
                    UsersList.Items.Add(User.Value);
                    UsersCollection.TryRemove(User.Value.ID, out ServerLib.JTypes.Server.UserClass DeletingUser);
                }
            }

            if (UsersList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(UsersList);
            }
        }

        /// <summary>
        /// Обработчик добавления пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool AddProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
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
                Params.CreateParameterValue("JobId", Request.JobID);
                Params.CreateParameterValue("AccessUserId");
                Params.CreateParameterValue("Job");
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("UserAdd", ref Params);
                if (Params.ParameterByName("State").AsString() == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.UserAddClass
                    {
                        ID = Params.ParameterByName("AccessUserId").AsInt64(),
                        Login = Request.Login,
                        FirstName = Request.FirstName,
                        LastName = Request.LastName,
                        MidleName = Request.MidleName,
                        Active = Request.Active,
                        JobID = Request.JobID,
                        JobName = Params.ParameterByName("Job").AsString()
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_add, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }

        /// <summary>
        /// Обработчик изменения пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool EditProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.UserEditClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.UserEditClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("UserId", Request.ID);
                Params.CreateParameterValue("InFirstName", Request.FirstName);
                Params.CreateParameterValue("InLastName", Request.LastName);
                Params.CreateParameterValue("InMidleName", Request.MidleName);
                Params.CreateParameterValue("InActive", Request.Active);
                Params.CreateParameterValue("InActive", Request.Active);
                Params.CreateParameterValue("JobId", Request.JobID);
                Params.CreateParameterValue("Job");
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("UserEdit", ref Params);
                if (Params.ParameterByName("State").AsString() == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.UserEditClass
                    {
                        ID = Request.ID,
                        FirstName = Request.FirstName,
                        LastName = Request.LastName,
                        MidleName = Request.MidleName,
                        Active = Request.Active,
                        JobId = Request.JobID,
                        JobName = Params.ParameterByName("Job").AsString()
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_edit, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.user_edit, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }
    }
}
