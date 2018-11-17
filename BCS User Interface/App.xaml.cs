using CLProject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BCS_User_Interface
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string AMessage1 = "Приложение загружено";
        private const string AMessage2 = "Приложение запущено";
        private const string AMessage3 = "Приложение остановлено";
        private const string AMessage4 = "Приложение выгружено";

        public static ProjectClass Project;
        public static WebSocketClass WebSocketClient;
    }
}
