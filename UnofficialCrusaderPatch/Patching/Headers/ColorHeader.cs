using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnofficialCrusaderPatch
{
    public class ColorHeader : ValueHeader
    {
        public ColorHeader(string descrIdent)
            : base(descrIdent, 4, 1)
        {
        }

        static readonly Dictionary<int, Color> Colors = new Dictionary<int, Color>()
        {
            { 4, Color.FromArgb(255, 240, 32, 0) }, // red
            { 2, Color.FromArgb(255, 248, 120, 0) }, // orange
            { 3, Color.FromArgb(255, 224, 224, 0) }, // yellow
            { 1, Color.FromArgb(255, 0, 0, 248) }, // blue
            { 5, Color.FromArgb(255, 80, 80, 80) }, // black
            { 6, Color.FromArgb(255, 184, 0, 248) }, // purple
            { 7, Color.FromArgb(255, 0, 208, 240) }, // light blue
            { 8, Color.FromArgb(255, 0, 216, 0) } // green
        };

        Image focus;

        protected override FrameworkElement CreateUI()
        {
            Grid grid = new Grid()
            {
                Width = 410,
                Height = 30,
            };

            focus = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/UnofficialCrusaderPatch;component/Graphics/shield.png")),
                Margin = new Thickness(7, 4, 0, 0),
                Width = 22,
                Height = 22,
            };

            Canvas canvas = new Canvas();
            canvas.Children.Add(focus);

            grid.Children.Add(canvas);

            for (int i = 0; i < 8; i++)
            {
                var pair = Colors.ElementAt(i);

                Border button = new Border()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(i * 53, 0, 0, 0),
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    BorderBrush = Brushes.Black,
                    Background = new SolidColorBrush(pair.Value),
                    Height = 30,
                    Width = 38,
                };

                button.MouseUp += (s, e) => SetFocus(button, pair.Key);

                grid.Children.Insert(0, button);

                if (pair.Key == this.Value)
                    SetFocus(button, pair.Key);
            }

            return grid;
        }
        
        void SetFocus(Border button, int value)
        {
            this.SetValue(value);
            Canvas.SetLeft(focus, button.Margin.Left);
        }
    }
}
