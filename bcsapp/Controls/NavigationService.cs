using bcsapp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace bcsapp.Controls
{
    public class NavigationService
    {

        private List<IViewModel> viewModels = null;
        private Window currentWindow = null;

        private static NavigationService _instance;
        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NavigationService();
                return _instance;
            }
        }


        public NavigationService()
        { 

        }

        public void RegisterWindow(Window window)
        {
            currentWindow = window;
            viewModels = new List<IViewModel>();
            viewModels.Add(window.Content as IViewModel);
        }

        public void UnregisterWindow()
        {
            currentWindow = null;
            viewModels = null;
        }

        public void GoBack()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                viewModels.Remove(viewModels.Last());
                currentWindow.Content = viewModels.Last();
            });

        }

        public void Navigate(IViewModel model)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                currentWindow.Content = model;
                viewModels.Add(model);
            });
        }

    }
}
