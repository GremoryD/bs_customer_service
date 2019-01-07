using System;
using System.Windows;
using Newtonsoft.Json;

namespace bcsapp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ApplClass Appl;

        public MainWindow()
        {
            InitializeComponent();
            Appl = new ApplClass(this);
            Output.Text = string.Empty;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Appl.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Appl.Start();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SendObject(new ServerLib.JTypes.Client.Login()
            { 
                UserName = "admin",
                Password = "123456"
            });
        }

        public void SendObject(object AObject)
        {
            Output.Text = string.Format("{0}Send: {1}\n\r", Output.Text, JsonConvert.SerializeObject(AObject));
            Appl.OutputQueueAddObject(AObject);
        }

        private void LoginError_Click(object sender, RoutedEventArgs e)
        {
            SendObject(new ServerLib.JTypes.Client.Login()
            {
                UserName = "admin",
                Password = "Wrong Password"
            });
        }
    }
}
