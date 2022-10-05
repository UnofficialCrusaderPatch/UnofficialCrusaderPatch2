using System;
using System.Windows;
using System.Windows.Controls;

namespace UCP.Patching
{
    public class SliderHeader : ValueHeader
    {
        private double min, max, delta;

        public SliderHeader(string descrIdent, bool isEnabled, double min, double max, double delta, double oriVal, double suggested)
            : base(descrIdent, isEnabled, oriVal, suggested)
        {
            this.min = min;
            this.max = max;
            this.delta = delta;
        }

        private Slider    slider;
        private TextBlock sliderText;

        public override void SetUIEnabled(bool enabled)
        {
            base.SetUIEnabled(enabled);
            slider.IsEnabled = enabled;
        }

        protected override FrameworkElement CreateUI()
        {
            const int sliderWidth = 260;
            const int sliderHeight = 18;

            Grid grid = new Grid
            {
                Width = 300,
                Height = sliderHeight,
            };

            slider = new Slider
                     {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, 0, 0, 0),
                Width = sliderWidth,
                Height = sliderHeight,

                Value = Value,
                IsSnapToTickEnabled = true,
                TickFrequency = delta,
                Minimum = min,
                Maximum = max,
            };
            slider.ValueChanged += Slider_ValueChanged;
            OnValueChange += SliderHeader_OnValueChange;
            grid.Children.Add(slider);

            sliderText = new TextBlock
                         {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(sliderWidth + 5, 0, 0, 0),
                Text = Value.ToString(),
                Height = sliderHeight,
            };
            grid.Children.Add(sliderText);

            return grid;
        }

        private void SliderHeader_OnValueChange()
        {
            if (Math.Abs(Value - slider.Value) < 0.00001)
            {
                return;
            }

            slider.Value = Value;
            sliderText.Text = Value.ToString();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Math.Abs(Value - slider.Value) < 0.00001)
            {
                return;
            }

            SetValue(slider.Value);
            sliderText.Text = Value.ToString();
        }
    }
}
