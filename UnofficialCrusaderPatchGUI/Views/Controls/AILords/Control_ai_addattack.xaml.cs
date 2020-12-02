using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.AILords
{
    /// <summary>
    /// Interaction logic for Control_ai_addattack.xaml
    /// </summary>
    public partial class Control_ai_addattack : UserControl
    {
        private string Identifier = "ai_addattack";
        public Control_ai_addattack()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => ai_addattack.IsChecked == true)
                    .withSubControlConfig(
                        new List<SubControlConfig>()
                        {
                            new SubControlConfig()
                                .withIdentifier("ai_addattack")
                                .withEnabled(() => ai_addattack_checkbox.IsChecked == true)
                                .withValue(() => ai_addattack_slider.Value)
                                .withSetEnabled((bool value) => ai_addattack_checkbox.IsChecked = value)
                                .withSetter((double value) => ai_addattack_slider.Value = value),

                            new SubControlConfig()
                                .withIdentifier("ai_addattack_alt")
                                .withEnabled(() => ai_addattack_checkbox1.IsChecked == true)
                                .withValue(() => ai_addattack_slider1.Value)
                                .withSetEnabled((bool value) => ai_addattack_checkbox1.IsChecked = value)
                                .withSetter((double value) => ai_addattack_slider1.Value = value)
                        }
                    )
                );
        }

        void OnSliderChanged(object sender, RoutedEventArgs e)
        {
            if ((sender as Slider).Value != 5)
            {
                ai_addattack.IsChecked = true;
                ai_addattack_checkbox.IsChecked = true;
                ai_addattack_checkbox1.IsChecked = false;
            }
            else
            {
                ai_addattack_checkbox.IsChecked = false;
                if (ai_addattack_checkbox1 != null && ai_addattack_checkbox1.IsChecked == false)
                {
                    ai_addattack.IsChecked = false;
                }
            }
        }

        void OnSlider1Changed(object sender, RoutedEventArgs e)
        {
            if ((sender as Slider).Value != 0)
            {
                ai_addattack.IsChecked = true;
                ai_addattack_checkbox1.IsChecked = true;
                ai_addattack_checkbox.IsChecked = false;
            }
            else
            {
                ai_addattack_checkbox1.IsChecked = false;
                if (ai_addattack_checkbox != null && ai_addattack_checkbox.IsChecked == false)
                {
                    ai_addattack.IsChecked = false;
                }
            }
        }

        void OnMainClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == false)
            {
                ai_addattack_slider.Value = 5;
                ai_addattack_slider1.Value = 0;
            }
            else
            {
                ai_addattack_slider.Value = 12;
                ai_addattack_slider1.Value = 0.3;
                ai_addattack_checkbox.IsChecked = false;
            }
        }

        void OnCheckBoxClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                ai_addattack_slider.Value = 12;
                ai_addattack_checkbox1.IsChecked = false;
            }
            else
            {
                ai_addattack_slider.Value = 5;
            }
        }

        void OnCheckBox1Clicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                ai_addattack_slider1.Value = 0.3;
                ai_addattack_checkbox.IsChecked = false;
            }
            else
            {
                ai_addattack_slider1.Value = 0;
            }
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
