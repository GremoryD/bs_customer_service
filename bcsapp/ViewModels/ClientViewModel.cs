using bcsapp.Controls;
using bcsapp.Models;
using bcsapp.ViewModels;
using ServerLib.JTypes.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace bcsapp.ViewModels
{
    public class ClientViewModel : IViewModel, INotifyPropertyChanged
    {
        //RibonData
        public String UserName { set; get; } = "";
        public String ConectedState { set; get; } = "";

        #region Commands

        public ICommand OpenUsersCommand { set; get; }
        public ICommand OpenClientsCommand { set; get; }
        public ICommand OpenSettingsCommand { set; get; }
        public ICommand LogOutCommand { set; get; } 
        public ICommand SwitchMenuCommand { get; set; }

        #endregion
        public bool MenuOpened { get; set; } = false;

        public IViewModel CurrentContent { get; set; } = new UserViewModel();

        public ClientViewModel()
        {
            Notify("MenuOpened");
            SwitchMenuCommand = new SimpleCommand(() =>
            {
                MenuOpened = !MenuOpened;
                Notify("MenuOpened");
            }); 
            WebSocketController.Instance.UpdateUserUI += (_, __) => Instance_UpdateUserUI(__);
            WebSocketController.Instance.ConnectedState += (_, __) => Instance_ConnectedState(__);


            //кнопки меню
            OpenUsersCommand = new SimpleCommand(OpenUsers);
            OpenClientsCommand = new SimpleCommand(OpenClients);
            OpenSettingsCommand = new SimpleCommand(OpenSettings);
            LogOutCommand = new SimpleCommand(LogOutC);
        }

        private void LogOutC()
        {
            WebSocketController.Instance.OutputQueueAddObject(new LogoutClass());
            NavigationService.Instance.Navigate(new LoginViewModel());
            DataStorage.Instance.ClearData();
        }

        private void OpenSettings()
        {
            CurrentContent = new SettingsViewModel();
            Notify("CurrentContent");

        }

        private void OpenClients()
        {
            CurrentContent = new CustomersViewModel();
            Notify("CurrentContent");
        }

        private void OpenUsers()
        {
            CurrentContent = new UserViewModel();
            Notify("CurrentContent");
        }

        private void Instance_UpdateUserUI(string value)
        {
            UserName = value;
            Notify("UserName");

        }

        private void Instance_ConnectedState( string value)
        {
            ConectedState = value;
            Notify("ConectedState");
        }

        #region Notify

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
