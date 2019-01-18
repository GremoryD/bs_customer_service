using System;
using System.Threading;

namespace bcsserver
{
    /// <summary>
    /// Класс проверки соединения с базой данных
    /// </summary>
    internal class DatabaseConnectionCheckClass : IDisposable
    {
        private const int TimerInterval = 3000;
        private MainWindow MainForm;
        public bool IsWorking = false;
        private bool IsStarted = false;
        private Timer DatabaseCheckTimer;

        public DatabaseConnectionCheckClass(MainWindow AMainForm)
        {
            MainForm = AMainForm;
            DatabaseCheckTimer = new Timer(DatabaseConnectionCheckProcessing, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void DatabaseConnectionCheckProcessing(object state)
        {
            new Thread(() =>
                        {
                            IsWorking = true;
                            string mess = "Не установлено";
                            try
                            {
                                if (MainForm.DatabaseConnectionInit() && ((System.Data.DataTable)MainForm.Project.Database.Execute("ConnectionCheck")).Rows.Count == 1)
                                {
                                    mess = "Установлено";
                                }
                            }
                            catch (Exception ex)
                            {
                                mess = string.Format("Ошибка {0}", ex.Message);
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
                        }).Start();
        }

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            new Thread(() =>
                        {
                            try
                            {
                                IsStarted = true;
                                DatabaseCheckTimer.Change(1, Timeout.Infinite);
                                while (IsStarted)
                                {
                                    Thread.Sleep(1);
                                }
                            }
                            catch { }
                            finally
                            {
                                DatabaseCheckTimer.Change(Timeout.Infinite, Timeout.Infinite);
                                IsWorking = false;
                            }
                        }).Start();
        }

        public void Stop()
        {
            IsStarted = false;
        }
    }
}
