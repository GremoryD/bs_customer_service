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



        //roles controle
        public ObservableCollection<AccessRolesData> accessRolesData { set; get; } = new ObservableCollection<AccessRolesData>(DataStorage.Instance.accessRolesData);


        public AddRolesViewModel()
        {
            EditAddButton = "Добавить";
            AddRoleCommand = new SimpleCommand(AddRole);
        }

        private void AddRole()
        {

            ServerLib.JTypes.Client.RoleAddClass rolesClass = new ServerLib.JTypes.Client.RoleAddClass { Token = DataStorage.Instance.Login.Token,
                                                                                                         Name = RoleName,
                                                                                                         Description = RoleDescription
                                                                                                        };
            WebSocketController.Instance.OutputQueueAddObject(rolesClass);

        }

        public AddRolesViewModel(RoleClass roleClass)
        {
            EditAddButton = "Редактировать";
            RoleName = roleClass.Name;
            RoleDescription = roleClass.Description;
            AddRoleCommand = new SimpleCommand(EditRole);
        }

        private void EditRole()
        {

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
