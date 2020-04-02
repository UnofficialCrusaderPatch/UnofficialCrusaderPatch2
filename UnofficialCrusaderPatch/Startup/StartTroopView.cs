using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UCP.Startup
{
    public class StartTroopView
    {
        public void InitUI(Grid grid, RoutedPropertyChangedEventHandler<object> SelectionDisabler)
        {
            TreeView view = new TreeView()
            {
                Background = null,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Focusable = false,
            };
            view.SelectedItemChanged += SelectionDisabler;

            foreach (StartTroopChange change in StartTroopChange.changes)
            {
                change.InitUI();
                view.Items.Add(change.UIElement);
            }
            grid.Children.Add(view);
            Button button = new Button
            {
                ToolTip = "Reload configs",
                Width = 20,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 20, 5),
                Content = new Image()
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/UnofficialCrusaderPatchGUI;component/Graphics/refresh.png")),
                }
            };
            grid.Children.Add(button);
            button.Click += (s, e) => Refresh(s, e, view);
        }

        private void Refresh(object s, RoutedEventArgs e, TreeView view)
        {
            String activeChange = StartTroopChange.activeChange == null ? String.Empty : StartTroopChange.activeChange.TitleIdent;
            for (int i = 0; i < StartTroopChange.changes.Count; i++)
            {
                view.Items.Remove(StartTroopChange.changes.ElementAt(i).UIElement);
                Localization.Remove(StartTroopChange.changes.ElementAt(i).TitleIdent.Substring(4) + "_descr");
            }
            StartTroopChange.Refresh(s, e);
            foreach (StartTroopChange change in StartTroopChange.changes)
            {
                change.InitUI();
                view.Items.Add(change.UIElement);
            }

            if (StartTroopChange.changes.Select(x => x.TitleIdent).Contains(activeChange))
            {
                foreach (StartTroopChange change in StartTroopChange.changes)
                {
                    if (change.TitleIdent == activeChange)
                    {
                        change.IsChecked = true;
                    }
                }
            }
        }
    }
}
