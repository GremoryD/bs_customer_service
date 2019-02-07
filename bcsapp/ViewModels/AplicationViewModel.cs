﻿using bcsapp.Controls;
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
using System.Diagnostics;

namespace bcsapp.ViewModels
{
    public class AplicationViewModel : IViewModel, INotifyPropertyChanged
    {
        public bool FullscreenView { get; set; } = true;
        public bool UserInterface { set; get; } = true;
        public ICommand RibbonCommand { set; get; }

        //Users left menu commands
        public ICommand UsersGridCommand { set; get; }
        public ICommand JobsGridCommand { set; get; }
        public ICommand RolsGridCommand { set; get; }
        public ICommand AddButtonCommand { set; get; }
        public ICommand EditButtonCommad { set; get; }
        public ICommand DeleteButtonCommand { set; get; }
        public ICommand PasswordChangeCommand { set; get; }

        public ICommand UnBlockButtonCommand { set; get; }
        public ICommand BlockCommand { set; get; }

        //GridUSers
        public ICommand UserSelectedItemChangedCommand { set; get; }

        //Ribon Menu Buttons
        public bool SelectedUsers { set; get; } = true;
        public bool SelectedClients { set; get; } = false;
        public bool SelectedSetings { set; get; } = false;

        //RibonData
        public String UserName { set; get; } = "";
        public String ConectedState { set; get; } = "";


        //UsersGridData
        public bool UsersGridShow { set; get; } = true;
        public bool JobsGridShow { set; get; } = false;
        public bool RolesGridShow { set; get; } = false;


        //Data Grid
        public ObservableCollection<ResponseUserClass> observableUserClass { set; get; } = new ObservableCollection<ResponseUserClass>(DataStorage.Instance.UserList);
        public ResponseUserClass SelectedUserClass { set; get; }
        public ObservableCollection<ResponseJobClass> observableJobsClass { set; get; } = new ObservableCollection<ResponseJobClass>(DataStorage.Instance.JobList);
        public ResponseJobClass SelectedJobsClass { set; get; }
        public ObservableCollection<ResponseRoleClass> observableRolesClass { set; get; } = new ObservableCollection<ResponseRoleClass>(DataStorage.Instance.RoleList);
        public ResponseRoleClass SelectedRoleClass { set; get; }


        //roles controle
        public ObservableCollection<ResponseRoleClass> UserUsedRoles { set; get; } = new ObservableCollection<ResponseRoleClass>();
        public ObservableCollection<ResponseRoleClass> UserUnusedRoles { set; get; } = new ObservableCollection<ResponseRoleClass>();

        public ICommand AddRoleToUserCommand { set; get; }
        public ICommand RemoveRoleToUserCommand { set; get; }

        //roles grid controle
        public ObservableCollection<AccessRolesData> accessRolesData { set; get; } = new ObservableCollection<AccessRolesData>(DataStorage.Instance.accessRolesData);
        public ObservableCollection<ResponseRoleClass> observableUsersRole { set; get; } = new ObservableCollection<ResponseRoleClass>(DataStorage.Instance.RoleList);

        public AplicationViewModel()
        {
            RibbonCommand = new SimpleCommand<RibbonControl>(UserRibbon);
            UsersGridCommand = new SimpleCommand(OpenUsersGrid);
            JobsGridCommand = new SimpleCommand(OpenJobsGrid);
            RolsGridCommand = new SimpleCommand(OpenRolesGrid);
            PasswordChangeCommand = new SimpleCommand(PasswordChange);
            AddButtonCommand = new SimpleCommand(AddButton);
            EditButtonCommad = new SimpleCommand(EditButton);
            DeleteButtonCommand = new SimpleCommand(DeleteButton);

            UserSelectedItemChangedCommand = new SimpleCommand(UserSelectedItemChanged);
            AddRoleToUserCommand = new SimpleCommand<ResponseRoleClass>(AddRoleToUser);
            RemoveRoleToUserCommand = new SimpleCommand<ResponseRoleClass>(RemoveRoleToUser);

            WebSocketController.Instance.UpdateUserUI += (_, __) => Application.Current.Dispatcher.Invoke(() => Instance_UpdateUserUI(__));
            WebSocketController.Instance.ConnectedState += (_, __) => Application.Current.Dispatcher.Invoke(() => Instance_ConnectedState(__));
            WebSocketController.Instance.UpdateUsers += (_, __) => Application.Current.Dispatcher.Invoke(() => Instance_UpdateUsers(__));
            WebSocketController.Instance.UpdateJobs += (_, __) => Application.Current.Dispatcher.Invoke(() => Instance_UpdateJobs(__));
            WebSocketController.Instance.UpdateRoles += (_, __) => Application.Current.Dispatcher.Invoke(() => Instance_UpdateRoles(__));

            Task.Run(() =>
            {
                ServerLib.JTypes.Client.RequestUsersClass usersClass = new ServerLib.JTypes.Client.RequestUsersClass() { Token = DataStorage.Instance.Login.Token };
                WebSocketController.Instance.OutputQueueAddObject(usersClass);
                ServerLib.JTypes.Client.RequestJobsClass jobsClass = new ServerLib.JTypes.Client.RequestJobsClass { Token = DataStorage.Instance.Login.Token };
                WebSocketController.Instance.OutputQueueAddObject(jobsClass);
                ServerLib.JTypes.Client.RequestRolesClass rolesClass = new ServerLib.JTypes.Client.RequestRolesClass { Token = DataStorage.Instance.Login.Token };
                WebSocketController.Instance.OutputQueueAddObject(rolesClass);
                ServerLib.JTypes.Client.RequestUsersRolesClass usersRolesClass = new ServerLib.JTypes.Client.RequestUsersRolesClass { Token = DataStorage.Instance.Login.Token };
                WebSocketController.Instance.OutputQueueAddObject(usersRolesClass);
            });

        }

        private void PasswordChange()
        {
            NavigationService.Instance.ShowDialogWin(new PasswordChangeViewModel(SelectedUserClass), "Изменить пароль");
        }

        private void UserSelectedItemChanged()
        {
            UserUsedRoles.Clear();
            foreach (ResponseUserRoleClass userRoleClass in DataStorage.Instance.UsersRolesList)
            {
                if (SelectedUserClass != null && userRoleClass.UserID == SelectedUserClass.ID)
                {
                    UserUsedRoles.Add(DataStorage.Instance.RoleList.Find(x => x.ID == userRoleClass.RoleID));
                    Notify("UserUsedRoles");
                }
            }
            UserUnusedRoles = new ObservableCollection<ResponseRoleClass>(DataStorage.Instance.RoleList.Except(UserUsedRoles));
            Notify("UserUnusedRoles");
            Notify("SelectedUserClass");
        }
         
        private void RemoveRoleToUser(ResponseRoleClass obj)
        {
            bool isDone;
            TransactionService.RemoveRoleToUser(new Transaction(new ServerLib.JTypes.Client.RequestUserRoleDeleteClass()
            {
                UserRoleID = DataStorage.Instance.UsersRolesList.Find(x => x.RoleID == obj.ID).ID,
                Token = DataStorage.Instance.Login.Token
            },
             new Action(() =>
             {
                 Application.Current.Dispatcher.Invoke(() =>
                 {
                     UserUnusedRoles.Add(obj);
                     UserUsedRoles.Remove(obj);
                     Notify("UserUnusedRoles");
                     Notify("UserUsedRoles");
                 });
             }), new Action(() => isDone = false)));


        }

        private void AddRoleToUser(ResponseRoleClass obj)
        {
            bool isDone;
            TransactionService.AddRoleToUser(new Transaction(new ServerLib.JTypes.Client.RequestUserRoleAddClass()
            {
                RoleID = obj.ID,
                UserID = SelectedUserClass.ID,
                Token = DataStorage.Instance.Login.Token
            },
             new Action(() =>
             {
                 Application.Current.Dispatcher.Invoke(() =>
                 {
                     UserUnusedRoles.Remove(obj);
                     UserUsedRoles.Add(obj);
                     Notify("UserUnusedRoles");
                     Notify("UserUsedRoles");
                 });
             }), new Action(() => isDone = false))); ;

        }


        #region Ribonn Buttons
        private void DeleteButton()
        {
            if (UsersGridShow && SelectedUserClass != null)
            {
                if (System.Windows.MessageBox.Show("Удалить пользователя " + SelectedUserClass.FirstName + " " + SelectedUserClass.LastName + "?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
                else
                {

                }

            }
            if (JobsGridShow && SelectedJobsClass != null)
            {
                if (System.Windows.MessageBox.Show("Удалить должность " + SelectedJobsClass.Name + "?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
                else
                {

                }

            }
            if (RolesGridShow && SelectedRoleClass != null)
            {
                if (System.Windows.MessageBox.Show("Удалить роль " + SelectedRoleClass.Name + "?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
                else
                {

                }

            }



        }

        private void EditButton()
        {
            if (UsersGridShow && SelectedUserClass != null)
            {
                NavigationService.Instance.ShowDialogWin(new AddUserViewModel(SelectedUserClass), "Редактировать пользователя");
            }
            if (JobsGridShow && SelectedJobsClass != null)
            {
                NavigationService.Instance.ShowDialogWin(new AddJobsViewModel(SelectedJobsClass), "Новая должность");
            }
            if (RolesGridShow && SelectedRoleClass != null)
            {
                NavigationService.Instance.ShowDialogWin(new AddRolesViewModel(SelectedRoleClass), "Новая должность");
            }

        }
        private void AddButton()
        {
            if (UsersGridShow)
            {
                NavigationService.Instance.ShowDialogWin(new AddUserViewModel(), "Новый пользователь");
            }
            if (JobsGridShow)
            {
                NavigationService.Instance.ShowDialogWin(new AddJobsViewModel(), "Новая должность");
            }
            if (RolesGridShow)
            {
                NavigationService.Instance.ShowDialogWin(new AddRolesViewModel(), "Новая Роль");
            }
        }


        #endregion

        //обновление данных в таблицах
        private void Instance_UpdateUsers(String data)
        {
            observableUserClass = new ObservableCollection<ResponseUserClass>(DataStorage.Instance.UserList);
            Notify("observableUserClass");

        }
        private void Instance_UpdateJobs(String data)
        {
            observableJobsClass = new ObservableCollection<ResponseJobClass>(DataStorage.Instance.JobList);
            Notify("observableJobsClass");
        }

        private void Instance_UpdateRoles(String data)
        {
            observableRolesClass = new ObservableCollection<ResponseRoleClass>(DataStorage.Instance.RoleList);
            Notify("observableRolesClass");
        }
        #region Left Menu Functions 
        //Функции кнопок левого меню
        private void OpenUsersGrid()
        {
            UserInterface = true;
            Notify("UserInterface");
            UsersGridShow = true;
            Notify("UsersGridShow");
        }

        private void OpenJobsGrid()
        {
            UserInterface = false;
            Notify("UserInterface");
            JobsGridShow = true;
            Notify("JobsGridShow");
        }


        private void OpenRolesGrid()
        {
            UserInterface = false;
            Notify("UserInterface");
            RolesGridShow = true;
            Notify("RolesGridShow");

        }

        #endregion

        //вывод состояния соединения и текущего пользователя
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


        //Функция для Нотифая
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Функция переключения рибона
        private void UserRibbon(RibbonControl ribbonControl)
        {
            if (ribbonControl.SelectedPage.Name == "UsersRibbonPage")
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
