﻿using System;
using System.Threading;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    /// <summary>
    /// Пользователи
    /// </summary>
    public class HandlerUsersClass : HandlerBaseClass
    {
        /// <summary>
        /// Словарь пользователей
        /// </summary>
        private ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseUserClass> ReadCollection;

        public HandlerUsersClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseUserClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.ResponseUsersClass OutputList = new ServerLib.JTypes.Server.ResponseUsersClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass ReadTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("Users", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.ResponseUserClass Item = new ServerLib.JTypes.Server.ResponseUserClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    Login = ReadTable.AsString(row, "LOGIN"),
                    FirstName = ReadTable.AsString(row, "FIRST_NAME"),
                    LastName = ReadTable.AsString(row, "LAST_NAME"),
                    MidleName = ReadTable.AsString(row, "MIDLE_NAME"),
                    JobID = ReadTable.AsInt64(row, "POSITION_ID"),
                    JobName = ReadTable.AsString(row, "POSITION"),
                    Active = (UserActive)ReadTable.AsInt32(row, "ACTIVE")
                };

                if (ReadCollection.TryGetValue(Item.ID, out ServerLib.JTypes.Server.ResponseUserClass ExistItem))
                {
                    if (ExistItem.Hash != Item.Hash)
                    {
                        Item.Command = ItemCommands.edit;
                        ReadCollection.TryUpdate(Item.ID, Item, ExistItem);
                        OutputList.Items.Add(Item);
                    }
                }
                else
                {
                    Item.Command = ItemCommands.add;
                    ReadCollection.TryAdd(Item.ID, Item);
                    OutputList.Items.Add(Item);
                }
            }

            foreach (System.Collections.Generic.KeyValuePair<long, ServerLib.JTypes.Server.ResponseUserClass> Item in ReadCollection)
            {
                bool IsExist = false;

                foreach (System.Data.DataRow row in ReadTable.Table.Rows)
                {
                    if (Item.Value.ID == ReadTable.AsInt64(row, "ID"))
                    {
                        IsExist = true;
                        break;
                    }
                }

                if (!IsExist)
                {
                    Item.Value.Command = ItemCommands.delete;
                    OutputList.Items.Add(Item.Value);
                    ReadCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.ResponseUserClass DeletingUser);
                }
            }

            if (OutputList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
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
                ServerLib.JTypes.Client.RequestUserAddClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestUserAddClass>(ARequest);
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
                if (Params.ParameterByName("State").AsString == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseUserAddClass
                    {
                        ID = Params.ParameterByName("AccessUserId").AsInt64,
                        Login = Request.Login,
                        FirstName = Request.FirstName,
                        LastName = Request.LastName,
                        MidleName = Request.MidleName,
                        Active = Request.Active,
                        JobID = Request.JobID,
                        JobName = Params.ParameterByName("Job").AsString
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.user_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.user_add, ErrorCodes.FatalError, ex.Message));
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
                ServerLib.JTypes.Client.RequestUserEditClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestUserEditClass>(ARequest);
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
                if (Params.ParameterByName("State").AsString == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseUserEditClass
                    {
                        ID = Request.ID,
                        FirstName = Request.FirstName,
                        LastName = Request.LastName,
                        MidleName = Request.MidleName,
                        Active = Request.Active,
                        JobId = Request.JobID,
                        JobName = Params.ParameterByName("Job").AsString
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.user_edit, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.user_edit, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void ChangePassword(string ARequest)
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    ServerLib.JTypes.Client.RequestUserPasswordChangeClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestUserPasswordChangeClass>(ARequest);
                    DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                    Params.CreateParameterValue("Token", Request.Token);
                    Params.CreateParameterValue("UserId", Request.ID);
                    Params.CreateParameterValue("Password", Request.Password);
                    Params.CreateParameterValue("State");
                    Params.CreateParameterValue("ErrorText");
                    UserSession.Project.Database.Execute("UserPasswordChange", ref Params);
                    if (Params.ParameterByName("State").AsString == "ok")
                    {
                        UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseUserPasswordChangeClass
                        {
                            ID = Request.ID
                        });
                    }
                    else
                    {
                        UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.user_password_change, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                    }
                }
                catch (Exception ex)
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.user_password_change, ErrorCodes.FatalError, ex.Message));
                }
            });
            th.Start();
        }
    }
}
