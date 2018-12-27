using bcs.Models;
using CLProject;
using ServerLib.JTypes.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace bcs.ViewModels
{
    public class LoginViewModel
    {
        public String LoginInput { get; set; }
        public String PasswordInput { get; set; }

        public ICommand LoginCommand { get; set; }


        public LoginViewModel()
        {
            LoginCommand = new SimpleCommand(LoginF); 
        }

        private void LoginF()
        {
            Login login = new Login()
            {
                Password = PasswordInput,
                UserName = LoginInput
            };
            Singleton.instance.SendLogin(login);

        }

        

    }
}
