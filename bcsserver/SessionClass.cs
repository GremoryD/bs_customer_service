using System;
using CLProject;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using ServerLib.JTypes.Server;

namespace bcsserver
{
    public class UserSessionClass
    {
        public ProjectClass Project;
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
        private bool IsAuthenticated { get { return UserInformation.UserId > 0; } }

        /// <summary>
        /// Поток обработки входной очереди
        /// </summary>
        private Thread InputQueueProcessing;

        /// <summary>
        /// Поток обработки выходной очереди
        /// </summary>
        private Thread OutputQueueProcessing;

        /// <summary>
        /// Класс информации о пользователе
        /// </summary>
        private UserInformationClass UserInformation;

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
            UserInformation = new UserInformationClass();
            UserInformation.SetUserSession(this);
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
                UserInformation.KillSession(UserInformation.Token);
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
                            UserInformation.Login(Request);
                        }
                        else
                        {
                            string Token = JsonConvert.DeserializeAnonymousType(Request, new { token = string.Empty }).token;
                            if (IsAuthenticated && Token == UserInformation.Token)
                            {
                                switch (Command)
                                {
                                    case "logout":
                                        UserInformation.Logout(Request);
                                        break;
                                    default:
                                        OutputQueueAddObject(Exceptions.ErrorUnknownCommand);
                                        break;
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
