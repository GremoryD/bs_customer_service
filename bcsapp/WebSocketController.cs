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
                                case ServerLib.JTypes.Enums.Commands.jobs:
                                    JobssListHandler(InputMessage);
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
        public event EventHandler<List<UserClass>> UpdateUsers;
        public event EventHandler<List<JobClass>> UpdateJobs;

        #endregion

        #region Обработчики 

        private void LoginHandler(string InputMessage)
        {
            DataStorage.Instance.Login = JsonConvert.DeserializeObject<LoginClass>(InputMessage);
            LoginDone?.Invoke(this, JsonConvert.DeserializeObject<LoginClass>(InputMessage));
        }

        private void UserInformationHandler(string InputMessage)
        {
            DataStorage.Instance.UserInformation = JsonConvert.DeserializeObject<UserInformationClass>(InputMessage);
            UpdateUserUI?.Invoke(this, String.Format("{0} {1} {2}", DataStorage.Instance.UserInformation.FirstName, DataStorage.Instance.UserInformation.MidleName, DataStorage.Instance.UserInformation.LastName));
        }

        private void UsersListHandler(string InputMessage)
        {
            if (DataStorage.Instance.UserList != null)
            {
                DataStorage.Instance.UserList = JsonConvert.DeserializeObject<UsersClass>(InputMessage).Items;
            }
            else
            {

            }
            UpdateUsers?.Invoke(this, DataStorage.Instance.UserList);
        }

        private void JobssListHandler(string InputMessage)
        {
            if (DataStorage.Instance.JobList != null)
            {
                DataStorage.Instance.JobList = JsonConvert.DeserializeObject<JobsClass>(InputMessage).Jobs;

            }
            else
            {

            }
            UpdateJobs?.Invoke(this, DataStorage.Instance.JobList);
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