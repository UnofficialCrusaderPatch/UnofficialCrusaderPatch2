using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UCP.Startup
{
    public class ResourceView
    {
        public void InitUI(Grid grid, RoutedPropertyChangedEventHandler<object> SelectionDisabler)
        {
            TreeView view = new TreeView()
            {
                Background = null,
                BorderThickness = new Thickness(0, 0, 0, 0),
                Focusable = false,
                Name = "ResourceView"
            };
            view.SelectedItemChanged += SelectionDisabler;

            foreach (ResourceChange change in ResourceChange.changes)
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
            String activeChange = ResourceChange.activeChange == null ? String.Empty : ResourceChange.activeChange.TitleIdent;
            for (int i = 0; i < ResourceChange.changes.Count; i++)
            {
                view.Items.Remove(ResourceChange.changes.ElementAt(i).UIElement);
                Localization.Remove(ResourceChange.changes.ElementAt(i).TitleIdent.Substring(4) + "_descr");
            }
            ResourceChange.Refresh(s, e);
            foreach (ResourceChange change in ResourceChange.changes)
            {
                change.InitUI();
                view.Items.Add(change.UIElement);
            }

            if (ResourceChange.changes.Select(x => x.TitleIdent).Contains(activeChange))
            {
                foreach (ResourceChange change in ResourceChange.changes)
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
