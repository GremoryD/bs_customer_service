using CLProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ServerLib.JTypes;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerLib.JTypes.Client;

namespace BCS_User_Interface
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.Project = new ProjectClass("Application");
            App.WebSocketClient = new WebSocketClass(ref App.Project, Properties.Settings.Default.WSServer, "/");
            App.WebSocketClient.Start();
        }

        private void simpleButton_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login()
            {
                Password = Password.Password,
                UserName = Login.Text
            };
            App.WebSocketClient.Send(login);
        }
    }
}
