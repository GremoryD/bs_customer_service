using System;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    public class HandlerRolesObjectsClass : HandlerBaseClass
    {
        /// <summary>
        /// Список разрешений ролей пользователей на операции с объектами системы
        /// </summary>
        private readonly ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseRoleObjectClass> ReadCollection;

        public HandlerRolesObjectsClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseRoleObjectClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.ResponseRolesObjectsClass OutputList = new ServerLib.JTypes.Server.ResponseRolesObjectsClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass ReadTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("RolesObjects", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.ResponseRoleObjectClass Item = new ServerLib.JTypes.Server.ResponseRoleObjectClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    RoleID = ReadTable.AsInt64(row, "ROLE_ID"),
                    ObjectID = ReadTable.AsInt64(row, "OBJECT_ID"),
                    OperationRead = ReadTable.AsInt64(row, "OPERATION_READ") == 1,
                    OperationAdd = ReadTable.AsInt64(row, "OPERATION_ADD") == 1,
                    OperationEdit = ReadTable.AsInt64(row, "OPERATION_EDIT") == 1,
                    OperationDelete = ReadTable.AsInt64(row, "OPERATION_DELETE") == 1
                };

                if (ReadCollection.TryGetValue(Item.ID, out ServerLib.JTypes.Server.ResponseRoleObjectClass ExistItem))
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

            foreach (System.Collections.Generic.KeyValuePair<long, ServerLib.JTypes.Server.ResponseRoleObjectClass> Item in ReadCollection)
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
                    ReadCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.ResponseRoleObjectClass DeletingItem);
                }
            }

            if (OutputList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
            }
        }

        /// <summary>
        /// Обработчик добавления разрешения для роли пользователей к операции над объектом системы
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool AddProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.RequestRolesObjectsAddClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestRolesObjectsAddClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("RoleId", Request.RoleID);
                Params.CreateParameterValue("ObjectId", Request.ObjectID);
                Params.CreateParameterValue("OperationId", (int)Request.ObjectOperation);
                Params.CreateParameterValue("NewId");
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("RolesObjectsAdd", ref Params);
                if (Params.ParameterByName("State").AsString == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseRolesObjectsAddClass
                    {
                        ID = Params.ParameterByName("NewId").AsInt64,
                        RoleID = Request.RoleID,
                        ObjectID = Request.ObjectID,
                        ObjectOperation = Request.ObjectOperation
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.roles_objects_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.roles_objects_add, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }

        /// <summary>
        /// Обработчик удаления разрешения для роли пользователей к операции над объектом системы
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool DeleteProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.RequestRolesObjectsDeleteClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestRolesObjectsDeleteClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("OperationId", Request.RolesObjectsPermissionID);
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("RolesObjectsDelete", ref Params);
                if (Params.ParameterByName("State").AsString == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseRolesObjectsDeleteClass()
                    {
                        ID = Request.RolesObjectsPermissionID
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.roles_objects_delete, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.roles_objects_delete, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }
    }
}
