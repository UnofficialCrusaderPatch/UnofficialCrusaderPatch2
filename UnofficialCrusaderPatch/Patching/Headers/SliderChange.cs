using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UnofficialCrusaderPatch
{
    public class SliderHeader : ChangeHeader
    {
        double min, max, oriVal, defaultVal;

        public SliderHeader(string descrIdent, double min, double max, double oriVal, double defaultVal)
            : base(descrIdent)
        {
            this.min = min;
            this.max = max;
            this.oriVal = oriVal;
            this.defaultVal = defaultVal;
        }
    }

    /*protected override UIElement CreateUIContent()
    {
        Grid grid = new Grid
        {
            Width = 390,
            Height = 40,
        };

        TextBlock textBlock = (TextBlock)base.CreateUIContent();
        textBlock.HorizontalAlignment = HorizontalAlignment.Left;
        textBlock.VerticalAlignment = VerticalAlignment.Top;
        grid.Children.Add(textBlock);

        sliderText = new TextBlock()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(152, 18, 0, 0),
        };
        grid.Children.Add(sliderText);

        slider = new Slider()
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Margin = new Thickness(0, 18, 0, 0),
            IsSnapToTickEnabled = true,
            IsEnabled = IsChecked,
            Width = 150,
        };
        slider.ValueChanged += ValueChanged;
        grid.Children.Add(slider);

        return grid;
    }*/
}
