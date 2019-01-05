using bcs.Models;
using CLProject;
using ServerLib.JTypes.Client;
using ServerLib.JTypes.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace bcs.ViewModels
{
    public class LoginViewModel : IViewModel, INotifyPropertyChanged
    {
        public String LoginInput { get; set; }
        public String PasswordInput { get; set; }
        public Boolean IsWait { get; set; }
        public ICommand LoginCommand { get; set; }


        public LoginViewModel()
        {
            Singleton.Instance.OnLogonSuccess += LogonDone;

            LoginCommand = new SimpleCommand(LoginF);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoginF()
        {
            Login login = new Login()
            {
                Password = PasswordInput,
                UserName = LoginInput
            };

            Singleton.Instance.SendLogin(login);
            IsWait = true;
            Notify("IsWait");
        }

        private void LogonDone(object sender, LoginClass e)
        {
            try
            {
                NavigationService.Instance.Navigate(new ClientViewModel());
                MessageBox.Show(e.Token);
            }
            catch { }
            finally
            {
                IsWait = false;
                Notify("IsWait");
            }
        }
    }
}
