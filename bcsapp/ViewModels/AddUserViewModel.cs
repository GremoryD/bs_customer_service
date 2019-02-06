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
    public class AddUserViewModel : IViewModel, INotifyPropertyChanged
    {
        private ResponseUserClass user;

        public String UserLogin { set; get; }
        public String UserPassword { set; get; }
        public String UserSurname { set; get; }
        public String UserName { set; get; }
        public String UserMiddleName { set; get; }
        public String EditAddButton { set; get; }

        public ObservableCollection<ResponseJobClass> observableJobsClass { set; get; } = new ObservableCollection<ResponseJobClass>(DataStorage.Instance.JobList);
        public ResponseJobClass SelectedJob { set; get; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddUserCommand { set; get; }
        public ICommand CancelCommand { set; get; }
        public bool FullscreenView { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AddUserViewModel()
        {
            EditAddButton = "Добваить";
            AddUserCommand = new SimpleCommand(AddUser);
            CancelCommand = new SimpleCommand(Cancel);
        }

        public AddUserViewModel(ResponseUserClass user)
        {
            this.user = user;
            UserLogin = user.Login;
            UserPassword = "";
            UserSurname = user.LastName;
            UserName = user.FirstName;
            UserMiddleName = user.MidleName;
            if (observableJobsClass.Where(x => x.ID == user.ID).Count<ResponseJobClass>()>0)
            {
                SelectedJob = observableJobsClass.Where(x => x.ID == user.ID).Last<ResponseJobClass>();

            }
            EditAddButton = "Изменить";
            AddUserCommand = new SimpleCommand(EditUser);
            CancelCommand = new SimpleCommand(Cancel);
        }


        private void Cancel()
        {
            NavigationService.Instance.CloseDialogWin();
        }


        private void AddUser()
        {
            bool isDone;
            TransactionService.AddUser(new Transaction(new ServerLib.JTypes.Client.RequestUserAddClass()
            {
                Login = UserLogin,
                Password = UserPassword,
                FirstName = UserName,
                LastName = UserSurname,
                MidleName = UserMiddleName,
                Active = ServerLib.JTypes.Enums.UserActive.activated,
                JobID = SelectedJob != null ? SelectedJob.ID : 1,
                Token = DataStorage.Instance.Login.Token
            },
            new Action(() => isDone = true), new Action(() => isDone = false))); 
            Cancel();
        }



        private void EditUser()
        {
            bool isDone;
            TransactionService.EditUser(new Transaction(new ServerLib.JTypes.Client.RequestUserEditClass()
            {
                ID = user.ID,
                FirstName = UserName,
                LastName = UserSurname,
                MidleName = UserMiddleName,
                JobID = SelectedJob != null ? SelectedJob.ID : 1,
                Active = user.Active,
                Token = DataStorage.Instance.Login.Token
            },
            new Action(() => isDone = true), new Action(() => isDone = false)));
            Cancel();   
        }
    }
}
