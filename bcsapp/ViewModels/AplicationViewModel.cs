using bcsapp.Controls;
using DevExpress.Mvvm;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using ServerLib.JTypes.Server;
using System.Collections.ObjectModel;
using bcsapp.Models;

namespace bcsapp.ViewModels
{
    public class AplicationViewModel : IViewModel, INotifyPropertyChanged
    {
        public ICommand RibbonCommand { set; get; } 


        public ICommand UsersGridCommand { set; get; }
        public ICommand RolesGridCommand { set; get; }

        //Ribon Menu Buttons
        public bool SelectedUsers { set; get; } = true;
        public bool SelectedClients { set; get; } = false;
        public bool SelectedSetings { set; get; } = false;

        //RibonData
        public String UserName { set; get; } = "";
        public String ConectedState { set; get; } = "";


        //UsersGridData
        public bool UsersGridShow { set; get; } = true;
        public bool RolesGridShow { set; get; } = false;


        //Data Grid
        public ObservableCollection<UserClass> observableUserClass { set; get; } = new ObservableCollection<UserClass>(DataStorage.Instance.UserList);
        public ObservableCollection<JobClass> observableRolesClass { set; get; } = new ObservableCollection<JobClass>(DataStorage.Instance.RolesList);

        public AplicationViewModel()
        {
            RibbonCommand = new SimpleCommand<RibbonControl>(UserRibbon);
            UsersGridCommand = new SimpleCommand(OpenUsersGrid);
            RolesGridCommand = new SimpleCommand(OpenRolesGrid);
            WebSocketController.Instance.UpdateUserUI += (_, __) => Instance_UpdateUserUI(__);
            WebSocketController.Instance.ConnectedState += (_, __) => Instance_ConnectedState(__);
            WebSocketController.Instance.UpdateUsers += (_, __) => Instance_UpdateUsers(__);

            ServerLib.JTypes.Client.UsersClass usersClass = new ServerLib.JTypes.Client.UsersClass() {Token = DataStorage.Instance.Login.Token };
            WebSocketController.Instance.OutputQueueAddObject(usersClass);
        }

        private void Instance_UpdateUsers(List<UserClass> users)
        {
            observableUserClass = new ObservableCollection<UserClass>(users);
        }

        private void OpenUsersGrid()
        {
            UsersGridShow = true;
            Notify("UsersGridShow");
        }

        private void OpenRolesGrid()
        {
            RolesGridShow = true;
            Notify("RolesGridShow");
        }

        private void Instance_ConnectedState(string value)
        {
            UserName = value;
            Notify("UserName");
        }

        private void Instance_UpdateUserUI(string value)
        {
            ConectedState = value;
            Notify("ConectedState");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UserRibbon(RibbonControl  ribbonControl)
        {
            if(ribbonControl.SelectedPage.Name == "UsersRibbonPage")
            { 
                SelectedUsers = true;
                Notify("SelectedUsers");
            }
            if (ribbonControl.SelectedPage.Name == "ClientsRibbonPage")
            { 
                SelectedClients = true;
                Notify("SelectedClients");
            }
            if (ribbonControl.SelectedPage.Name == "SetingsRibbonPage")
            {
                SelectedSetings = true;
                Notify("SelectedSetings"); 
            }

        }

    }
}
