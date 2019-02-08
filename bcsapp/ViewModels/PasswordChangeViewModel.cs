using ServerLib.JTypes.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcsapp.ViewModels
{
    public class PasswordChangeViewModel : IViewModel, INotifyPropertyChanged
    {
        public ResponseUserClass SelectedUserClass { set; get; }

        public PasswordChangeViewModel(ResponseUserClass userClass)
        {
            SelectedUserClass = userClass;
        }



        public bool FullscreenView { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnInitDone;

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
