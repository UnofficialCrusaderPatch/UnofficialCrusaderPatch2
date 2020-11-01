using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.AILords
{
    /// <summary>
    /// Interaction logic for Control_ai_attacklimit.xaml
    /// </summary>
    public partial class Control_ai_attacklimit : UserControl
    {
        private string Identifier = "ai_attacklimit";
        public Control_ai_attacklimit()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => ai_attacklimit.IsChecked == true)
                    .withSubControlConfig(
                        new List<SubControlConfig>()
                        {
                            new SubControlConfig()
                                .withIdentifier("ai_attacklimit_slider")
                                .withEnabled(() => ai_attacklimit.IsChecked == true)
                                .withSetEnabled((bool value) => ai_attacklimit.IsChecked = value)
                                .withValue(() => ai_attacklimit_slider.Value)
                                .withSetter((double value) => ai_attacklimit_slider.Value = value)
                        })
                );
        }

        void OnSliderChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as Slider).Value != 200)
            {
                ai_attacklimit.IsChecked = true;
            }
            else
            {
                ai_attacklimit.IsChecked = false;
            }
        }

        void OnClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                ai_attacklimit_slider.Value = 500;
            }
            else
            {
                ai_attacklimit_slider.Value = 200;
            }
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
