using System;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{

    public class HandlerUsersRolesClass : HandlerBaseClass
    {
        /// <summary>
        /// Список назначенных ролей пользователей
        /// </summary>
        public ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseUserRoleClass> ReadCollection;

        public HandlerUsersRolesClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseUserRoleClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.ResponseUsersRolesClass OutputList = new ServerLib.JTypes.Server.ResponseUsersRolesClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass ReadTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("UsersRoles", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.ResponseUserRoleClass Item = new ServerLib.JTypes.Server.ResponseUserRoleClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    UserID = ReadTable.AsInt64(row, "USER_ID"),
                    RoleID = ReadTable.AsInt64(row, "ROLE_ID"),
                    RoleName = ReadTable.AsString(row, "ROLE_NAME")
                };

                if (ReadCollection.TryGetValue(Item.ID, out ServerLib.JTypes.Server.ResponseUserRoleClass ExistItem))
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

            foreach (System.Collections.Generic.KeyValuePair<long, ServerLib.JTypes.Server.ResponseUserRoleClass> Item in ReadCollection)
            {
                bool IsExist = false;

                foreach (System.Data.DataRow row in ReadTable.Table.Rows)
                {
                    if (Item.Key == ReadTable.AsInt64(row, "ID"))
                    {
                        IsExist = true;
                        break;
                    }
                }

                if (!IsExist)
                {
                    Item.Value.Command = ItemCommands.delete;
                    OutputList.Items.Add(Item.Value);
                    ReadCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.ResponseUserRoleClass DeletingItem);
                }
            }

            if (OutputList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
            }
        }

        /// <summary>
        /// Обработчик добавления роли пользователю
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool AddProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.RequestUserRoleAddClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestUserRoleAddClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("UserID", Request.UserID);
                Params.CreateParameterValue("RoleID", Request.RoleID);
                Params.CreateParameterValue("NewId");
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("UsersRolesAdd", ref Params);
                if (Params.ParameterByName("State").AsString== "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseUserRoleAddClass
                    {
                        ID = Params.ParameterByName("NewId").AsInt64,
                        UserID = Request.UserID,
                        RoleID = Request.RoleID
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.users_roles_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.users_roles_add, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }

        /// <summary>
        /// Обработчик удаления роли пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool DeleteProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.RequestUserRoleDeleteClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestUserRoleDeleteClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("RoleID", Request.UserRoleID);
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("UsersRolesDelete", ref Params);
                if (Params.ParameterByName("State").AsString== "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseUserRoleDeleteClass());
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.users_roles_delete, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.users_roles_delete, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }
    }
}