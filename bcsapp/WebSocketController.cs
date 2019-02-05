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
                        ResponseBaseClass Message = JsonConvert.DeserializeObject<ResponseBaseClass>(InputMessage);
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
                                    switch (JsonConvert.DeserializeObject<ResponseExceptionClass>(InputMessage).Code)
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
        public event EventHandler<ResponseLoginClass> LoginDone;
        public event EventHandler<string> LoginFailed;

        //функция для инициализации списков
        public event EventHandler<List<ResponseUserClass>> UpdateUsers;
        public event EventHandler<List<ResponseJobClass>> UpdateJobs;
        public event EventHandler<List<ResponseRoleClass>> UpdateRoles;
        public event EventHandler<List<ResponseUserRoleClass>> UpdateUserRoles;
        //функции вызываемые при обновление одиночного елемента
        public event EventHandler<ResponseUserClass> UpdateUser;
        public event EventHandler<ResponseJobClass> UpdateJob;
        public event EventHandler<ResponseRoleClass> UpdateRole;
        public event EventHandler<ResponseUserRoleClass> UpdateUserRole; 
        #endregion

        #region Обработчики 

        //функция логина
        private void LoginHandler(string InputMessage)
        {
            DataStorage.Instance.Login = JsonConvert.DeserializeObject<ResponseLoginClass>(InputMessage);
            LoginDone?.Invoke(this, JsonConvert.DeserializeObject<ResponseLoginClass>(InputMessage));
        }
        //функция получения информации текущего пользователя
        private void UserInformationHandler(string InputMessage)
        {
            DataStorage.Instance.UserInformation = JsonConvert.DeserializeObject<ResponseUserInformationClass>(InputMessage);
            UpdateUserUI?.Invoke(this, String.Format("{0} {1} {2}", DataStorage.Instance.UserInformation.FirstName, DataStorage.Instance.UserInformation.MidleName, DataStorage.Instance.UserInformation.LastName));
        }

        //Функции для работы с пользователями
        private void UsersListHandler(string InputMessage)
        {
            if (DataStorage.Instance.UserList.Count==0)
            {
                DataStorage.Instance.UserList = JsonConvert.DeserializeObject<ResponseUsersClass>(InputMessage).Items;
                DataStorage.Instance.UserList.Remove(DataStorage.Instance.UserList.Find(x => x.ID == DataStorage.Instance.Login.ID));
                UpdateUsers?.Invoke(this, DataStorage.Instance.UserList);
            }
            else
            {
                //TODO 
                foreach(ResponseUserClass user in JsonConvert.DeserializeObject<ResponseUsersClass>(InputMessage).Items)
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
               
                    ResponseUserEditClass temp = JsonConvert.DeserializeObject<ResponseUserEditClass>(InputMessage);
                    ResponseUserClass user = new ResponseUserClass() { ID = temp.ID,
                                                       Active = temp.Active,
                                                       FirstName = temp.FirstName,
                                                       LastName = temp.LastName,
                                                       MidleName = temp.MidleName,
                                                       JobID = temp.JobId,
                                                       JobName = temp.JobName };

                    ResponseUserClass result = DataStorage.Instance.UserList.Find(x => x.ID == user.ID);
                    if (result!=null)
                    { 
                            DataStorage.Instance.UserList.RemoveAt(DataStorage.Instance.UserList.IndexOf(result));                      
                    } 
                    DataStorage.Instance.UserList.Add(user); 
                    UpdateUser?.Invoke(this, user);
            } 
        }

        private void UpdateUsersHandler(ResponseUserClass InputUser)
        {
            if (DataStorage.Instance.UserList != null)
            {   
                ResponseUserClass result = DataStorage.Instance.UserList.Find(x => x.ID == InputUser.ID);
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
                DataStorage.Instance.JobList = JsonConvert.DeserializeObject<ResponseJobsClass>(InputMessage).Jobs;

            }
            else
            {
                foreach (ResponseJobClass job in JsonConvert.DeserializeObject<ResponseJobsClass>(InputMessage).Jobs)
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

                ResponseJobAddClass temp = JsonConvert.DeserializeObject<ResponseJobAddClass>(InputMessage);
                ResponseJobClass jobAdd = new ResponseJobClass()
                {
                    ID = temp.ID,
                    Name = temp.Name
                };

                ResponseJobClass result = DataStorage.Instance.JobList.Find(x => x.ID == jobAdd.ID);
                if (result != null)
                {
                    DataStorage.Instance.JobList.RemoveAt(DataStorage.Instance.JobList.IndexOf(result));
                }
                DataStorage.Instance.JobList.Add(jobAdd);
                UpdateJob?.Invoke(this, jobAdd);
            }
            
        }



        private void UpdateJobssListHandler(ResponseJobClass InputMessage)
        {
            if (DataStorage.Instance.UserList != null)
            {
                 
                ResponseJobClass jobAdd = new ResponseJobClass()
                {
                    ID = InputMessage.ID,
                    Name = InputMessage.Name
                };

                ResponseJobClass result = DataStorage.Instance.JobList.Find(x => x.ID == jobAdd.ID);
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
                DataStorage.Instance.RoleList = JsonConvert.DeserializeObject<ResponseRolesClass>(InputMessage).Items;

            }
            else
            {
                //TODO 
                foreach (ResponseRoleClass role in JsonConvert.DeserializeObject<ResponseRolesClass>(InputMessage).Items)
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

                ResponseRoleEditClass temp = JsonConvert.DeserializeObject<ResponseRoleEditClass>(InputMessage);
                ResponseRoleClass role = new ResponseRoleClass()
                {
                     ID = temp.ID,
                     Name = temp.Name,
                     Description = temp.Description 
                    
                    
                };

                ResponseRoleClass result = DataStorage.Instance.RoleList.Find(x => x.ID == role.ID);
                if (result != null)
                {
                    DataStorage.Instance.RoleList.RemoveAt(DataStorage.Instance.RoleList.IndexOf(result));
                }
                DataStorage.Instance.RoleList.Add(role);
                UpdateRole?.Invoke(this, role);
            } 
        }

        private void UpdateRoleHandler(ResponseRoleClass InputRole)
        {
            if (DataStorage.Instance.RoleList != null)
            {

                ResponseRoleClass result = DataStorage.Instance.RoleList.Find(x => x.ID == InputRole.ID);
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
                DataStorage.Instance.UsersRolesList = JsonConvert.DeserializeObject<ResponseUsersRolesClass>(InputMessage).Items;

            }
            else
            {
                //TODO 
                foreach (ResponseUserRoleClass roleUser in JsonConvert.DeserializeObject<ResponseUsersRolesClass>(InputMessage).Items)
                {

                }

            }
            UpdateUserRoles?.Invoke(this, DataStorage.Instance.UsersRolesList);

        }

        private void UpdateUserRoleAddHandler(string InputMessage)
        { 
            if (DataStorage.Instance.RoleList != null)
            {

                ResponseUserRoleAddClass temp = JsonConvert.DeserializeObject<ResponseUserRoleAddClass>(InputMessage);
                ResponseUserRoleClass role = new ResponseUserRoleClass()
                {
                    ID = temp.ID,
                    RoleID = temp.RoleID,
                    UserID = temp.UserID,
                    RoleName = DataStorage.Instance.RoleList.Find(x => x.ID == temp.RoleID).Name
                };

                ResponseUserRoleClass result = DataStorage.Instance.UsersRolesList.Find(x => x.ID == role.ID);
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

                ResponseUserRoleDeleteClass temp = JsonConvert.DeserializeObject<ResponseUserRoleDeleteClass>(InputMessage);
                 
            }
        }

        private void UpdateUserRoleHandler(ResponseUserRoleClass InputRole)
        {
            if (DataStorage.Instance.RoleList != null)
            {

                ResponseUserRoleClass role = new ResponseUserRoleClass()
                {
                    ID = InputRole.ID,
                    RoleID = InputRole.RoleID,
                    UserID = InputRole.UserID,
                    RoleName = DataStorage.Instance.RoleList.Find(x => x.ID == InputRole.RoleID).Name 
                };

                ResponseUserRoleClass result = DataStorage.Instance.UsersRolesList.Find(x => x.ID == role.ID);
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