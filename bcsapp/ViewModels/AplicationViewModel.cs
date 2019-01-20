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
using System.Diagnostics;
using System.Windows.Forms;

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
        public ObservableCollection<UserClass> observableUserClass { set; get; } = new ObservableCollection<UserClass>(DataStorage.Instance.UserList);
        public UserClass SelectedUserClass { set; get; }
        public ObservableCollection<JobClass> observableJobsClass { set; get; } = new ObservableCollection<JobClass>(DataStorage.Instance.JobList);
        public JobClass SelectedJobsClass { set; get; } 

        public AplicationViewModel()
        {
            Stopwatch watcher = Stopwatch.StartNew();

            RibbonCommand = new SimpleCommand<RibbonControl>(UserRibbon);
            UsersGridCommand = new SimpleCommand(OpenUsersGrid);
            JobsGridCommand = new SimpleCommand(OpenJobsGrid);
            RolsGridCommand = new SimpleCommand(OpenRolesGrid);

            AddButtonCommand = new SimpleCommand(AddButton);
            EditButtonCommad = new SimpleCommand(EditButton);
            DeleteButtonCommand = new SimpleCommand(DeleteButton);


            WebSocketController.Instance.UpdateUserUI += (_, __) => Instance_UpdateUserUI(__);
            WebSocketController.Instance.ConnectedState += (_, __) => Instance_ConnectedState(__);
            WebSocketController.Instance.UpdateUsers += (_, __) => Instance_UpdateUsers(__);
            WebSocketController.Instance.UpdateJobs += (_, __) => Instance_UpdateJobs(__);

            Task.Run(() =>
            {
                ServerLib.JTypes.Client.UsersClass usersClass = new ServerLib.JTypes.Client.UsersClass() { Token = DataStorage.Instance.Login.Token };
                WebSocketController.Instance.OutputQueueAddObject(usersClass);
                ServerLib.JTypes.Client.JobsClass jobsClass = new ServerLib.JTypes.Client.JobsClass { Token = DataStorage.Instance.Login.Token };
                WebSocketController.Instance.OutputQueueAddObject(jobsClass);
            });

            var time = watcher.Elapsed;
        }

#region Ribonn Buttons
        private void DeleteButton()
        {
            if (UsersGridShow && SelectedUserClass!=null)
            { 
                if (System.Windows.MessageBox.Show("Удалить пользователя " + SelectedUserClass.FirstName + " " + SelectedUserClass.LastName + "?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {

                }
                else
                {

                }

            }
            if (JobsGridShow && SelectedJobsClass!=null)
            {
                if (System.Windows.MessageBox.Show("Удалить должность " + SelectedJobsClass.Name  +  "?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
                Window window = new Window();
                window.Title = "Редактировать пользователя";
                window.Content = new AddUserViewModel(SelectedUserClass);
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.ShowDialog();
            }
            if (JobsGridShow && SelectedJobsClass != null)
            {
                Window window = new Window();
                window.Title = "Новая должность";
                window.Content = new AddJobsViewModel(SelectedJobsClass);
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.ShowDialog();
            }

        }

        private void AddButton()
        {
            if (UsersGridShow)
            {
                Window window = new Window();
                window.Title = "Новый пользователь";
                window.Content = new AddUserViewModel();
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.ShowDialog();
            }
            if (JobsGridShow)
            {
                Window window = new Window();
                window.Title = "Новая должность";
                window.Content = new AddJobsViewModel();
                window.SizeToContent = SizeToContent.WidthAndHeight;
                window.ShowDialog();
            }
        }
 #endregion

        //обновление данных в таблицах
        private void Instance_UpdateUsers(List<UserClass> users)
        {
            observableUserClass = new ObservableCollection<UserClass>(users);
            Notify("observableUserClass");

        }

        private void Instance_UpdateJobs(List<JobClass> jobs)
        {
            observableJobsClass = new ObservableCollection<JobClass>(jobs);
            Notify("observableJobsClass");
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
