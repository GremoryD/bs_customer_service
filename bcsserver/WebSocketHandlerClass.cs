using System;
using CLProject;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace bcsserver
{
    /// <summary>
    /// Обработчик запросов поступающих от клиента через WebSocket-соединение
    /// </summary>
    public class WebSocketHandlerClass : WebSocketBehavior
    {
        /// <summary>
        /// Потокозащищённый словарь сессий клиентов
        /// </summary>
        private static ConcurrentDictionary<string, UserSessionClass> UserSessions = new ConcurrentDictionary<string, UserSessionClass>();

        /// <summary>
        /// Вспомогательный класс проекта
        /// </summary>
        private ProjectClass Project;

        public WebSocketHandlerClass() { }

        /// <summary>
        /// Конструктор класса обработки WebSocket-соединений
        /// </summary>
        /// <param name="AProject">Вспомогательный класс проекта</param>
        public WebSocketHandlerClass(ref ProjectClass AProject)
        {
            Project = AProject;
        }

        /// <summary>
        /// Отправляет строку клиенту через WebSocket-соединение
        /// </summary>
        /// <param name="AString">Строка передаваемая клиенту WebSocket-соединения</param>
        public void SendString(string AString)
        {
            Send(AString);
        }

        /// <summary>
        /// Отправляет JSON-объект клиенту через WebSocket-соединение
        /// </summary>
        /// <param name="AObject">Объект передаваемый клиенту WebSocket-соединения</param>
        public void SendObject(object AObject)
        {
            SendString(JsonConvert.SerializeObject(AObject));
        }

        /// <summary>
        /// Вызывается при открытии WebSocket-соединения
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen();
            UserSessions.TryAdd(ID, new UserSessionClass(ID, this, ref Project));
        }

        /// <summary>
        /// Вызывается при закрытии WebSocket-соядинения
        /// </summary>
        /// <param name="e">Содержит данные события OnClose</param>
        protected override void OnClose(CloseEventArgs e)
        {
            if (UserSessions.TryGetValue(ID, out UserSessionClass UserSession))
            {
                UserSession.Stop();
                UserSessions.TryRemove(ID, out UserSessionClass UserSessionRemoved);
            }
        }

        /// <summary>
        /// Вызывается при получении данных от клиента через WebSocket-соединение
        /// </summary>
        /// <param name="e">Содержит данные события OnMessage</param>
        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                if (e.IsText)
                {
                    if (UserSessions.TryGetValue(ID, out UserSessionClass UserSession))
                    {
                        UserSession.InputQueueAdd(e.Data);
                    }
                    else
                    {
                        SendObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.none, ServerLib.JTypes.Enums.ErrorCodes.SessionNotFound));
                    }
                }
                else
                {
                    SendObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.none, ServerLib.JTypes.Enums.ErrorCodes.NonTextMessage));
                }
            }
            catch (Exception ex)
            {
                SendObject(new ServerLib.JTypes.Server.ExceptionClass(ServerLib.JTypes.Enums.Commands.none, ServerLib.JTypes.Enums.ErrorCodes.FatalError, ex.Message));
            }
        }
    }
}
