using CLProject;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    public class HandlerObjectsClass : HandlerBaseClass
    {
        /// <summary>
        /// Обработчик списка объектов системы
        /// </summary>
        public ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseObjectClass> ReadCollection;

        public HandlerObjectsClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            ReadCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseObjectClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.ResponseObjectsClass OutputList = new ServerLib.JTypes.Server.ResponseObjectsClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass ReadTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("Objects", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.ResponseObjectClass Item = new ServerLib.JTypes.Server.ResponseObjectClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    Name = ReadTable.AsString(row, "NAME"),
                    Description = ReadTable.AsString(row, "DESCRIPTION"),
                    OperationRead = ReadTable.AsInt64(row, "OPERATION_READ") == 1,
                    OperationAdd = ReadTable.AsInt64(row, "OPERATION_ADD") == 1,
                    OperationEdit = ReadTable.AsInt64(row, "OPERATION_EDIT") == 1,
                    OperationDelete = ReadTable.AsInt64(row, "OPERATION_DELETE") == 1
                };

                if (ReadCollection.TryGetValue(Item.ID, out ServerLib.JTypes.Server.ResponseObjectClass ExistItem))
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

            foreach (System.Collections.Generic.KeyValuePair<long, ServerLib.JTypes.Server.ResponseObjectClass> Item in ReadCollection)
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
                    ReadCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.ResponseObjectClass DeletingItem);
                }
            }

            if (OutputList.Items.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
            }
        }
    }
}
