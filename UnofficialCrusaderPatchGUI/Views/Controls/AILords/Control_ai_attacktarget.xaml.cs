using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.AILords
{
    /// <summary>
    /// Interaction logic for Control_ai_attacktarget.xaml
    /// </summary>
    public partial class Control_ai_attacktarget : UserControl
    {
        private string Identifier = "ai_attacktarget";
        public Control_ai_attacktarget()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => ai_attacktarget.IsChecked == true)
                    .withSubControlConfig(
                        new List<SubControlConfig>()
                        {
                            new SubControlConfig()
                                .withIdentifier("ai_attacktarget_nearest")
                                .withSetEnabled((bool value) => ai_attacktarget_nearest.IsChecked = value)
                                .withEnabled(() => ai_attacktarget_nearest.IsChecked == true),
                            new SubControlConfig()
                                .withIdentifier("ai_attacktarget_richest")
                                .withSetEnabled((bool value) => ai_attacktarget_richest.IsChecked = value)
                                .withEnabled(() => ai_attacktarget_richest.IsChecked == true),
                            new SubControlConfig()
                                .withIdentifier("ai_attacktarget_weakest")
                                .withSetEnabled((bool value) => ai_attacktarget_weakest.IsChecked = value)
                                .withEnabled(() => ai_attacktarget_weakest.IsChecked == true)
                        })
                );
        }

        void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == false)
            {
                ai_attacktarget_nearest.IsChecked = false;
                ai_attacktarget_richest.IsChecked = false;
                ai_attacktarget_weakest.IsChecked = false;
            }
        }

        void OnSelected(object sender, RoutedEventArgs e)
        {
            if ((sender as RadioButton).IsChecked == true)
            {
                ai_attacktarget.IsChecked = true;
            }
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
