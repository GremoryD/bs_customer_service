using bcsapp.Controls;
using bcsapp.Models;
using bcsapp.ViewModels;
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

        #region Commands

        public ICommand SwitchMenuCommand { get; set; }

        #endregion
    }
}
