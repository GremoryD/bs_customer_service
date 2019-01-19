using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcsapp.ViewModels
{
    public class AddJobsViewModel : INotifyPropertyChanged
    {
        public AddJobsViewModel()
        {

        }
        public AddJobsViewModel(ServerLib.JTypes.Server.JobClass job)
        {

        }

        //Функция для Нотифая
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
