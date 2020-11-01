using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.Other
{
    /// <summary>
    /// Interaction logic for Control_o_default_multiplayer_speed.xaml
    /// </summary>
    public partial class Control_o_default_multiplayer_speed : UserControl
    {
        private string Identifier = "o_default_multiplayer_speed";
        public Control_o_default_multiplayer_speed()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => o_default_multiplayer_speed.IsChecked == true)
                    .withSubControlConfig(new List<SubControlConfig>(){
                        new SubControlConfig()
                            .withIdentifier(this.Identifier)
                            .withSetEnabled((bool value) => o_default_multiplayer_speed.IsChecked = value)
                            .withEnabled(() => o_default_multiplayer_speed.IsChecked == true)
                            .withValue(() => o_default_multiplayer_speed_slider.Value)
                            .withSetter((double value) => o_default_multiplayer_speed_slider.Value = value)
                    })
                );
        }

        void OnSliderChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as Slider).Value != 40)
            {
                o_default_multiplayer_speed.IsChecked = true;
            }
            else
            {
                o_default_multiplayer_speed.IsChecked = false;
            }
        }

        void OnClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                o_default_multiplayer_speed_slider.Value = 50;
            }
            else
            {
                o_default_multiplayer_speed_slider.Value = 40;
            }
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
