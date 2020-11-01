using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.Other
{
    /// <summary>
    /// Interaction logic for Control_o_engineertent.xaml
    /// </summary>
    public partial class Control_o_engineertent : UserControl
    {
        private string Identifier = "o_engineertent";
        public Control_o_engineertent()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => o_engineertent.IsChecked == true)
                    .withSubControlConfig(new List<SubControlConfig>(){
                        new SubControlConfig()
                            .withIdentifier(this.Identifier)
                            .withSetEnabled((bool value) => o_engineertent.IsChecked = value)
                            .withEnabled(() => o_engineertent.IsChecked == true)
                            .withValue(() => null)
                    })
                );
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
