using System;
using CLProject;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using ServerLib.JTypes.Server;

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
        public bool IsAuthenticated { get { return Login.UserId > 0; } }

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
        public Handlers.UsersClass Users;

        /// <summary>
        /// Конструктор класса сессии пользователя
        /// </summary>
        /// <param name="AWebSocketSessionIDSessionID">Идентификатор сессии WebSocket-сервера</param>
        /// <param name="AHandler"></param>
        /// <param name="AProject"></param>
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

            Users = new Handlers.UsersClass(this);

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
                        Enum.TryParse(JsonConvert.DeserializeAnonymousType(Request, new { command = string.Empty }).command.ToLower(), out ServerLib.JTypes.Enums.Commands Command);
                        if (Enum.IsDefined(typeof(ServerLib.JTypes.Enums.Commands), Command) && Command != ServerLib.JTypes.Enums.Commands.none)
                        {
                            if (Command == ServerLib.JTypes.Enums.Commands.login)
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
                                            case ServerLib.JTypes.Enums.Commands.logout:
                                                Logout.Logout(Request);
                                                break;
                                            case ServerLib.JTypes.Enums.Commands.users:
                                                Users.SendData();
                                                break;
                                            case ServerLib.JTypes.Enums.Commands.user_add:
                                                Users.UserAdd(Request);
                                                break;
                                            case ServerLib.JTypes.Enums.Commands.user_edit:
                                                Users.UserEdit(Request);
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        OutputQueueAddObject(Exceptions.ErrorNotAuthenticated);
                                    }
                                }
                                else
                                {
                                    OutputQueueAddObject(Exceptions.ErrorIncorrectToken);
                                }
                            }
                        }
                        else
                        {
                            OutputQueueAddObject(Exceptions.ErrorUnknownCommand);
                        }
                    }
                    catch (Exception ex)
                    {
                        OutputQueueAddObject(new { state = "error", code = ex.HResult, description = ex.Message });
                    }
                }
                Thread.Sleep(1);
            }
        }

        public void InputQueueAdd(string ARequest)
        {
            Requests.Enqueue(ARequest);
        }

        public void OutputQueueAddString(string AResponse)
        {
            Responses.Enqueue(AResponse);
        }

        public void OutputQueueAddObject(object AResponse)
        {
            OutputQueueAddString(JsonConvert.SerializeObject(AResponse));
        }

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
