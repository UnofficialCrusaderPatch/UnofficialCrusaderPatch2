using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UCP.Views.Utils
{
    static class Utility
    {
        public static void SetAutoSaveCallback(FrameworkElement parent, Data.MainViewModel vm)
        {
            Utility.FindLogicalChildren<CheckBox>(parent).ToList().ForEach(x =>
            {
                x.Click += vm.WindowClicked;
                /*x.Unchecked += vm.WindowClicked;*/
            }
            );

            Utility.FindLogicalChildren<RadioButton>(parent).ToList().ForEach(x =>
            {
                x.Click += vm.WindowClicked;
                /*x.Checked += vm.WindowClicked;
                x.Unchecked += vm.WindowClicked;*/
            }
            );

            Utility.FindLogicalChildren<Slider>(parent).ToList().ForEach(x =>
            {
                x.ValueChanged += vm.WindowClicked;
                x.ValueChanged += vm.WindowClicked;
            }
            );
        }


        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static IEnumerable<T> FindLogicalChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                foreach (DependencyObject child in LogicalTreeHelper.GetChildren(depObj).OfType<DependencyObject>())
                {
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindLogicalChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
