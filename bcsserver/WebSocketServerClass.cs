using System;
using CLProject;
using WebSocketSharp.Server;

namespace bcsserver
{
    /// <summary>
    /// Сервер
    /// </summary>
    class WebSocketServerClass
    {
        /// <summary>
        /// Вспомогательный класс проекта
        /// </summary>
        private ProjectClass Project;

        /// <summary>
        /// WebSocket-сервер
        /// </summary>
        private WebSocketServer wsServer;

        /// <summary>
        /// Дата и время запуска сервера
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// Количество активных сессий
        /// </summary>
        public int SessionCount
        {
            get
            {
                return wsServer == null ? 0 : wsServer.WebSocketServices.SessionCount;
            }
        }

        /// <summary>
        /// Признак запущен ли сервер
        /// </summary>
        public bool IsRunning = false;

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="AProject"></param>
        public WebSocketServerClass(ref ProjectClass AProject)
        {
            Project = AProject;
            StartTime = DateTime.MinValue;
        }

        /// <summary>
        /// Инициализирует обработчик WebSocket-соединения
        /// </summary>
        /// <returns></returns>
        private WebSocketHandlerClass HandlerMainCreator()
        {
            return new WebSocketHandlerClass(ref Project);
        }

        /// <summary>
        /// Запуск сервера
        /// </summary>
        public void Start()
        {
            wsServer = new WebSocketServer(System.Net.IPAddress.Any, Properties.Settings.Default.port);
            //wsServer.WebSocketServices.AddService<WebSocketHandlerClass>("/", );
            wsServer.AddWebSocketService("/", HandlerMainCreator);
            wsServer.Start();
            StartTime = DateTime.Now;
            IsRunning = true;
        }

        /// <summary>
        /// Остановка сервера
        /// </summary>
        public void Stop()
        {
            wsServer.Stop();
            IsRunning = false;
            StartTime = DateTime.MinValue;
        }
    }
}
