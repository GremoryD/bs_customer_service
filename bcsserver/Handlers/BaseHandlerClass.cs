using System;
using System.Threading;

namespace bcsserver.Handlers
{
    /// <summary>
    /// Базовый обработчик
    /// </summary>
    public class BaseHandlerClass
    {
        /// <summary>
        /// Сессия пользователя
        /// </summary>
        public UserSessionClass UserSession;

        /// <summary>
        /// Таймер обновления данных 
        /// </summary>
        public Timer RefreshDataTimer;

        /// <summary>
        /// Признак запуска потока чтения данных
        /// </summary>
        public bool RefreshDataIsRunning = false;

        public BaseHandlerClass(UserSessionClass AUserSession)
        {
            UserSession = AUserSession;
            RefreshDataTimer = new Timer(RefreshDataProcessing, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Поток обновления данных 
        /// </summary>
        /// <param name="state"></param>
        public void RefreshDataProcessing(object state)
        {
            if (UserSession.IsAuthenticated)
            {
                if (!RefreshDataIsRunning)
                {
                    Thread th = new Thread(() =>
                    {
                        try
                        {
                            RefreshDataIsRunning = true;
                            RefreshData();
                        }
                        catch (Exception ex)
                        {
                            UserSession.Project.Log.Error(string.Format("RefreshDataProcessing {0}", ex.Message));
                        }
                        finally
                        {
                            RefreshDataIsRunning = false;
                        }
                    });
                    th.Start();
                }
                RefreshDataTimer.Change(5000, Timeout.Infinite);
            }
        }

        /// <summary>
        /// Обновление данных
        /// </summary>
        public virtual void RefreshData() { }

        /// <summary>
        /// Добавление пользователя
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void Add(string ARequest)
        {
            Thread th = new Thread(() =>
            {
                AddProcessing(ARequest);
            });
            th.Start();
        }

        /// <summary>
        /// Обработчик добавления
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public virtual void AddProcessing(string ARequest) { }

        /// <summary>
        /// Изменение 
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void Edit(string ARequest)
        {
            Thread th = new Thread(() =>
            {
                EditProcessing(ARequest);
            });
            th.Start();
        }

        /// <summary>
        /// Обработчик изменения
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public virtual void EditProcessing(string ARequest) { }

        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public void Delete(string ARequest)
        {
            Thread th = new Thread(() =>
            {
                DeleteProcessing(ARequest);
            });
            th.Start();
        }

        /// <summary>
        /// Обработчик удаления
        /// </summary>
        /// <param name="ARequest">Запрос в формате JSON-объекта</param>
        public virtual void DeleteProcessing(string ARequest) { }

        /// <summary>
        /// Принудительное чтение из базы данных и отправка данных клиенту
        /// </summary>
        public void SendData()
        {
            RefreshDataTimer.Change(1, Timeout.Infinite);
        }
    }
}
