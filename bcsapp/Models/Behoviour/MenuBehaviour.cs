using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace bcsapp.Models.Behoviour
{
    public static class MenuBehaviour
    {
        public static readonly DependencyProperty MenuHeightProperty
            = DependencyProperty.RegisterAttached("MenuHeight", typeof(double), typeof(MenuBehaviour), new UIPropertyMetadata(50d));

        #region Property

        public static double GetMenuHeight(DependencyObject source)
        {
            return (double)source.GetValue(MenuHeightProperty);
        }

        public static void SetMenuHeight(DependencyObject source, double value)
        {
            source.SetValue(MenuHeightProperty, value);
        }

        #endregion

        public static readonly DependencyProperty MenuWidthProperty
            = DependencyProperty.RegisterAttached("MenuWidth", typeof(double), typeof(MenuBehaviour), new UIPropertyMetadata(120d));

        #region Property

        public static double GetMenuWidth(DependencyObject source)
        {
            return (double)source.GetValue(MenuWidthProperty);
        }

        public static void SetMenuWidth(DependencyObject source, double value)
        {
            source.SetValue(MenuWidthProperty, value);
        }

        #endregion

        public static readonly DependencyProperty IsOpenedProperty
            = DependencyProperty.RegisterAttached("IsOpened", typeof(bool), typeof(MenuBehaviour), new UIPropertyMetadata(false, new PropertyChangedCallback(IsOpenedChanged)));

        #region Property

        public static bool GetIsOpened(DependencyObject source)
        {
            return (bool)source.GetValue(IsOpenedProperty);
        }

        public static void SetIsOpened(DependencyObject source, bool value)
        {
            source.SetValue(IsOpenedProperty, value);
        }

        #endregion

        private static void IsOpenedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Grid grd)
            {
                if ((bool)e.NewValue)
                {
                    var animation = new DoubleAnimation(GetMenuHeight(d) + GetMenuWidth(d), new Duration(TimeSpan.FromMilliseconds(300)));
                    grd.BeginAnimation(Grid.WidthProperty, animation);
                }
                else
                {
                    var animation = new DoubleAnimation(GetMenuHeight(d), new Duration(TimeSpan.FromMilliseconds(300)));
                    grd.BeginAnimation(Grid.WidthProperty, animation);
                }
            }
        }
    }
}
