using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UCP.Balance
{
    public class BalanceView
    {
        public void InitUI(Grid grid, RoutedPropertyChangedEventHandler<object> SelectionDisabler)
        {
            TreeView view = new TreeView()
            {
                Background = null,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Focusable = false,
                Name = "BalanceView"
            };
            view.SelectedItemChanged += SelectionDisabler;

            foreach (BalanceChange change in BalanceChange.changes)
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
            String activeChange = BalanceChange.activeChange == null ? String.Empty : BalanceChange.activeChange.TitleIdent;
            for (int i = 0; i < BalanceChange.changes.Count; i++)
            {
                view.Items.Remove(BalanceChange.changes.ElementAt(i).UIElement);
                Localization.Remove(BalanceChange.changes.ElementAt(i) + "_descr");
            }
            BalanceChange.Refresh(s, e);
            foreach (BalanceChange change in BalanceChange.changes)
            {
                change.InitUI();
                view.Items.Add(change.UIElement);
            }

            if (BalanceChange.changes.Select(x => x.TitleIdent).Contains(activeChange))
            {
                foreach (BalanceChange change in BalanceChange.changes)
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
