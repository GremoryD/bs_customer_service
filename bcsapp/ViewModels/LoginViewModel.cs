﻿using bcsapp.Controls; 
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
using System.Timers;
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

        //команды 
        public ICommand LoginCommand { get; set; }



        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnInitDone; 

        public LoginViewModel()
        {
            var id = System.Threading.Thread.CurrentThread.ManagedThreadId;

            LoginCommand = new SimpleCommand(LoginF, (_) =>
            {
                if (LoginInput == null || PasswordInput == null)
                    return false;

                return LoginInput.Length >= 1 && PasswordInput.Length >=1;
            });

            WebSocketController.Instance.LoginFailed += OnLoginFailed;
            WebSocketController.Instance.LoginDone += OnLoginDone;
            WebSocketController.Instance.ServerErr += OnErrorMessage;
        }

        private void OnErrorMessage(object sender, string e)
        {
            Application.Current.Dispatcher.Invoke(() => ErrorMessage(e));
        }

        private void OnLoginDone(object sender, ResponseLoginClass e)
        {
            Application.Current.Dispatcher.Invoke(() => LoginDone(e));
        }

        private void OnLoginFailed(object sender, string e)
        {
            Application.Current.Dispatcher.Invoke(() => ErrorMessage(e));
        }

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnSubscribed()
        {

            WebSocketController.Instance.LoginFailed -= OnLoginFailed;
            WebSocketController.Instance.LoginDone -= OnLoginDone;
            WebSocketController.Instance.ServerErr -= OnErrorMessage;
        }

        
        private void LoginF()
        {
            Models.DataStorage.Instance.LoginIN = new RequestLoginClass()
            {
                UserName = LoginInput,
                Password = PasswordInput
            }; 
            WebSocketController.Instance.OutputQueueAddObject(Models.DataStorage.Instance.LoginIN);

        }
         

        private void LoginDone(ResponseLoginClass AObject)
        { 
            ClearMessage(); 
            if (AObject.Active == ServerLib.JTypes.Enums.UserActive.blocked)
            {
                BlockMessage("Пользователь заблокирован");
            }
            else
            {
                OnSubscribed();
                    NavigationService.Instance.Navigate(new AplicationViewModel());
            }
        }



        private void ClearMessage()
        { 
            MessageState = "";
            Notify("MessageState");
        }

        private static TimeSpan timeout = TimeSpan.FromSeconds(3);
        private void ErrorMessage(string Message)
        { 
            MessageState = Message;
            Notify("MessageState");
            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { ClearMessage(); };
            timer.Start(); 

        }

        private void BlockMessage(string Message)
        { 
            MessageState = Message;
            Notify("MessageState");
            var timer = new Timer(timeout.TotalMilliseconds) { AutoReset = false };
            timer.Elapsed += (_, __) => { ClearMessage(); };
            timer.Start();
        }
    }
}
