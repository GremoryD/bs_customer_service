using System;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using CLProject;
using ServerLib.JTypes.Server;
using ServerLib.JTypes.Enums;

namespace bcsserver
{
    /// <summary>
    /// Класс сессии пользователя
    /// </summary>
    public class UserSessionClass
    {
        /// <summary>
        /// Вспомогательный класс проекта
        /// </summary>
        public ProjectClass Project;

        /// <summary>
        /// Идентификатор сессии WebSocket-сервера
        /// </summary>
        public string WebSocketSessionID { get; set; } = null;

        /// <summary>
        /// Выходная очередь сообщений клиенту
        /// </summary>
        public ConcurrentQueue<string> Responses = new ConcurrentQueue<string>();

        /// <summary>
        /// Входная очередь запросов
        /// </summary>
        private ConcurrentQueue<string> Requests = new ConcurrentQueue<string>();

        /// <summary>
        /// Обработчик WebSocket-соединения
        /// </summary>
        private WebSocketHandlerClass Handler;

        /// <summary>
        /// Признак активности сессии
        /// </summary>
        private bool IsActive { get; set; }

        /// <summary>
        /// Признак аутентификации пользователя
        /// </summary>
        public bool IsAuthenticated { get { return Login.ID > 0; } }

        /// <summary>
        /// Поток обработки входной очереди
        /// </summary>
        private Thread InputQueueProcessing;

        /// <summary>
        /// Поток обработки выходной очереди
        /// </summary>
        private Thread OutputQueueProcessing;

        /// <summary>
        /// Класс аутентификации пользователя
        /// </summary>
        public Handlers.LoginClass Login;

        /// <summary>
        /// Класс аутентификации пользователя
        /// </summary>
        public Handlers.LogoutClass Logout;

        /// <summary>
        /// Класс информации о пользователе
        /// </summary>
        public Handlers.UserInformationClass UserInformation;

        /// <summary>
        /// Обработчик списка пользователей
        /// </summary>
        public Handlers.HandlerUsersClass Users;

        /// <summary>
        /// Обработчик списка должностей пользователей
        /// </summary>
        public Handlers.HandlerJobsClass Jobs;

        /// <summary>
        /// Обработчик списка ролей пользователей
        /// </summary>
        public Handlers.HandlerRolesClass Roles;

        /// <summary>
        /// Обработчик списка ролей пользователей
        /// </summary>
        public Handlers.HandlerUsersRolesClass UsersRoles;

        /// <summary>
        /// Обработчик списка объектов системы
        /// </summary>
        public Handlers.HandlerObjectsClass Objects;

        /// <summary>
        /// Обработчик списка прав доступа ролей пользователей к объектам системы
        /// </summary>
        public Handlers.HandlerRolesObjectsClass RolesObjects;

        /// <summary>
        /// Конструктор класса сессии пользователя
        /// </summary>
        /// <param name="AWebSocketSessionIDSessionID">Идентификатор сессии WebSocket-сервера</param>
        /// <param name="AHandler">Обработчик WebSocket-соединения</param>
        /// <param name="AProject">Вспомогательный класс проекта</param>
        public UserSessionClass(string AWebSocketSessionIDSessionID, WebSocketHandlerClass AHandler, ref ProjectClass AProject)
        {
            WebSocketSessionID = AWebSocketSessionIDSessionID;
            Handler = AHandler;
            Project = AProject;
            IsActive = true;

            Login = new Handlers.LoginClass();
            Login.SetUserSession(this);

            Logout = new Handlers.LogoutClass();
            Logout.SetUserSession(this);

            UserInformation = new Handlers.UserInformationClass();
            UserInformation.SetUserSession(this);

            Users = new Handlers.HandlerUsersClass(this);
            Jobs = new Handlers.HandlerJobsClass(this);
            Roles = new Handlers.HandlerRolesClass(this);
            UsersRoles = new Handlers.HandlerUsersRolesClass(this);
            Objects = new Handlers.HandlerObjectsClass(this);
            RolesObjects = new Handlers.HandlerRolesObjectsClass(this);

            InputQueueProcessing = new Thread(InputQueueProcessingThread);
            OutputQueueProcessing = new Thread(OutputQueueProcessingThread);
            InputQueueProcessing.Start();
            OutputQueueProcessing.Start();
        }

        /// <summary>
        /// Остановка сессии
        /// </summary>
        public void Stop()
        {
            if (IsAuthenticated)
            {
                Logout.KillSession(Login.Token);
            }
            IsActive = false;
        }

        /// <summary>
        /// Поток обработчика входной очереди запросов
        /// </summary>
        private void InputQueueProcessingThread()
        {
            while (IsActive)
            {
                if (Requests.TryDequeue(out string Request))
                {
                    try
                    {
                        string CommandStr = string.Empty;
                        try
                        {
                            CommandStr = JsonConvert.DeserializeAnonymousType(Request, new { command = string.Empty }).command.ToLower();
                        }
                        catch
                        {
                            OutputQueueAddObject(new ResponseExceptionClass(Commands.none, ErrorCodes.NotJSONObject));
                        }
                        if (CommandStr.Length > 0)
                        {
                            Enum.TryParse(CommandStr, out Commands Command);
                            if (Enum.IsDefined(typeof(Commands), Command) && Command != Commands.none)
                            {
                                if (Command == Commands.login)
                                {
                                    Login.Login(Request);
                                }
                                else
                                {
                                    string Token = JsonConvert.DeserializeAnonymousType(Request, new { token = string.Empty }).token;
                                    if (Token == Login.Token)
                                    {
                                        if (IsAuthenticated)
                                        {
                                            switch (Command)
                                            {
                                                case Commands.logout:
                                                    Logout.Logout(Request);
                                                    break;
                                                case Commands.users:
                                                    Users.SendData();
                                                    break;
                                                case Commands.user_add:
                                                    Users.Add(Request);
                                                    break;
                                                case Commands.user_edit:
                                                    Users.Edit(Request);
                                                    break;
                                                case Commands.jobs:
                                                    Jobs.SendData();
                                                    break;
                                                case Commands.job_add:
                                                    Jobs.Add(Request);
                                                    break;
                                                case Commands.job_edit:
                                                    Jobs.Edit(Request);
                                                    break;
                                                case Commands.roles:
                                                    Roles.SendData();
                                                    break;
                                                case Commands.roles_add:
                                                    Roles.Add(Request);
                                                    break;
                                                case Commands.roles_edit:
                                                    Roles.Edit(Request);
                                                    break;
                                                case Commands.users_roles:
                                                    UsersRoles.SendData();
                                                    break;
                                                case Commands.users_roles_add:
                                                    UsersRoles.Add(Request);
                                                    break;
                                                case Commands.users_roles_delete:
                                                    UsersRoles.Delete(Request);
                                                    break;
                                                case Commands.objects:
                                                    Objects.SendData();
                                                    break;
                                                case Commands.roles_objects:
                                                    RolesObjects.SendData();
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            OutputQueueAddObject(new ResponseExceptionClass(Command, ErrorCodes.NotAuthenticated));
                                        }
                                    }
                                    else
                                    {
                                        OutputQueueAddObject(new ResponseExceptionClass(Command, ErrorCodes.IncorrectToken));
                                    }
                                }
                            }
                            else
                            {
                                OutputQueueAddObject(new { command = CommandStr, state = "error", code = "UnknownCommand" });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        OutputQueueAddObject(new { state = "error", code = "FatalError", description = ex.Message });
                    }
                }
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Добавляет текст запроса во входную очередь
        /// </summary>
        /// <param name="ARequest">Текст запроса</param>
        public void InputQueueAdd(string ARequest)
        {
            Requests.Enqueue(ARequest);
        }

        /// <summary>
        /// Добавляет текст ответа в выходную очередь
        /// </summary>
        /// <param name="AResponse">Текст ответа</param>
        public void OutputQueueAddString(string AResponse)
        {
            Responses.Enqueue(AResponse);
        }

        /// <summary>
        /// Добавляет объект в выходную очередь
        /// </summary>
        /// <param name="AResponse">Объект</param>
        public void OutputQueueAddObject(object AResponse)
        {
            OutputQueueAddString(JsonConvert.SerializeObject(AResponse));
        }

        /// <summary>
        /// Поток передачи данных из выходной очереди через обработчик WebSocket-соединения
        /// </summary>
        private void OutputQueueProcessingThread()
        {
            while (IsActive)
            {
                if (Responses.TryDequeue(out string Response))
                {
                    Handler.SendString(Response);
                }
                Thread.Sleep(1);
            }
        }
    }
}
