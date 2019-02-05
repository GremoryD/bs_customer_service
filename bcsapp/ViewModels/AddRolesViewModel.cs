using bcsapp.Controls;
using bcsapp.Models;
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
    public class AddRolesViewModel : IViewModel, INotifyPropertyChanged
    {
        public String RoleName { set; get; }
        public String RoleDescription { set; get; }
        public String EditAddButton { set; get; }
        public ICommand AddRoleCommand { set; get; }
        public ICommand CancelCommand { set; get; }
        private long  roleID { set; get; }

        //roles controle
        public ObservableCollection<AccessRolesData> accessRolesData { set; get; } = new ObservableCollection<AccessRolesData>(DataStorage.Instance.accessRolesData);


        public AddRolesViewModel()
        {
            EditAddButton = "Добавить";
            AddRoleCommand = new SimpleCommand(AddRole);
            CancelCommand = new SimpleCommand(Cancel);
        }

        private void Cancel()
        {
            NavigationService.Instance.CloseDialogWin();
        }

        private void AddRole()
        {

            ServerLib.JTypes.Client.RequestRoleAddClass rolesClass = new ServerLib.JTypes.Client.RequestRoleAddClass
            {
                Token = DataStorage.Instance.Login.Token,
                Name = RoleName,
                Description = RoleDescription
            };
            WebSocketController.Instance.OutputQueueAddObject(rolesClass);

        }

        public AddRolesViewModel(ResponseRoleClass roleClass)
        {
            roleID = roleClass.ID;
            EditAddButton = "Редактировать";
            RoleName = roleClass.Name;
            RoleDescription = roleClass.Description;
            AddRoleCommand = new SimpleCommand(EditRole);
            CancelCommand = new SimpleCommand(Cancel);
        }

        private void EditRole()
        {
            ServerLib.JTypes.Client.RequestRoleEditClass rolesClass = new ServerLib.JTypes.Client.RequestRoleEditClass
            {
                Token = DataStorage.Instance.Login.Token,
                Name = RoleName,
                ID = roleID,
                Description = RoleDescription
            };
            WebSocketController.Instance.OutputQueueAddObject(rolesClass);

        }


        public bool FullscreenView { get; set; } = false;

        //Функция для Нотифая
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
