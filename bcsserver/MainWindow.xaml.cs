using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using CLProject;

namespace bcsserver
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string AMessage1 = "Настройки сохранены";

        /// <summary>
        /// Класс управления проектом
        /// </summary>
        public ProjectClass Project = new ProjectClass("Application");

        /// <summary>
        /// Класс сервера
        /// </summary>
        private WebSocketServerClass Server;

        /// <summary>
        /// Признак того, что сервер запущен
        /// </summary>
        public bool IsStarted = false;

        /// <summary>
        /// Класс проверки соединения с базой данных
        /// </summary>
        private DatabaseConnectionCheckClass DatabaseCheck;

        /// <summary>
        /// Хранит предыдущий выбор в главном меню для возможности возврата при выборе кнопки, которая не содержит отображения в рабочей области в правой части главного окна
        /// </summary>
        private int previousSelection = 0;

        private System.Threading.Timer ServerStateTimer;
        private const int ServerStateTimerInterval = 1000;

        public MainWindow()
        {
            InitializeComponent();
            mainMenu.SelectedIndex = 0;
            mainMenu.Focus();
            ServerStateTimer = new System.Threading.Timer(ServerStateProcessing, null, Timeout.Infinite, ServerStateTimerInterval);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseCheck = new DatabaseConnectionCheckClass(this);
            Server = new WebSocketServerClass(ref Project);

            Title = string.Format("{0} v{1}", Title, ProgramInfo.Version);

            TNS_Name.Text = Properties.Settings.Default.tns_name;
            Login.Text = Properties.Settings.Default.login;
            Password.Password = Properties.Settings.Default.password;
            Port.Value = Properties.Settings.Default.port;
            LogsPath.Text = Project.Settings.LogPath;
            IssueOnExit.IsChecked = Project.Settings.IssueOnExit;

            Project.Database.Connections.CreateConnection("conn");

            Project.Database.Parameters.CreateParameter("WebSocketID", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Идентификатор сессии сервера");
            Project.Database.Parameters.CreateParameter("Login", System.Data.ParameterDirection.Input , OracleDbType.NVarchar2, "Логин пользователя");
            Project.Database.Parameters.CreateParameter("Password", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Пароль пользователя");
            Project.Database.Parameters.CreateParameter("AccessToken", System.Data.ParameterDirection.Output, OracleDbType.NVarchar2, "Токен доступа", 100);
            Project.Database.Parameters.CreateParameter("AccessUserId", System.Data.ParameterDirection.Output, OracleDbType.Decimal, "Id пользователя в БД", 20);
            Project.Database.Parameters.CreateParameter("NewId", System.Data.ParameterDirection.Output, OracleDbType.Decimal, "Id сущности после создания в БД", 20);
            Project.Database.Parameters.CreateParameter("UserId", System.Data.ParameterDirection.Input, OracleDbType.Decimal, "Id пользователя в БД", 20);
            Project.Database.Parameters.CreateParameter("InId", System.Data.ParameterDirection.Input, OracleDbType.Decimal, "Id", 20);
            Project.Database.Parameters.CreateParameter("FirstName", System.Data.ParameterDirection.Output, OracleDbType.NVarchar2, "Имя пользователя", 100);
            Project.Database.Parameters.CreateParameter("LastName", System.Data.ParameterDirection.Output, OracleDbType.NVarchar2, "Фамилия пользователя", 100);
            Project.Database.Parameters.CreateParameter("MidleName", System.Data.ParameterDirection.Output, OracleDbType.NVarchar2, "Отчество пользователя", 100);
            Project.Database.Parameters.CreateParameter("InFirstName", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Имя пользователя", 100);
            Project.Database.Parameters.CreateParameter("InLastName", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Фамилия пользователя", 100);
            Project.Database.Parameters.CreateParameter("InMidleName", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Отчество пользователя", 100);
            Project.Database.Parameters.CreateParameter("Job", System.Data.ParameterDirection.Output, OracleDbType.NVarchar2, "Должность пользователя", 200);
            Project.Database.Parameters.CreateParameter("JobName", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Должность пользователя", 200);
            Project.Database.Parameters.CreateParameter("JobId", System.Data.ParameterDirection.Input, OracleDbType.Decimal, "Идентификатор должности", 20);
            Project.Database.Parameters.CreateParameter("Active", System.Data.ParameterDirection.Output, OracleDbType.Decimal, "Признак активности пользователя (1 - активен, 0 - заблокирован)", 20);
            Project.Database.Parameters.CreateParameter("InActive", System.Data.ParameterDirection.Input, OracleDbType.Decimal, "Признак активности пользователя (1 - активен, 0 - заблокирован)", 20);
            Project.Database.Parameters.CreateParameter("Token", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Ключ доступа");
            Project.Database.Parameters.CreateParameter("State", System.Data.ParameterDirection.Output, OracleDbType.NVarchar2, "Статус завершения SQL-запроса", 20);
            Project.Database.Parameters.CreateParameter("ErrorText", System.Data.ParameterDirection.Output, OracleDbType.NVarchar2, "Текст ошибки выполнения SQL-запроса", 500);
            Project.Database.Parameters.CreateParameter("Name", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Наименование", 200);
            Project.Database.Parameters.CreateParameter("Description", System.Data.ParameterDirection.Input, OracleDbType.NVarchar2, "Описание", 200);

            Project.Database.Commands.CreateCommand("conn", RequestType.Reader, "ConnectionCheck", "SELECT 1 FROM DUAL", "Проверка соединения с базой данных");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "Login", "USR.LOGIN(:WebSocketID, :Login, :Password, :AccessToken, :AccessUserId, :Active)", "Аутентификация пользователя");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "Logout", "USR.LOGOUT(:Token, :State)", "Выход из системы");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "UserInformation", "USR.GET_USER_INFORMATION(:Token, :UserId, :FirstName, :LastName, :MidleName, :Job, :Active, :State, :ErrorText)", "Чтение информации и пользователе");
            Project.Database.Commands.CreateCommand("conn", RequestType.Table, "Users", "USR.GET_USERS(:Token)", "Чтение списка пользователей");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "UserAdd", "USR.USER_CREATE(:Token, :Login, :Password, :InFirstName, :InLastName, :InMidleName, :InActive, :JobId, :AccessUserId, :Job, :State, :ErrorText)", "Добавление пользователя");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "UserEdit", "USR.USER_UPDATE(:Token, :UserId, :InFirstName, :InLastName, :InMidleName, :InActive, :JobId, :Job, :State, :ErrorText)", "Изменение пользователя");
            Project.Database.Commands.CreateCommand("conn", RequestType.Table, "Jobs", "USR.GET_JOBS(:Token)", "Чтение списка должностей пользователей");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "JobAdd", "USR.JOB_CREATE(:Token, :JobName, :NewId, :State, :ErrorText)", "Добавление должности пользователей");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "JobEdit", "USR.JOB_UPDATE(:Token, :JobName, :JobId, :State, :ErrorText)", "Изменение должности пользователей");
            Project.Database.Commands.CreateCommand("conn", RequestType.Table, "Roles", "USR.GET_ROLES_LIST(:Token)", "Чтение списка ролей пользователей");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "RoleAdd", "USR.USER_ROLES_LIST_ADD(:Token, :Name, :Description, :NewId, :State, :ErrorText)", "Добавление роли пользователей");
            Project.Database.Commands.CreateCommand("conn", RequestType.Procedure, "RoleEdit", "USR.USER_ROLES_LIST_WRITE(:Token, :Name, :Description, :InId, :State, :ErrorText)", "Изменение роли пользователей");

            DatabaseCheck.Start();
            SetSettingsButtons();

           ServerStateTimer.Change(1, Timeout.Infinite);
        }

        private void MailWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Project.Settings.IssueOnExit)
            {
                if (System.Windows.MessageBox.Show("Завершить работу\r\nсервера клиентской поддержки?", "Завершение работы", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question) == System.Windows.MessageBoxResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
            }
            mainMenu.SelectedIndex = previousSelection;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            DatabaseCheck.Dispose();
            Stop();
            ServerStateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            Project.Dispose();
        }

        public bool DatabaseConnectionInit()
        {
            bool IsInitConnection = Properties.Settings.Default.tns_name.Length > 0 && Properties.Settings.Default.login.Length > 0 && Properties.Settings.Default.password.Length > 0;
            if (IsInitConnection)
            {
                Project.Database.Connections.ConnectionByName("conn").DataSource = Properties.Settings.Default.tns_name;
                Project.Database.Connections.ConnectionByName("conn").UserID = Properties.Settings.Default.login;
                Project.Database.Connections.ConnectionByName("conn").Password = Properties.Settings.Default.password;
            }
            return (IsInitConnection);
        }

        private void Start()
        {
            if (DatabaseConnectionInit())
            {
                Server.Start();
                IsStarted = Server.IsRunning;
            }
        }

        private void Stop()
        {
            if (IsStarted)
            {
                Server.Stop();
                IsStarted = Server.IsRunning;
            }
        }

        private void ServerStateProcessing(object state)
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    string stateServer = Server.IsRunning ? "Запущен" : "Остановлен";
                    Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new DispatcherOperationCallback(delegate
                    {
                        Status.Text = stateServer;
                        ConnectionCount.Text = Server.SessionCount.ToString();
                        StartTime.Text = Server.StartTime == DateTime.MinValue ? string.Empty : Server.StartTime.ToString();
                        WorkingTime.Text = Server.StartTime == DateTime.MinValue ? string.Empty : Utils.PassedTime(Server.StartTime);
                        return null;
                    }), null);
                }
                catch (Exception ex)
                {
                    Project.Log.Error(string.Format("Ошибка отображения статуса: {0}", ex.Message));
                }
                finally
                {
                    ServerStateTimer.Change(ServerStateTimerInterval, Timeout.Infinite);
                }
            });
            th.Start();
        }


    #region Настройки
    private void MainMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControlMain != null && mainMenu.SelectedItem != null)
            {
                string mainMenuItemName = ((ListBoxItem)mainMenu.SelectedItem).Name;
                if (mainMenuItemName.ToLower() == "mainMenuQuit".ToLower())
                {
                    Close();
                }
                else if (mainMenuItemName.ToLower() == "mainMenuStatus".ToLower())
                {
                    tabControlMain.SelectedIndex = 0;
                }
                else if (mainMenuItemName.ToLower() == "mainMenuSettings".ToLower())
                {
                    tabControlMain.SelectedIndex = 1;
                }
                previousSelection = tabControlMain.SelectedIndex;
            }
        }

        private void SetSettingsButtons()
        {
            btnSettings_Save.IsEnabled =
                Properties.Settings.Default.tns_name != TNS_Name.Text ||
                Properties.Settings.Default.login != Login.Text ||
                Properties.Settings.Default.password != Password.Password ||
                Properties.Settings.Default.port != Port.Value ||
                Project.Settings.LogPath != LogsPath.Text ||
                Project.Settings.IssueOnExit != Convert.ToBoolean(IssueOnExit.IsChecked);
            btnStop.IsEnabled = IsStarted;
            btnStart.IsEnabled = !IsStarted;
        }

        private void BtnSettings_Save_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.tns_name = TNS_Name.Text;
            Properties.Settings.Default.login = Login.Text;
            Properties.Settings.Default.password = Password.Password;
            Properties.Settings.Default.port = Convert.ToInt32(Port.Value);
            Project.Settings.LogPath = LogsPath.Text;
            Project.Settings.LogPathArchive = System.IO.Path.Combine(Project.Settings.LogPath, "Archive");
            Project.Settings.IssueOnExit = Convert.ToBoolean(IssueOnExit.IsChecked);
            Project.Settings.Save();
            Properties.Settings.Default.Save();
            Project.Log.Trace(AMessage1);
            SetSettingsButtons();
        }

        private void IssueOnExit_Click(object sender, RoutedEventArgs e)
        {
            SetSettingsButtons();
        }

        private void TNS_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSettingsButtons();
        }

        private void Login_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSettingsButtons();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            SetSettingsButtons();
        }

        private void LogsPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetSettingsButtons();
        }

        private void Port_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetSettingsButtons();
        }

        private void LogsPathChange_Click(object sender, RoutedEventArgs e)
        {
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                fbd.RootFolder = Environment.SpecialFolder.MyComputer;
                fbd.Description = "Путь размещения журнальных файлов";
                fbd.SelectedPath = LogsPath.Text;
                System.Windows.Forms.DialogResult result = fbd.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath) && Directory.Exists(fbd.SelectedPath))
                {
                    LogsPath.Text = fbd.SelectedPath;
                }
            }
        }
        #endregion

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            SetSettingsButtons();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            Start();
            SetSettingsButtons();
        }
    }
}
