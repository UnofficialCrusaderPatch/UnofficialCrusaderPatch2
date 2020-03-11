using System.Windows;
using System.Windows.Controls;

namespace UCP.AIV
{
    public class AIVView
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

            foreach (AIVChange change in AIVChange.changes)
            {
                change.InitUI();
                view.Items.Add(change.UIElement);
            }
            grid.Children.Add(view);
        }
    }
}
