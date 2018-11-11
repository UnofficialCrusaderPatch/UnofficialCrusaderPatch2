using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace UnofficialCrusaderPatch
{
    class SliderChange : BinaryChange
    {
        public SliderChange(string locIdent, ChangeType type, bool checkedDefault, double min, double max, double freq, double defaultValue)
            : base(locIdent, type, checkedDefault)
        {
            slider.Minimum = min;
            slider.Maximum = max;
            slider.TickFrequency = freq;
            slider.Value = defaultValue;
            this.defaultValue = defaultValue;
        }

        Slider slider;
        double setValue;
        double defaultValue;
        TextBlock sliderText;

        public override bool IsChecked
        {
            get => base.IsChecked;
            set
            {
                base.IsChecked = value;
                slider.Value = this.defaultValue;
                slider.IsEnabled = value;
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

        void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.setValue = e.NewValue;
            sliderText.Text = e.NewValue.ToString();
        }

        public override void Edit(byte[] data, byte[] oriData)
        {
            foreach (BinaryEdit edit in this)
                foreach (BinElement element in edit)
                {
                    if (element is BinProduct prod)
                        prod.Multiply(setValue);
                }

            base.Edit(data, oriData);
        }
    }

    class BinProduct : BinInt32
    {
        double oriValue;
        public BinProduct(double input) : base((int)input)
        {
            oriValue = input;
        }

        public void Multiply(double p)
        {
            base.editData = BitConverter.GetBytes((int)(p * oriValue));
        }
    }
}
