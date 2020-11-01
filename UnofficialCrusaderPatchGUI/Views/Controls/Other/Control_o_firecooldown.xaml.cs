using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.Other
{
    /// <summary>
    /// Interaction logic for Control_o_firecooldown.xaml
    /// </summary>
    public partial class Control_o_firecooldown : UserControl
    {
        private string Identifier = "o_firecooldown";
        public Control_o_firecooldown()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => o_firecooldown.IsChecked == true)
                    .withSubControlConfig(new List<SubControlConfig> {
                        new SubControlConfig()
                            .withIdentifier("o_firecooldown_slider")
                            .withSetEnabled((bool value) => o_firecooldown.IsChecked = value)
                            .withEnabled(() => o_firecooldown.IsChecked == true)
                            .withValue(() => o_firecooldown_slider.Value)
                            .withSetter((double value) => o_firecooldown_slider.Value = value)
                    })
                );
        }

        void OnSliderChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as Slider).Value != 2000)
            {
                o_firecooldown.IsChecked = true;
            }
            else
            {
                o_firecooldown.IsChecked = false;
            }
        }

        void OnClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                o_firecooldown_slider.Value = 6000;
            }
            else
            {
                o_firecooldown_slider.Value = 2000;
            }
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
