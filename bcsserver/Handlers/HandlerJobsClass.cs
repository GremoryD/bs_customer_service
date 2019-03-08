using System;
using CLProject;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using ServerLib.JTypes.Enums;

namespace bcsserver.Handlers
{
    /// <summary>
    /// Должности пользователей
    /// </summary>
    public class HandlerJobsClass : HandlerBaseClass
    {
        /// <summary>
        /// Словарь должностей пользователей
        /// </summary>
        private readonly ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseJobClass> JobsCollection;

        public HandlerJobsClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            JobsCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.ResponseJobClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.ResponseJobsClass OutputList = new ServerLib.JTypes.Server.ResponseJobsClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass ReadTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("Jobs", ref Params)
            };

            foreach (System.Data.DataRow row in ReadTable.Table.Rows)
            {
                ServerLib.JTypes.Server.ResponseJobClass Item = new ServerLib.JTypes.Server.ResponseJobClass
                {
                    ID = ReadTable.AsInt64(row, "ID"),
                    Name = ReadTable.AsString(row, "JOB_NAME")
                };

                if (JobsCollection.TryGetValue(Item.ID, out ServerLib.JTypes.Server.ResponseJobClass ExistItem))
                {
                    if (ExistItem.Hash != Item.Hash)
                    {
                        Item.Command = ItemCommands.edit;
                        JobsCollection.TryUpdate(Item.ID, Item, ExistItem);
                        OutputList.Jobs.Add(Item);
                    }
                }
                else
                {
                    Item.Command = ItemCommands.add;
                    JobsCollection.TryAdd(Item.ID, Item);
                    OutputList.Jobs.Add(Item);
                }
            }

            foreach (System.Collections.Generic.KeyValuePair<long, ServerLib.JTypes.Server.ResponseJobClass> Item in JobsCollection)
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
                    OutputList.Jobs.Add(Item.Value);
                    JobsCollection.TryRemove(Item.Value.ID, out ServerLib.JTypes.Server.ResponseJobClass DeletingItem);
                }
            }

            if (OutputList.Jobs.Count > 0)
            {
                UserSession.OutputQueueAddObject(OutputList);
            }
        }

        /// <summary>
        /// Обработчик добавления должности пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool AddProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.RequestJobAddClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestJobAddClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("JobName", Request.Name.Trim());
                Params.CreateParameterValue("NewId");
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("JobAdd", ref Params);
                if (Params.ParameterByName("State").AsString== "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseJobAddClass
                    {
                        ID = Params.ParameterByName("NewId").AsInt64,
                        Name = Request.Name.Trim()
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.job_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.job_add, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }

        /// <summary>
        /// Обработчик изменения должности пользователей
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public override bool EditProcessing(string ARequest)
        {
            bool ProcessingSuccess = false;
            try
            {
                ServerLib.JTypes.Client.RequestJobEditClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.RequestJobEditClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("JobName", Request.Name.Trim());
                Params.CreateParameterValue("JobId", Request.ID);
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("JobEdit", ref Params);
                if (Params.ParameterByName("State").AsString== "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseJobEditClass
                    {
                        ID = Request.ID,
                        Name = Request.Name.Trim()
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.job_edit, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").AsString));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ResponseExceptionClass(Commands.job_edit, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }
    }
}
