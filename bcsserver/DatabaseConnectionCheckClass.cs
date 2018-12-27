using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace bcsserver
{
    /// <summary>
    /// Класс проверки соединения с базой данных
    /// </summary>
    internal class DatabaseConnectionCheckClass : IDisposable
    {
        private const int TimerInterval = 2000;

        private MainWindow MainForm;
        public bool IsWorking = false;
        private bool IsStarted = false;
        private System.Threading.Timer DatabaseCheckTimer;

        public DatabaseConnectionCheckClass(MainWindow AMainForm)
        {
            MainForm = AMainForm;
            DatabaseCheckTimer = new System.Threading.Timer(DatabaseConnectionCheckProcessing, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void DatabaseConnectionCheckProcessing(object state)
        {
            Thread th = new Thread(() =>
            {
                IsWorking = true;
                string mess = "Не установлено";
               
                    if (MainForm.DatabaseConnectionInit() && ((System.Data.DataTable)MainForm.Project.Database.Execute("ConnectionCheck")).Rows.Count == 1)
                    {
                        mess = "Установлено";
                }
                try
                {
                }
                catch
                {

                    mess = "Ошибка";
                }
                finally
                {
                    MainForm.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new System.Windows.Threading.DispatcherOperationCallback(delegate
                    {
                        MainForm.DatabaseConnectionStatus.Text = mess;
                        return null;
                    }), null);
                    DatabaseCheckTimer.Change(TimerInterval, Timeout.Infinite);
                    IsWorking = false;
                }
            });
            th.Start();
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            Thread th = new Thread(() =>
            {
                DatabaseCheckTimer.Change(1, Timeout.Infinite);
                IsStarted = true;
                while (IsStarted)
                {
                    Thread.Sleep(1);
                }
                DatabaseCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);
                IsWorking = false;
            });
            th.Start();
        }

        public void Stop()
        {
            IsStarted = false;
        }
    }
}
