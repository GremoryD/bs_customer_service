using bcsapp.Controls;
using bcsapp.Models;
using ServerLib.JTypes.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace bcsapp.ViewModels
{
    public class AddUserViewModel : INotifyPropertyChanged
    {

        public String UserLogin { set; get; }
        public String UserPassword { set; get; }
        public String UserSurname { set; get; }
        public String UserName { set; get; }



        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand AddUserCommand { set; get; }
        public ICommand CancelCommand { set; get; }

        public AddUserViewModel()
        {
            AddUserCommand = new SimpleCommand(AddUser);
            CancelCommand = new SimpleCommand(Cancel);
        }

        private void Cancel()
        { 
            
        }

        private void AddUser()
        {
            UserAddClass addUser = new UserAddClass()
            {
                Login = UserLogin,
                Password = UserPassword,
                FirstName = UserName,
                LastName = UserSurname,
                Active = ServerLib.JTypes.Enums.UserActive.activated,
                JobId = 1,
                Token = DataStorage.Instance.Login.Token
            };

            WebSocketController.Instance.OutputQueueAddObject(addUser);

        }
    }
}
