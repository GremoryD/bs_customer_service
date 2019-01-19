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
    public class JobsClass : BaseHandlerClass
    {
        /// <summary>
        /// Словарь должностей пользователей
        /// </summary>
        private readonly ConcurrentDictionary<long, ServerLib.JTypes.Server.JobClass> JobsCollection;

        public JobsClass(UserSessionClass AUserSession) : base(AUserSession)
        {
            JobsCollection = new ConcurrentDictionary<long, ServerLib.JTypes.Server.JobClass>();
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public override void RefreshData()
        {
            ServerLib.JTypes.Server.JobsClass JobsList = new ServerLib.JTypes.Server.JobsClass();
            DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
            Params.CreateParameterValue("Token", UserSession.Login.Token);
            DatabaseTableClass JobsTable = new DatabaseTableClass
            {
                Table = (System.Data.DataTable)UserSession.Project.Database.Execute("Jobs", ref Params)
            };

            foreach (System.Data.DataRow row in JobsTable.Table.Rows)
            {
                ServerLib.JTypes.Server.JobClass Job = new ServerLib.JTypes.Server.JobClass
                {
                    ID = JobsTable.AsInt64(row, "ID"),
                    Name = JobsTable.AsString(row, "JOB_NAME")
                };

                if (JobsCollection.TryGetValue(Job.ID, out ServerLib.JTypes.Server.JobClass ExistJob))
                {
                    if (ExistJob.Hash != Job.Hash)
                    {
                        Job.Command = ListCommands.edit;
                        JobsCollection.TryUpdate(Job.ID, Job, ExistJob);
                        JobsList.Jobs.Add(Job);
                    }
                }
                else
                {
                    Job.Command = ListCommands.add;
                    JobsCollection.TryAdd(Job.ID, Job);
                    JobsList.Jobs.Add(Job);
                }
            }

            foreach (var User in JobsCollection)
            {
                bool IsExist = false;

                foreach (System.Data.DataRow row in JobsTable.Table.Rows)
                {
                    if (User.Value.ID == JobsTable.AsInt64(row, "ID"))
                    {
                        IsExist = true;
                        break;
                    }
                }

                if (!IsExist)
                {
                    User.Value.Command = ListCommands.delete;
                    JobsList.Jobs.Add(User.Value);
                    JobsCollection.TryRemove(User.Value.ID, out ServerLib.JTypes.Server.JobClass DeletingJob);
                }
            }

            if (JobsList.Jobs.Count > 0)
            {
                UserSession.OutputQueueAddObject(JobsList);
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
                ServerLib.JTypes.Client.JobAddClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.JobAddClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("JobName", Request.Name);
                Params.CreateParameterValue("NewId");
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("JobAdd", ref Params);
                if (Params.ParameterByName("State").Value.ToString() == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.JobAddClass
                    {
                        ID = Convert.ToInt64(Params.ParameterByName("NewId").Value.ToString()),
                        Name = Request.Name.Trim()
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.job_add, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.job_add, ErrorCodes.FatalError, ex.Message));
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
                ServerLib.JTypes.Client.JobEditClass Request = JsonConvert.DeserializeObject<ServerLib.JTypes.Client.JobEditClass>(ARequest);
                DatabaseParameterValuesClass Params = new DatabaseParameterValuesClass();
                Params.CreateParameterValue("Token", Request.Token);
                Params.CreateParameterValue("JobName", Request.Name);
                Params.CreateParameterValue("JobId", Request.ID);
                Params.CreateParameterValue("State");
                Params.CreateParameterValue("ErrorText");
                UserSession.Project.Database.Execute("JobEdit", ref Params);
                if (Params.ParameterByName("State").Value.ToString() == "ok")
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.JobEditClass
                    {
                        ID = Request.ID,
                        Name = Request.Name.Trim()
                    });
                    ProcessingSuccess = true;
                }
                else
                {
                    UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.job_edit, ErrorCodes.DatabaseError, Params.ParameterByName("ErrorText").Value.ToString()));
                }
            }
            catch (Exception ex)
            {
                UserSession.OutputQueueAddObject(new ServerLib.JTypes.Server.ExceptionClass(Commands.job_edit, ErrorCodes.FatalError, ex.Message));
            }
            return ProcessingSuccess;
        }
    }
}
