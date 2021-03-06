﻿using System;
using System.Windows;
using bcsapp.Controls;
using Newtonsoft.Json; 

namespace bcsapp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public  partial class MainWindow : Window
    { 

        public MainWindow()
        {
            InitializeComponent(); 
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.RegisterWindow(this);
            WebSocketController.Instance.Start();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            NavigationService.Instance.UnregisterWindow();
            WebSocketController.Instance.Stop();
        }

         

        private void Button_Click(object sender, RoutedEventArgs e)
        { 
        }

        public void SendObject(object AObject)
        {  
        }

        private void LoginError_Click(object sender, RoutedEventArgs e)
        { 
        }
         

        private void LoginBan_Click(object sender, RoutedEventArgs e)
        { 

        }
    }
}
