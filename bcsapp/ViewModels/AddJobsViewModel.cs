using bcsapp.Controls; 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace bcsapp.ViewModels
{
    public class AddJobsViewModel : IViewModel, INotifyPropertyChanged
    {
        public String JobName { set; get; }
        public String EditAddButton { set; get; }
        public ICommand AddJobCommand { set; get; }
        public bool FullscreenView { get ; set; }

        public AddJobsViewModel()
        {
            AddJobCommand = new SimpleCommand(AddJob);
            EditAddButton = "Добавить";
        }

        private void AddJob()
        {
            ServerLib.JTypes.Client.JobAddClass jobAdd = new ServerLib.JTypes.Client.JobAddClass()
            {
                Name = JobName,
            };

            WebSocketController.Instance.OutputQueueAddObject(jobAdd);
        }

        public AddJobsViewModel(ServerLib.JTypes.Server.JobClass job)
        {
            JobName = job.Name;
            AddJobCommand = new SimpleCommand(EditJob);
            EditAddButton = "Редактировать";
        }

        private void EditJob()
        {
            ServerLib.JTypes.Client.JobEditClass jobAdd = new ServerLib.JTypes.Client.JobEditClass()
            {
                Name = JobName,
            }; 
            WebSocketController.Instance.OutputQueueAddObject(jobAdd); 
        }

        //Функция для Нотифая
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
