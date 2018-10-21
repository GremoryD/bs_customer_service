﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CLProject;

namespace bcs
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string AMessage1 = "Приложение загружено";
        private const string AMessage2 = "Приложение запущено";
        private const string AMessage3 = "Приложение остановлено";
        private const string AMessage4 = "Приложение выгружено";

        private ProjectClass Project;

        public MainWindow()
        {
            InitializeComponent();
            Project = new ProjectClass("Application");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = string.Format("{0} v{1}", Title, ProgramInfo.Version);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Project.Dispose();
        }
    }
}
