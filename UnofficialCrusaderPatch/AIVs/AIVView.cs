using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UCP.AIVs
{
    public class AIVView
    {
        public void InitUI(StackPanel panel)
        {
            foreach (AIVChange change in AIVChange.changes)
            {
                change.InitUI();
                TreeView view = new TreeView()
                {
                    Background = null,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Focusable = false,
                };
                view.Items.Add(change.UIElement);
                panel.Children.Add(view);
            }
        }
    }
}
