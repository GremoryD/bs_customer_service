using bcsapp.Controls;
using bcsapp.ViewModels;
using CLProject;
using Newtonsoft.Json;
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

namespace bcsapp.ViewModels
{
    public class LoginViewModel : IViewModel, INotifyPropertyChanged
    {
        //Вывод данных на форму
        public String LoginInput { get; set; }
        public String PasswordInput { get; set; }
        public String Output { set; get; }
        public String ErrState { get; set; }
        //отображения загрузки и ошибки
        public Boolean IsWait { get; set; }
        public Boolean IsError { get; set; }
        public Boolean IsBlocked { get; set; }

        //команды 
        public ICommand LoginCommand { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        { 
            LoginCommand = new SimpleCommand(LoginF, (_) =>
            {
                if (LoginInput == null || PasswordInput == null)
                    return false;

                return LoginInput.Length >= 5 && PasswordInput.Length >=6;
            });

            WebSocketController.Instance.LoginFailed += (_, __) => ErrorMessage(__);
            WebSocketController.Instance.LoginDone += (_, __) => LoginDone(__);
            WebSocketController.Instance.ServerErr += (_, __) => ErrorMessage(__);
        }

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
        private void LoginF()
        {
            SendObject(new  Login()
            {
                UserName = LoginInput,
                Password = PasswordInput
            });
        }


        public void SendObject(object AObject)
        { 
            Output = string.Format("{0}Send: {1}\n\r", Output, JsonConvert.SerializeObject(AObject));
            Notify("Output"); 
            WebSocketController.Instance.OutputQueueAddObject(AObject);
        }

        private void LoginDone(LoginClass AObject)
        { 
            IsBlocked = false;
            Notify("IsBlocked");
            IsError = false;
            Notify("IsError");
            ErrState = "";
            Notify("ErrState");

            Output = string.Format("{0}Get: {1}\n\r", Output, JsonConvert.SerializeObject(AObject));
            Notify("Output");
            if (AObject.Active == ServerLib.JTypes.Enums.UserActive.blocked)
            {
                IsBlocked = true;
                Notify("IsBlocked");
                ErrState = "Пользователь заблокирован";
                Notify("ErrState"); 
            }
            else
            {
                NavigationService.Instance.Navigate(new ClientViewModel());
            }
        }
         

        private void ErrorMessage(string errorInfo)
        {
            IsBlocked = false;
            Notify("IsBlocked");
            IsError = true;
            Notify("IsError");
            ErrState = errorInfo; 
            Notify("ErrState"); 
        }
    }
}
