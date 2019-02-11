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

                if (viewModels.Last().FullscreenView)
                    currentWindow.WindowState = WindowState.Maximized;
                else
                    currentWindow.WindowState = WindowState.Normal;
            });
        }

        public void Navigate(IViewModel model)
        {
            if (model is LoginViewModel)
            {
                currentWindow.ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                currentWindow.ResizeMode = ResizeMode.CanResize;
            } 

            Application.Current.Dispatcher.Invoke(() =>
            {
                currentWindow.Visibility = Visibility.Collapsed;

                model.OnInitDone += (_, __) =>
                {
                    if (model.FullscreenView)
                    {
                        currentWindow.WindowState = WindowState.Maximized;
                        currentWindow.SizeToContent = SizeToContent.Manual;
                    }
                    else
                    {
                        currentWindow.WindowState = WindowState.Normal;
                        currentWindow.SizeToContent = SizeToContent.WidthAndHeight;
                    }

                    currentWindow.Visibility = Visibility.Visible;
                };

                currentWindow.Content = model;
                viewModels.Add(model);
            });
        }

        Window window;
        public void ShowDialogWin(IViewModel viewModel, string title)
        {
            window = new Window();
            window.Title = title;
            window.WindowStyle = WindowStyle.ToolWindow;
            window.ShowInTaskbar = false;
            window.Owner = currentWindow;
            window.Content = viewModel;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ShowDialog();
        }

        public void CloseDialogWin()
        {
            window.Close();
        }
    }
}
