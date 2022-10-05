using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UCP.AIV
{
    public class AIVView
    {
        public static void InitUI(Grid grid, RoutedPropertyChangedEventHandler<object> SelectionDisabler)
        {
            TreeView view = new TreeView
                            {
                Background = null,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Focusable = false,
            };
            view.SelectedItemChanged += SelectionDisabler;

            foreach (AIVChange change in AIVChange.changes)
            {
                change.InitUI();
                view.Items.Add(change.UIElement);
            }
            grid.Children.Add(view);

            Button button = new Button
            {
                ToolTip = "Reload AIV",
                Width = 20,
                Height = 20,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 20, 5),
                Content = new Image
                          {
                    Source = new BitmapImage(new Uri("pack://application:,,,/UnofficialCrusaderPatchGUI;component/Graphics/refresh.png")),
                }
            };
            grid.Children.Add(button);
            button.Click += (s, e) => Refresh(s, e, view);
        }

        private static void Refresh(object s, RoutedEventArgs e, TreeView view)
        {
            String activeChange = AIVChange.activeChange == null ? String.Empty : AIVChange.activeChange.TitleIdent;
            foreach (AIVChange change in AIVChange.changes)
            {
                Localization.Remove(change.TitleIdent + "_descr");
                view.Items.Remove(change.UIElement);
            }
            AIVChange.Refresh();
            foreach (AIVChange change in AIVChange.changes)
            {
                change.InitUI();
                change.IsChecked = false;
                view.Items.Add(change.UIElement);
            }

            if (!AIVChange.changes.Select(x => x.TitleIdent).Contains(activeChange))
            {
                return;
            }

            {
                foreach (AIVChange change in AIVChange.changes.Where(change => change.TitleIdent == activeChange))
                {
                    change.IsChecked = true;
                }
            }
        }
    }
}
