using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.Other
{
    /// <summary>
    /// Interaction logic for Control_o_playercolor.xaml
    /// </summary>
    public partial class Control_o_playercolor : UserControl
    {
        private string Identifier = "o_playercolor";
        private SubControlConfig config;
        public Control_o_playercolor()
        {
            InitializeComponent();
            player1_color.SetValue(Grid.ColumnProperty, 0);

            config = new SubControlConfig()
                    .withIdentifier("o_playercolor")
                    .withValue(() => 0)
                    .withEnabled(() => config.Value() != 0);

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => o_playercolor.IsChecked == true)
                    .withSubControlConfig(new List<SubControlConfig>
                    {
                        config.withSetEnabled((bool value) => o_playercolor.IsChecked = value)
                            .withSetter((double value) => SetConfigValue(config, value))
                    })
                );
        }

        private void SetConfigValue(SubControlConfig config, double value)
        {
            config.Value = () => value;
            player1_color.SetValue(Grid.ColumnProperty, (int)value - 1);
        }

        public void Update_Value(object sender, RoutedEventArgs e)
        {
            double value = double.Parse((sender as Button).Tag.ToString());
            SetConfigValue(config, value);

            if (value > 1)
            {
                o_playercolor.IsChecked = true;
            }
            else
            {
                o_playercolor.IsChecked = false;
            }
        }

        public void OnClick(object sender, RoutedEventArgs e)
        {
            SetConfigValue(config, 1);
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
