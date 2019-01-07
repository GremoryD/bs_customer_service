using System;
using CLProject;
using WebSocketSharp;
using System.Threading;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace bcsapp
{
    /// <summary>
    /// Класс приложения
    /// </summary>
    public class ApplClass
    {
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

        /// <summary>
        /// Признак аутентификации пользователя
        /// </summary>
        public bool IsAuthenticated { get { return Session.Login.UserId > 0; } }


        Handlers.SessionClass Session;

        MainWindow MainForm;

        public ApplClass(MainWindow AMainForm)
        {
            MainForm = AMainForm;
            Project = new ProjectClass("BCSApp");
            WebSocketClient = new WebSocket(string.Format("ws://{0}:{1}/", Project.Settings.WebSocketServerAddress, Project.Settings.WebSocketServerPort));
            WebSocketClient.OnMessage += OnWebSocketMessage;
            ConnectToWebSocketServerThread = new Thread(ConnectToWebSocketServer);
            InputQueueProcessing = new Thread(InputQueueProcessingThread);
            OutputQueueProcessing = new Thread(OutputQueueProcessingThread);
            Session = new Handlers.SessionClass();
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
                        MainForm.Output.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => { MainForm.Output.Text = string.Format("{0}Recieve: {1}\n\r", MainForm.Output.Text, InputMessage); }));
                        ServerLib.JTypes.Server.BaseResponseClass Message = JsonConvert.DeserializeObject<ServerLib.JTypes.Server.BaseResponseClass>(InputMessage);
                        if (Message.State == ServerLib.JTypes.Enums.ResponseState.ok)
                        {
                            switch(Message.Command)
                            {
                                case ServerLib.JTypes.Enums.Commands.login:
                                    Session.LoginProcessing(JsonConvert.DeserializeObject<ServerLib.JTypes.Server.LoginClass>(InputMessage));
                                    break;
                                case ServerLib.JTypes.Enums.Commands.user_information:
                                    Session.UserInformationProcessing(JsonConvert.DeserializeObject<ServerLib.JTypes.Server.UserInformationClass>(InputMessage));
                                    break;
                            }
                        }
                        else if (Message.State == ServerLib.JTypes.Enums.ResponseState.error)
                        {
                            switch (Message.Command)
                            {
                                case ServerLib.JTypes.Enums.Commands.login:
                                    Session.LoginErrorProcessing(JsonConvert.DeserializeObject<ServerLib.JTypes.Server.ExceptionClass>(InputMessage));
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
                if (OutputQueue.TryDequeue(out string Message))
                {
                    WebSocketClient.Send(Message);
                }
                Thread.Sleep(1);
            }
        }
    }
}