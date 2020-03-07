using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace UCP.AIC
{
    public class AICView
    {
        public void InitUI(StackPanel panel)
        {
            Button button = new Button
            {
                ToolTip = "Reload .aics",
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
            panel.Children.Add(button);
            //button.Click += (s, e) => AICChange.RefreshLocalFiles();
            //grid.Children.Add(button);
        }
    }
}
