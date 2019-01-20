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
        /// Список назначенных ролей пользователей
        /// </summary>
        public ConcurrentDictionary<long, ServerLib.JTypes.Server.UserRoleClass> ReadCollection;

        public HandlerUsersRolesClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.UserRoleClass>();
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
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("UsersRoles", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.UserRoleClass Item = new ServerLib.JTypes.Server.UserRoleClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    UserID = ReadTable.AsInt64(row, "USER_ID"),
                    RoleID = ReadTable.AsInt64(row, "ROLE_ID"),
                    RoleName = ReadTable.AsString(row, "ROLE_NAME")
                };

                if (ReadCollection.TryGetValue(Item.ID, out ServerLib.JTypes.Server.UserRoleClass ExistItem))
                {
                    if (ExistItem.Hash != Item.Hash)
                    {
                        Item.Command = ListCommands.edit;
                        ReadCollection.TryUpdate(Item.ID, Item, ExistItem);
                        OutputList.Items.Add(Item);
                    }
                }
                else
                {
                    Item.Command = ListCommands.add;
                    ReadCollection.TryAdd(Item.ID, Item);
                    OutputList.Items.Add(Item);
                }
            }

            foreach (System.Collections.Generic.KeyValuePair<long, ServerLib.JTypes.Server.UserRoleClass> Item in ReadCollection)
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
                    ReadCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.UserRoleClass DeletingItem);
                }
            }

            if (OutputList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
            }
        }

    }
}