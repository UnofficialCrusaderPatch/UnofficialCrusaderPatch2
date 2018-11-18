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
            : base(descrIdent, false, 1, 1)
        {
        }

        static readonly List<Color> Colors = new List<Color>()
        {
            Color.FromArgb(255, 240, 32, 0), // red
            Color.FromArgb(255, 248, 120, 0), // orange
            Color.FromArgb(255, 224, 224, 0), // yellow
            Color.FromArgb(255, 0, 0, 248), // blue
            Color.FromArgb(255, 80, 80, 80), // black
            Color.FromArgb(255, 184, 0, 248), // purple
            Color.FromArgb(255, 0, 208, 240), // light blue
            Color.FromArgb(255, 0, 216, 0) // green
        };

        Image focus;
        const int ButtonSpacing = 53;

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
                Border button = new Border()
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(i * ButtonSpacing, 0, 0, 0),
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    BorderBrush = Brushes.Black,
                    Background = new SolidColorBrush(Colors[i]),
                    Height = 30,
                    Width = 38,
                };

                int value = i + 1;
                button.MouseUp += (s, e) => SetValue(value);

                grid.Children.Insert(0, button);
            }

            this.OnValueChange += ValueChange;
            SetValue(1);

            return grid;
        }

        void ValueChange()
        {
            Canvas.SetLeft(focus, ButtonSpacing * (this.Value - 1));
        }
    }
}
