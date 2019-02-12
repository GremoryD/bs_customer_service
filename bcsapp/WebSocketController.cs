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
                                    LoginHandler(InputMessage);//"{\"token\":\"4hg62w+7GrRlTOL+CXD332a6TAQrP5zULdeHwRp4d70=\",\"id\":1,\"active\":\"activated\",\"command\":\"login\",\"state\":\"ok\"}"
                                    break;
                                case ServerLib.JTypes.Enums.Commands.user_information:
                                    UserInformationHandler(InputMessage);//"{\"first_name\":\"Super\",\"last_name\":\"Puper\",\"midle_name\":\"Admin\",\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"command\":\"user_information\",\"state\":\"ok\"}"
                                    break;                               //"{\"first_name\":\"Super\",\"last_name\":\"Puper\",\"midle_name\":\"Admin\",\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"command\":\"user_information\",\"state\":\"ok\"}"
                                case ServerLib.JTypes.Enums.Commands.users:
                                    UsersListHandler(InputMessage);//"{\"users\":[{\"login\":\"admin\",\"first_name\":\"Super\",\"last_name\":\"Puper\",\"midle_name\":\"Admin\",\"job_id\":1,\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"id\":1,\"command\":\"add\"},{\"login\":\"Тест\",\"first_name\":\"Тест\",\"last_name\":\"Тест\",\"midle_name\":\"Тест\",\"job_id\":1,\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"id\":4,\"command\":\"add\"},{\"login\":\"new1\",\"first_name\":\"Новый\",\"last_name\":\"Пользователь\",\"midle_name\":\"ТЕСТ\",\"job_id\":1,\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"id\":3,\"command\":\"add\"},{\"login\":\"TestBan\",\"first_name\":\"Заблокированный\",\"last_name\":\"Пользователь\",\"midle_name\":\"ТЕСТ\",\"job_id\":1,\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"id\":2,\"command\":\"add\"}],\"command\":\"users\",\"state\":\"ok\"}"
                                    break;
                                case ServerLib.JTypes.Enums.Commands.user_add:
                                    UserAddConfirmationHandler(InputMessage);//"{\"id\":6,\"login\":\"Тест2\",\"first_name\":\"Тест2\",\"last_name\":\"Тест2\",\"midle_name\":\"Тест2\",\"job_id\":2,\"job_name\":\"Тест\",\"active\":\"activated\",\"command\":\"user_add\",\"state\":\"ok\"}"
                                    break;                           //"{\"users\":[{\"login\":\"Тест2\",\"first_name\":\"Тест2\",\"last_name\":\"Тест2\",\"midle_name\":\"Тест2\",\"job_id\":2,\"job_name\":\"Тест\",\"active\":\"activated\",\"id\":6,\"command\":\"add\"}],\"command\":\"users\",\"state\":\"ok\"}" 
                                case ServerLib.JTypes.Enums.Commands.user_edit:
                                    UserEditConfirmationHandler(InputMessage);//"{\"id\":4,\"first_name\":\"Тест1\",\"last_name\":\"Тест1\",\"midle_name\":\"Тест1\",\"job_id\":1,\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"command\":\"user_edit\",\"state\":\"ok\"}"
                                    break;                           //"{\"users\":[{\"login\":\"Тест\",\"first_name\":\"Тест2\",\"last_name\":\"Тест2\",\"midle_name\":\"Тест2\",\"job_id\":1,\"job_name\":\"Главный администратор\",\"active\":\"activated\",\"id\":4,\"command\":\"edit\"}],\"command\":\"users\",\"state\":\"ok\"}"
                                case ServerLib.JTypes.Enums.Commands.jobs:
                                    JobssListHandler(InputMessage); //"{\"jobs\":[{\"name\":\"Главный администратор\",\"id\":1,\"command\":\"add\"}],\"command\":\"jobs\",\"state\":\"ok\"}"
                                    break;
                                case ServerLib.JTypes.Enums.Commands.job_add:
                                    JobAddConfirmationListHandler(InputMessage);//"{\"id\":2,\"name\":\"Тест\",\"command\":\"job_add\",\"state\":\"ok\"}"
                                    break;                              //"{\"jobs\":[{\"name\":\"Тест\",\"id\":2,\"command\":\"add\"}],\"command\":\"jobs\",\"state\":\"ok\"}"
                                case ServerLib.JTypes.Enums.Commands.job_edit:
                                    JobEditConfirmationHandler(InputMessage); //"{\"id\":0,\"name\":\"Тест2\",\"command\":\"job_edit\",\"state\":\"ok\"}"
                                    break;
                                case ServerLib.JTypes.Enums.Commands.roles:
                                    RolesListHandler(InputMessage);//"{\"roles\":[{\"name\":\"Администратор\",\"description\":\"Пользователи с неограниченными правами доступа\",\"id\":1,\"command\":\"add\"},{\"name\":\"Тестовая роль\",\"id\":2,\"command\":\"add\"}],\"command\":\"roles\",\"state\":\"ok\"}"
                                    break;
                                case ServerLib.JTypes.Enums.Commands.roles_add:
                                    RoleAddConfirmationHandler(InputMessage);//"{\"id\":3,\"name\":\"Тест1\",\"description\":\"Тестовая роль\",\"command\":\"roles_add\",\"state\":\"ok\"}"
                                    break;                          //"{\"roles\":[{\"name\":\"Тест1\",\"description\":\"Тестовая роль\",\"id\":3,\"command\":\"add\"}],\"command\":\"roles\",\"state\":\"ok\"}"
                                case ServerLib.JTypes.Enums.Commands.roles_edit:
                                    RoleEditConfirmationHandler(InputMessage);//"{\"id\":3,\"name\":\"Тест2\",\"description\":\"Тестовая роль\",\"command\":\"roles_edit\",\"state\":\"ok\"}"
                                    break;                          //"{\"roles\":[{\"name\":\"Тест2\",\"description\":\"Тестовая роль\",\"id\":3,\"command\":\"edit\"}],\"command\":\"roles\",\"state\":\"ok\"}"
                                case ServerLib.JTypes.Enums.Commands.users_roles:
                                    RolesUsersListHandler(InputMessage);//"{\"users_roles\":[{\"user_id\":1,\"role_id\":1,\"role_name\":\"Администратор\",\"id\":4,\"command\":\"add\"},{\"user_id\":3,\"role_id\":1,\"role_name\":\"Администратор\",\"id\":3,\"command\":\"add\"},{\"user_id\":3,\"role_id\":2,\"role_name\":\"Тестовая роль\",\"id\":31,\"command\":\"add\"}],\"command\":\"users_roles\",\"state\":\"ok\"}"
                                    break;
                                case ServerLib.JTypes.Enums.Commands.users_roles_add:
                                    UserRoleAddConfirmationHandler(InputMessage); //"{\"id\":34,\"user_id\":8,\"role_id\":3,\"command\":\"users_roles_add\",\"state\":\"ok\"}"
                                                                                  //"{\"users_roles\":[{\"user_id\":8,\"role_id\":3,\"role_name\":\"Тест2\",\"id\":34,\"command\":\"add\"}],\"command\":\"users_roles\",\"state\":\"ok\"}"
                                    break;
                                case ServerLib.JTypes.Enums.Commands.users_roles_delete:
                                    UserRoleRemoveConfirmationHandler(InputMessage);//"{\"command\":\"users_roles_delete\",\"state\":\"ok\"}"
                                                                                    //"{\"users_roles\":[{\"user_id\":8,\"role_id\":3,\"role_name\":\"Тест2\",\"id\":34,\"command\":\"delete\"}],\"command\":\"users_roles\",\"state\":\"ok\"}"
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
        private bool ReconectState=false;
        private void OutputQueueProcessingThread()
        {
            while (IsStarting)
            {
                if (WebSocketClient.ReadyState == WebSocketState.Closed)
                {
                    ServerErr?.Invoke(this, "Отсутствует подключение к серверу");
                    ConnectedState?.Invoke(this, "Отсутствует подключение");
                    ReconectState = true;
                }

                if (WebSocketClient.ReadyState == WebSocketState.Open)
                { 
                    ConnectedState?.Invoke(this, "Соеденено"); ReconectState = false;
                }  
                if(OutputQueue.IsEmpty && ReconectState)
                {
                    ReconectToken(); ReconectState = false;
                    Thread.Sleep(3000);
                }



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


        private void ReconectToken()
        { 
           OutputQueueAddObject(Models.DataStorage.Instance.LoginIN); 
        }


        #region События 

        // Функции вызываемые в LoginViewModel
        public event EventHandler<String> ServerErr;
        public event EventHandler<String> UpdateUserUI;
        public event EventHandler<String> ConnectedState;
        public event EventHandler<ResponseLoginClass> LoginDone;
        public event EventHandler<string> LoginFailed;

        //функция для инициализации списков
        public event EventHandler<String> UpdateUsers;
        public event EventHandler<String> UpdateJobs;
        public event EventHandler<String> UpdateRoles;
        public event EventHandler<String> UpdateUserRoles;

        //функции для подтверждения действий
        public event EventHandler<String> UpdateUserAdd;
        public event EventHandler<String> UpdateUserEdit;
        public event EventHandler<String> UpdateUserAdd_Err;
        public event EventHandler<String> UpdateUserEdit_Err;
        public event EventHandler<String> UpdateJobAdd;
        public event EventHandler<String> UpdateJobEdit;
        public event EventHandler<String> UpdateJobAdd_Err;
        public event EventHandler<String> UpdateJobEdit_Err;
        public event EventHandler<String> UpdateRoleAdd;
        public event EventHandler<String> UpdateRoleEdit;
        public event EventHandler<String> UpdateRoleAdd_Err;
        public event EventHandler<String> UpdateRoleEdit_Err;
        public event EventHandler<String> UpdateRoleDelete;
        public event EventHandler<String> UpdateUserRoleAdd;
        public event EventHandler<String> UpdateUserRoleRemove;
        public event EventHandler<String> UpdateUserRoleAdd_Err;
        public event EventHandler<String> UpdateUserRoleRemove_Err;
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
               
            }
            else
            { 
                foreach(ResponseUserClass user in JsonConvert.DeserializeObject<ResponseUsersClass>(InputMessage).Items)
                {
                    if (user.Command == ServerLib.JTypes.Enums.ItemCommands.add) DataStorage.Instance.UserList.Add(user);
                    if (user.Command == ServerLib.JTypes.Enums.ItemCommands.delete) DataStorage.Instance.UserList.Remove(user);
                    if (user.Command == ServerLib.JTypes.Enums.ItemCommands.edit)
                    {
                        ResponseUserClass temp = DataStorage.Instance.UserList.Find(x => x.ID == user.ID);
                        int index = DataStorage.Instance.UserList.IndexOf(temp);
                        DataStorage.Instance.UserList[index].FirstName = user.FirstName;
                        DataStorage.Instance.UserList[index].MidleName = user.MidleName;
                        DataStorage.Instance.UserList[index].LastName = user.LastName;
                        DataStorage.Instance.UserList[index].JobID = user.JobID;
                        DataStorage.Instance.UserList[index].JobName = user.JobName;
                        DataStorage.Instance.UserList[index].Login = user.Login;
                        DataStorage.Instance.UserList[index].Active = user.Active; 
                    }
                }
            }
            UpdateUsers?.Invoke(this, "");
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
                foreach (ResponseJobClass jobs in JsonConvert.DeserializeObject<ResponseJobsClass>(InputMessage).Jobs)
                {
                    if (jobs.Command ==  ServerLib.JTypes.Enums.ItemCommands.add ) DataStorage.Instance.JobList.Add(jobs);
                    if (jobs.Command == ServerLib.JTypes.Enums.ItemCommands.delete) DataStorage.Instance.JobList.Remove(jobs);
                    if (jobs.Command == ServerLib.JTypes.Enums.ItemCommands.edit)
                    {
                        ResponseJobClass temp = DataStorage.Instance.JobList.Find(x => x.ID == jobs.ID);
                        int index = DataStorage.Instance.JobList.IndexOf(temp);
                        DataStorage.Instance.JobList[index] = jobs; 

                    }
                } 
            }
            UpdateJobs?.Invoke(this, "");
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

                foreach (ResponseRoleClass role in JsonConvert.DeserializeObject<ResponseRolesClass>(InputMessage).Items)
                {
                    if (role.Command == ServerLib.JTypes.Enums.ItemCommands.add) DataStorage.Instance.RoleList.Add(role);
                    if (role.Command == ServerLib.JTypes.Enums.ItemCommands.delete) DataStorage.Instance.RoleList.Remove(role);
                    if (role.Command == ServerLib.JTypes.Enums.ItemCommands.edit)
                    {
                        ResponseRoleClass temp = DataStorage.Instance.RoleList.Find(x => x.ID == role.ID);
                        int index = DataStorage.Instance.RoleList.IndexOf(temp);
                        DataStorage.Instance.RoleList[index] = role; 

                    }

                } 

            }
            UpdateRoles?.Invoke(this,"");
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
                foreach (ResponseUserRoleClass role in JsonConvert.DeserializeObject<ResponseUsersRolesClass>(InputMessage).Items)
                {
                    if (role.Command == ServerLib.JTypes.Enums.ItemCommands.add) DataStorage.Instance.UsersRolesList.Add(role);
                    if (role.Command == ServerLib.JTypes.Enums.ItemCommands.delete)
                    { 
                         ResponseUserRoleClass temp = DataStorage.Instance.UsersRolesList.Find(x => x.Hash == role.Hash);
                         DataStorage.Instance.UsersRolesList.Remove(temp);
                  
                    }
                    if (role.Command == ServerLib.JTypes.Enums.ItemCommands.edit)
                    {
                        ResponseUserRoleClass temp = DataStorage.Instance.UsersRolesList.Find(x=>x.ID == role.ID);
                        DataStorage.Instance.UsersRolesList.Remove(temp);
                        DataStorage.Instance.UsersRolesList.Add(role);
                    } 
                }

            }
            UpdateUserRoles?.Invoke(this,""); 
        }

        public void UserRoleRemoveConfirmationHandler(string inputMessage)
        {
            UpdateUserRoleRemove?.Invoke(this, "");
        }

        public void UserRoleAddConfirmationHandler(string inputMessage)
        {
            UpdateUserRoleAdd?.Invoke(this, "");
        }

        public void RoleEditConfirmationHandler(string inputMessage)
        {
            UpdateRoleEdit?.Invoke(this, "");
        }

        public void RoleAddConfirmationHandler(string inputMessage)
        {
            UpdateRoleAdd?.Invoke(this, "");
        }

        public void JobEditConfirmationHandler(string inputMessage)
        {
            UpdateJobEdit?.Invoke(this, "");
        }

        public void JobAddConfirmationListHandler(string inputMessage)
        {
            UpdateJobAdd?.Invoke(this, "");
        }

        public void UserEditConfirmationHandler(string inputMessage)
        {
            UpdateUserEdit?.Invoke(this, "");
        }

        public void UserAddConfirmationHandler(string inputMessage)
        {
            UpdateUserAdd?.Invoke(this, "");
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