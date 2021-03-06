﻿using bcsapp.Controls;
using bcsapp.Models;
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
        public ServerLib.JTypes.Server.ResponseJobClass Job;

        public String JobName { set; get; }
        public String EditAddButton { set; get; }
        public ICommand AddJobCommand { set; get; }
        public ICommand CancelCommand { set; get; }
        public bool FullscreenView { get; set; }

        public AddJobsViewModel()
        {
            AddJobCommand = new SimpleCommand(AddJob);
            EditAddButton = "Сохранить";
            CancelCommand = new SimpleCommand(Cancel);
        }

        private void AddJob()
        {
            TransactionService.AddJob(new Transaction(new ServerLib.JTypes.Client.RequestJobAddClass()
            {
                Name = JobName,
                Token = DataStorage.Instance.Login.Token
            },
            new Action(() => { }), new Action(() => { })));
            Cancel();
        }

        public AddJobsViewModel(ServerLib.JTypes.Server.ResponseJobClass AJob)
        {
            Job = AJob;
            JobName = Job.Name;
            EditAddButton = "Сохранить";
            Notify("EditAddButton");
            AddJobCommand = new SimpleCommand(EditJob);
            CancelCommand = new SimpleCommand(Cancel);
        }

        private void EditJob()
        {
            TransactionService.EditJob(new Transaction(new ServerLib.JTypes.Client.RequestJobEditClass()
            {
                ID = Job.ID,
                Name = JobName,
                Token = DataStorage.Instance.Login.Token
            },
            new Action(() => { }), new Action(() => { })));
            Cancel();
        }

        private void Cancel()
        {
            NavigationService.Instance.CloseDialogWin();
        }

        //Функция для Нотифая
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler OnInitDone;

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
