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
using System.Windows.Threading;

namespace bcsapp.ViewModels
{
    public class LoginViewModel : IViewModel, INotifyPropertyChanged
    {
        public bool FullscreenView { get; set; } = false;
        //Вывод данных на форму
        public String LoginInput { get; set; }
        public String PasswordInput { get; set; }
        public String Output { set; get; }
        public String MessageState { get; set; }
        //отображения загрузки и ошибки
        public Boolean IsWait { get; set; }
        public Boolean IsError { get; set; }
        public Boolean IsBlocked { get; set; }

        //команды 
        public ICommand LoginCommand { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;

        public LoginViewModel()
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;

            LoginCommand = new SimpleCommand(LoginF, (_) =>
            {
                if (LoginInput == null || PasswordInput == null)
                    return false;

                return LoginInput.Length >= 1 && PasswordInput.Length >=1;
            });

            WebSocketController.Instance.LoginFailed += (_, __) => Application.Current.Dispatcher.Invoke(() => ErrorMessage(__));
            WebSocketController.Instance.LoginDone += (_, __) => Application.Current.Dispatcher.Invoke(() => LoginDone(__));
            WebSocketController.Instance.ServerErr += (_, __) => Application.Current.Dispatcher.Invoke(() => ErrorMessage(__));
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
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;
            ClearMessage();
            Output = string.Format("{0}Get: {1}\n\r", Output, JsonConvert.SerializeObject(AObject));
            Notify("Output");
            if (AObject.Active == ServerLib.JTypes.Enums.UserActive.blocked)
            {
                BlockMessage("Пользователь заблокирован");
            }
            else
            {
                 NavigationService.Instance.Navigate(new AplicationViewModel()); 
            }
        }



        private void ClearMessage()
        {
            IsBlocked = false;
            Notify("IsBlocked");
            IsError = false;
            Notify("IsError");
            MessageState = "";
            Notify("MessageState");
        }

        private void ErrorMessage(string Message)
        {
            IsBlocked = false;
            Notify("IsBlocked");
            IsError = true;
            Notify("IsError");
            MessageState = Message; 
            Notify("MessageState"); 
        }

        private void BlockMessage(string Message)
        {
            IsError = false;
            Notify("IsError");
            IsBlocked = true;
            Notify("IsBlocked");
            MessageState = Message;
            Notify("MessageState");
        }
    }
}
