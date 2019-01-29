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
        private readonly ConcurrentDictionary<long, ServerLib.JTypes.Server.RoleObjectClass> ReadCollection;

        public HandlerRolesObjectsClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.RoleObjectClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.RolesObjectsClass OutputList = new ServerLib.JTypes.Server.RolesObjectsClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass ReadTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("RolesObjects", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.RoleObjectClass Item = new ServerLib.JTypes.Server.RoleObjectClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    RoleID = ReadTable.AsInt64(row, "ROLE_ID"),
                    ObjectID = ReadTable.AsInt64(row, "OBJECT_ID"),
                    ObjectOperation = (Operations)ReadTable.AsInt32(row, "OPERATION")
                };

                if (ReadCollection.TryGetValue(Item.ID, out ServerLib.JTypes.Server.RoleObjectClass ExistItem))
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

            foreach (System.Collections.Generic.KeyValuePair<long, ServerLib.JTypes.Server.RoleObjectClass> Item in ReadCollection)
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
                    ReadCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.RoleObjectClass DeletingItem);
                }
            }

            if (OutputList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
            }
        }
    }
}
