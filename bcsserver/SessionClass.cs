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

        public ConcurrentQueue<string> Responses = new ConcurrentQueue<string>();
        private ConcurrentQueue<string> Requests = new ConcurrentQueue<string>();
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
            //Users.SetUserSession(this);

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

        private void InputQueueProcessingThread()
        {
            while (IsActive)
            {
                if (Requests.TryDequeue(out string Request))
                {
                    try
                    {
                        string Command = JsonConvert.DeserializeAnonymousType(Request, new { command = string.Empty }).command.ToLower();
                        if (Command == "login")
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
                                        case "logout":
                                            Logout.Logout(Request);
                                            break;
                                        case "users":
                                            Users.SendData();
                                            break;
                                        default:
                                            OutputQueueAddObject(Exceptions.ErrorUnknownCommand);
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
