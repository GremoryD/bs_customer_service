using System;
using CLProject;
using WebSocketSharp;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using bcsapp.Controls;
using bcsapp.ViewModels;
using ServerLib.JTypes.Server;
using bcsapp.Models;
using System.Windows;
using System.Collections.Generic;

namespace bcsapp
{
    /// <summary>
    /// Класс приложения
    /// </summary>
    public class WebSocketController
    {
        public static WebSocketController Instance { get { if (StaticInstance == null) { StaticInstance = new WebSocketController(); } return StaticInstance; } }

        public ProjectClass Project;
        public WebSocket WebSocketClient;

        /// <summary>
        /// Поток подключения к WebSocket-серверу
        /// </summary>
        private Thread ConnectToWebSocketServerThread;

        /// <summary>
        /// Поток обработки входной очереди
        /// </summary>
        private Thread InputQueueProcessing;

        /// <summary>
        /// Поток обработки выходной очереди
        /// </summary>
        private Thread OutputQueueProcessing;

        /// <summary>
        /// Признак запуска приложения
        /// </summary>
        private bool IsStarting = false;

        /// <summary>
        /// Входная очередь сообщений 
        /// </summary>
        public ConcurrentQueue<string> InputQueue = new ConcurrentQueue<string>();

        /// <summary>
        /// Выходная очередь сообщений
        /// </summary>
        private ConcurrentQueue<string> OutputQueue = new ConcurrentQueue<string>();

        private static WebSocketController StaticInstance;

        public WebSocketController()
        {
            Project = new ProjectClass("BCSApp");
            WebSocketClient = new WebSocket(string.Format("ws://{0}:{1}/", Project.Settings.WebSocketServerAddress, Project.Settings.WebSocketServerPort));
            WebSocketClient.OnMessage += OnWebSocketMessage;
            ConnectToWebSocketServerThread = new Thread(ConnectToWebSocketServer);
            InputQueueProcessing = new Thread(InputQueueProcessingThread);
            OutputQueueProcessing = new Thread(OutputQueueProcessingThread);
        }

        /// <summary>
        /// Запуск приложения
        /// </summary>
        public void Start()
        {
            IsStarting = true;
            ConnectToWebSocketServerThread.Start();
            InputQueueProcessing.Start();
            OutputQueueProcessing.Start();
        }

        /// <summary>
        /// Остановка приложения
        /// </summary>
        public void Stop()
        {
            IsStarting = false;
            WebSocketClient.Close();
        }

        /// <summary>
        /// Поток подключения к WebSocket-серверу
        /// </summary>
        private void ConnectToWebSocketServer()
        {
            while (IsStarting)
            {
                if (!WebSocketClient.IsAlive)
                {
                    try
                    {
                        WebSocketClient.Connect();
                    }
                    catch { }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Содержит данные события OnMessage</param>
        private void OnWebSocketMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {
                InputQueue.Enqueue(e.Data);
            }
        }

        /// <summary>
        /// Поток обработчика входной очереди запросов
        /// </summary>
        private void InputQueueProcessingThread()
        {
            while (IsStarting)
            {
                if (InputQueue.TryDequeue(out string InputMessage))
                {
                    try
                    {
                        BaseResponseClass Message = JsonConvert.DeserializeObject<BaseResponseClass>(InputMessage);
                        if (Message.State == ServerLib.JTypes.Enums.ResponseState.ok)
                        {
                            switch (Message.Command)
                            {
                                case ServerLib.JTypes.Enums.Commands.login:
                                    LoginHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.user_information:
                                    UserInformationHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.users:
                                    UsersListHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.user_add:
                                    UpdateUsersHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.user_edit:
                                    UpdateUsersHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.jobs:
                                    JobssListHandler(InputMessage); 
                                    break;
                                case ServerLib.JTypes.Enums.Commands.job_add:
                                    UpdateJobssListHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.job_edit:
                                    UpdateJobssListHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.roles:
                                    RolesListHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.roles_add:
                                    UpdateRoleHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.roles_edit:
                                    UpdateRoleHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.users_roles:
                                    RolesUsersListHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.users_roles_add:
                                    UpdateUserRoleAddHandler(InputMessage);
                                    break;
                                case ServerLib.JTypes.Enums.Commands.users_roles_delete:
                                    UpdateUserRoleDeleteHandler(InputMessage);
                                    break;
                            }
                        }
                        else if (Message.State == ServerLib.JTypes.Enums.ResponseState.error)
                        {
                            switch (Message.Command)
                            {
                                case ServerLib.JTypes.Enums.Commands.login:
                                    switch (JsonConvert.DeserializeObject<ExceptionClass>(InputMessage).Code)
                                    {
                                        case ServerLib.JTypes.Enums.ErrorCodes.IncorrectLoginOrPassword:
                                            LoginFailedHandler();
                                            break;
                                        default:
                                            break;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            Project.Log.Error(string.Format("InputQueueProcessingThread: Unknown value filed \"state\":\"{0}\"", Message.State.ToString()));
                        }
                    }
                    catch (Exception ex)
                    {
                        Project.Log.Error(string.Format("InputQueueProcessingThread: {0}", ex.Message));
                    }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Добавляет строку в выходную очередь
        /// </summary>
        /// <param name="AString">Текст ответа</param>
        public void OutputQueueAddString(string AString)
        {
            OutputQueue.Enqueue(AString);
        }

        /// <summary>
        /// Добавляет объект в выходную очередь
        /// </summary>
        /// <param name="AObject">Объект с ответом</param>
        public void OutputQueueAddObject(object AObject)
        {
            OutputQueueAddString(JsonConvert.SerializeObject(AObject));
        }

        /// <summary>
        /// Поток передачи данных из выходной очереди в WebSocket-соединение
        /// </summary>
        private void OutputQueueProcessingThread()
        {
            while (IsStarting)
            {
                if (WebSocketClient.ReadyState == WebSocketState.Closed)
                { ServerErr?.Invoke(this, "Отсутствует подключение к серверу"); ConnectedState?.Invoke(this, "Отсутствует подключение"); }

                if (WebSocketClient.ReadyState == WebSocketState.Open) ConnectedState?.Invoke(this, "Соеденено");
                if (OutputQueue.TryDequeue(out string Message))
                {
                    try
                    {
                        WebSocketClient.Send(Message);
                    }
                    catch
                    {
                        ServerErr?.Invoke(this, "Отсутствует подключение к серверу");
                        ConnectedState?.Invoke(this, "Отсутствует подключение");
                    }
                }
                Thread.Sleep(1);
            }
        }

        #region События 

        // Функции вызываемые в LoginViewModel
        public event EventHandler<String> ServerErr;
        public event EventHandler<String> UpdateUserUI;
        public event EventHandler<String> ConnectedState;
        public event EventHandler<LoginClass> LoginDone;
        public event EventHandler<string> LoginFailed;

        //функция для инициализации списков
        public event EventHandler<List<UserClass>> UpdateUsers;
        public event EventHandler<List<JobClass>> UpdateJobs;
        public event EventHandler<List<RoleClass>> UpdateRoles;
        public event EventHandler<List<UserRoleClass>> UpdateUserRoles;
        //функции вызываемые при обновление одиночного елемента
        public event EventHandler<UserClass> UpdateUser;
        public event EventHandler<JobClass> UpdateJob;
        public event EventHandler<RoleClass> UpdateRole;
        public event EventHandler<UserRoleClass> UpdateUserRole; 
        #endregion

        #region Обработчики 

        //функция логина
        private void LoginHandler(string InputMessage)
        {
            DataStorage.Instance.Login = JsonConvert.DeserializeObject<LoginClass>(InputMessage);
            LoginDone?.Invoke(this, JsonConvert.DeserializeObject<LoginClass>(InputMessage));
        }
        //функция получения информации текущего пользователя
        private void UserInformationHandler(string InputMessage)
        {
            DataStorage.Instance.UserInformation = JsonConvert.DeserializeObject<UserInformationClass>(InputMessage);
            UpdateUserUI?.Invoke(this, String.Format("{0} {1} {2}", DataStorage.Instance.UserInformation.FirstName, DataStorage.Instance.UserInformation.MidleName, DataStorage.Instance.UserInformation.LastName));
        }

        //Функции для работы с пользователями
        private void UsersListHandler(string InputMessage)
        {
            if (DataStorage.Instance.UserList.Count==0)
            {
                DataStorage.Instance.UserList = JsonConvert.DeserializeObject<UsersClass>(InputMessage).Items;
                DataStorage.Instance.UserList.Remove(DataStorage.Instance.UserList.Find(x => x.ID == DataStorage.Instance.Login.ID));
                UpdateUsers?.Invoke(this, DataStorage.Instance.UserList);
            }
            else
            {
                //TODO 
                foreach(UserClass user in JsonConvert.DeserializeObject<UsersClass>(InputMessage).Items)
                {
                    UpdateUsersHandler(user);
                } 

                UpdateUsers?.Invoke(this, DataStorage.Instance.UserList);
            }
        }

        private void UpdateUsersHandler(string InputMessage)
        {
            if (DataStorage.Instance.UserList != null)
            {
               
                    UserEditClass temp = JsonConvert.DeserializeObject<UserEditClass>(InputMessage);
                    UserClass user = new UserClass() { ID = temp.ID,
                                                       Active = temp.Active,
                                                       FirstName = temp.FirstName,
                                                       LastName = temp.LastName,
                                                       MidleName = temp.MidleName,
                                                       JobID = temp.JobId,
                                                       JobName = temp.JobName };

                    UserClass result = DataStorage.Instance.UserList.Find(x => x.ID == user.ID);
                    if (result!=null)
                    { 
                            DataStorage.Instance.UserList.RemoveAt(DataStorage.Instance.UserList.IndexOf(result));                      
                    } 
                    DataStorage.Instance.UserList.Add(user); 
                    UpdateUser?.Invoke(this, user);
            } 
        }

        private void UpdateUsersHandler(UserClass InputUser)
        {
            if (DataStorage.Instance.UserList != null)
            {   
                UserClass result = DataStorage.Instance.UserList.Find(x => x.ID == InputUser.ID);
                if (result != null)
                {
                    DataStorage.Instance.UserList.RemoveAt(DataStorage.Instance.UserList.IndexOf(result));
                }
                DataStorage.Instance.UserList.Add(InputUser);
                UpdateUser?.Invoke(this, InputUser);
            }
            else
            {

            }
        }

        //функции для работы с професиями
        private void JobssListHandler(string InputMessage)
        {
            if (DataStorage.Instance.JobList != null)
            {
                DataStorage.Instance.JobList = JsonConvert.DeserializeObject<JobsClass>(InputMessage).Jobs;

            }
            else
            {
                foreach (JobClass job in JsonConvert.DeserializeObject<JobsClass>(InputMessage).Jobs)
                {
                    UpdateJobssListHandler(job);
                }

                UpdateUsers?.Invoke(this, DataStorage.Instance.UserList);
            }
            UpdateJobs?.Invoke(this, DataStorage.Instance.JobList);
        } 
         
        private void UpdateJobssListHandler(string InputMessage)
        {
            if (DataStorage.Instance.UserList != null)
            {

                JobAddClass temp = JsonConvert.DeserializeObject<JobAddClass>(InputMessage);
                JobClass jobAdd = new JobClass()
                {
                    ID = temp.ID,
                    Name = temp.Name
                };

                JobClass result = DataStorage.Instance.JobList.Find(x => x.ID == jobAdd.ID);
                if (result != null)
                {
                    DataStorage.Instance.JobList.RemoveAt(DataStorage.Instance.JobList.IndexOf(result));
                }
                DataStorage.Instance.JobList.Add(jobAdd);
                UpdateJob?.Invoke(this, jobAdd);
            }
            
        }



        private void UpdateJobssListHandler(JobClass InputMessage)
        {
            if (DataStorage.Instance.UserList != null)
            {
                 
                JobClass jobAdd = new JobClass()
                {
                    ID = InputMessage.ID,
                    Name = InputMessage.Name
                };

                JobClass result = DataStorage.Instance.JobList.Find(x => x.ID == jobAdd.ID);
                if (result != null)
                {
                    DataStorage.Instance.JobList.RemoveAt(DataStorage.Instance.JobList.IndexOf(result));
                }
                DataStorage.Instance.JobList.Add(jobAdd);
                UpdateJob?.Invoke(this, jobAdd);
            }

        }



        //Функции для работы с ролями
        private void RolesListHandler(string InputMessage)
        {
            if (DataStorage.Instance.RoleList.Count ==  0)
            {
                DataStorage.Instance.RoleList = JsonConvert.DeserializeObject<RolesClass>(InputMessage).Items;

            }
            else
            {
                //TODO 
                foreach (RoleClass role in JsonConvert.DeserializeObject<RolesClass>(InputMessage).Items)
                {
                    UpdateRoleHandler(role);
                } 

            }
            UpdateRoles?.Invoke(this, DataStorage.Instance.RoleList);
        }

        private void UpdateRoleHandler(string InputMessage)
        {
            if (DataStorage.Instance.RoleList != null)
            {

                RoleEditClass temp = JsonConvert.DeserializeObject<RoleEditClass>(InputMessage);
                RoleClass role = new RoleClass()
                {
                     ID = temp.ID,
                     Name = temp.Name,
                     Description = temp.Description 
                    
                    
                };

                RoleClass result = DataStorage.Instance.RoleList.Find(x => x.ID == role.ID);
                if (result != null)
                {
                    DataStorage.Instance.RoleList.RemoveAt(DataStorage.Instance.RoleList.IndexOf(result));
                }
                DataStorage.Instance.RoleList.Add(role);
                UpdateRole?.Invoke(this, role);
            } 
        }

        private void UpdateRoleHandler(RoleClass InputRole)
        {
            if (DataStorage.Instance.RoleList != null)
            {

                RoleClass result = DataStorage.Instance.RoleList.Find(x => x.ID == InputRole.ID);
                if (result != null)
                {
                    DataStorage.Instance.RoleList.RemoveAt(DataStorage.Instance.RoleList.IndexOf(result));
                }
                DataStorage.Instance.RoleList.Add(InputRole);
                UpdateRole?.Invoke(this, InputRole);
            }
            else
            {

            }
        }

        //функция получения ролей пользователей 
        private void RolesUsersListHandler(string InputMessage)
        {
            if (DataStorage.Instance.UsersRolesList.Count == 0)
            {
                DataStorage.Instance.UsersRolesList = JsonConvert.DeserializeObject<UsersRolesClass>(InputMessage).Items;

            }
            else
            {
                //TODO 
                foreach (UserRoleClass roleUser in JsonConvert.DeserializeObject<UsersRolesClass>(InputMessage).Items)
                {
                    UpdateUserRoleHandler(roleUser);
                }

            }
            UpdateUserRoles?.Invoke(this, DataStorage.Instance.UsersRolesList);

        }

        private void UpdateUserRoleAddHandler(string InputMessage)
        { 
            if (DataStorage.Instance.RoleList != null)
            {

                UserRoleAddClass temp = JsonConvert.DeserializeObject<UserRoleAddClass>(InputMessage);
                UserRoleClass role = new UserRoleClass()
                {
                    ID = temp.ID,
                    RoleID = temp.RoleID,
                    UserID = temp.UserID,
                    RoleName = DataStorage.Instance.RoleList.Find(x => x.ID == temp.RoleID).Name
                };

                UserRoleClass result = DataStorage.Instance.UsersRolesList.Find(x => x.ID == role.ID);
                if (result != null)
                {
                    DataStorage.Instance.UsersRolesList.RemoveAt(DataStorage.Instance.UsersRolesList.IndexOf(result));
                }
                DataStorage.Instance.UsersRolesList.Add(role);
                UpdateUserRole?.Invoke(this, role);
            }
        }

        private void UpdateUserRoleDeleteHandler(string InputMessage)
        { 
            if (DataStorage.Instance.RoleList != null)
            {

                UserRoleDeleteClass temp = JsonConvert.DeserializeObject<UserRoleDeleteClass>(InputMessage);
                 
            }
        }

        private void UpdateUserRoleHandler(UserRoleClass InputRole)
        {
            if (DataStorage.Instance.RoleList != null)
            {

                UserRoleClass role = new UserRoleClass()
                {
                    ID = InputRole.ID,
                    RoleID = InputRole.RoleID,
                    UserID = InputRole.UserID,
                    RoleName = DataStorage.Instance.RoleList.Find(x => x.ID == InputRole.RoleID).Name 
                };

                UserRoleClass result = DataStorage.Instance.UsersRolesList.Find(x => x.ID == role.ID);
                if (result != null)
                {
                    DataStorage.Instance.UsersRolesList.RemoveAt(DataStorage.Instance.UsersRolesList.IndexOf(result));
                }
                DataStorage.Instance.UsersRolesList.Add(role);
                UpdateUserRole?.Invoke(this, role);
            }
            else
            {

            }
        }



        #endregion

        #region Обработчики ошибок

        private void LoginFailedHandler()
        {
            LoginFailed?.Invoke(this, "Неправильный логин или пароль");
        }

        #endregion
    }
}