using System;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    public class HandlerUsersRolesClass : BaseHandlerClass
    {
        /// <summary>
        /// Словарь ролей пользователей
        /// </summary>
        public ConcurrentDictionary<long, ServerLib.JTypes.Server.UsersRoleClass> ReadCollection;

        public HandlerUsersRolesClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.UsersRoleClass>();
        }
        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.UsersRolesClass OutputList = new ServerLib.JTypes.Server.UsersRolesClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass ReadTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("Roles", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.UsersRoleClass Role = new ServerLib.JTypes.Server.UsersRoleClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    Name = ReadTable.AsString(row, "NAME"),
                    Description = ReadTable.AsString(row, "DESCRIPTION")
                };

                if (ReadCollection.TryGetValue(Role.ID, out ServerLib.JTypes.Server.UsersRoleClass ExistRole))
                {
                    if (ExistRole.Hash != Role.Hash)
                    {
                        Role.Command = ListCommands.edit;
                        ReadCollection.TryUpdate(Role.ID, Role, ExistRole);
                        OutputList.Items.Add(Role);
                    }
                }
                else
                {
                    Role.Command = ListCommands.add;
                    ReadCollection.TryAdd(Role.ID, Role);
                    OutputList.Items.Add(Role);
                }
            }

            foreach (var Item in ReadCollection)
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
                    Item.Value.Command = ListCommands.delete;
                    OutputList.Items.Add(Item.Value);
                    ReadCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.UsersRoleClass DeletingItem);
                }
            }

            if (OutputList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
            }
        }

        /// <summary>
        /// Обработчик добавления роли пользователей
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool AddProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.UsersRoleAddClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.UsersRoleAddClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("Name", Request.Name);
                Params.CreateParameterValue("Description", Request.Description);
                Params.CreateParameterValue("NewId");
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("RoleAdd", ref Params);
                if (Params.ParameterByName("State").Value.ToString() == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.UsersRoleAddClass
                    {
                        ID = Convert.ToInt64(Params.ParameterByName("NewId").Value.ToString()),
                        Name = Request.Name.Trim(),
                        Description = Request.Description
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.users_roles_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.users_roles_add, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }

        /// <summary>
        /// Обработчик изменения роли пользователей
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool EditProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.UsersRoleEditClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.UsersRoleEditClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("Name", Request.Name);
                Params.CreateParameterValue("InId", Request.ID);
                Params.CreateParameterValue("Description", Request.Description);
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("RoleEdit", ref Params);
                if (Params.ParameterByName("State").Value.ToString() == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.UsersRoleEditClass
                    {
                        ID = Request.ID,
                        Name = Request.Name.Trim(),
                        Description = Request.Description
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.users_roles_edit, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.users_roles_edit, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }
    }
}