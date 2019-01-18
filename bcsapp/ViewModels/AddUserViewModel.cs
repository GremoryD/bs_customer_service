using bcsapp.Controls;
using bcsapp.Models;
using ServerLib.JTypes.Client;
using ServerLib.JTypes.Server;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace bcsapp.ViewModels
{
    public class AddUserViewModel : INotifyPropertyChanged
    {
        private UserClass user;

        public String UserLogin { set; get; }
        public String UserPassword { set; get; }
        public String UserSurname { set; get; }
        public String UserName { set; get; }
        public String UserMiddleName { set; get; }
        public String EditAddButton { set; get; }

        public ObservableCollection<JobClass> observableJobsClass { set; get; } = new ObservableCollection<JobClass>(DataStorage.Instance.JobList);
        public JobClass SelectedJob { set; get; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddUserCommand { set; get; }
        public ICommand CancelCommand { set; get; }

        public AddUserViewModel()
        {
            EditAddButton = "Добваить";
            AddUserCommand = new SimpleCommand(AddUser);
            CancelCommand = new SimpleCommand(Cancel);
        }

        public AddUserViewModel(UserClass user)
        {
            this.user = user;
            UserLogin = user.Login;
            UserPassword = "";
            UserSurname = user.LastName;
            UserName = user.FirstName;
            UserMiddleName = user.MidleName;
            if (observableJobsClass.Where(x => x.Id == user.UserID).Count<JobClass>()>0)
            {
                SelectedJob = observableJobsClass.Where(x => x.Id == user.UserID).Last<JobClass>();

            }
            EditAddButton = "Изменить";
            AddUserCommand = new SimpleCommand(EditUser);
            CancelCommand = new SimpleCommand(Cancel);
        }

        private void Cancel()
        { 
            
        }

        private void AddUser()
        {
            ServerLib.JTypes.Client.UserAddClass addUser = new ServerLib.JTypes.Client.UserAddClass()
            {
                Login = UserLogin,
                Password = UserPassword,
                FirstName = UserName,
                LastName = UserSurname,
                MidleName = UserMiddleName,
                Active = ServerLib.JTypes.Enums.UserActive.activated,
                JobId = SelectedJob.Id,
                Token = DataStorage.Instance.Login.Token
            };

            WebSocketController.Instance.OutputQueueAddObject(addUser);

        }



        private void EditUser()
        {
            ServerLib.JTypes.Client.UserEditClass addUser = new ServerLib.JTypes.Client.UserEditClass()
            {
                UserID = user.UserID,
                FirstName = UserName,
                LastName = UserSurname,
                MidleName = UserMiddleName,
                
                JobId = SelectedJob.Id,
                Active = user.Active,
                Token = DataStorage.Instance.Login.Token
            };

            WebSocketController.Instance.OutputQueueAddObject(addUser);

        }
    }
}
