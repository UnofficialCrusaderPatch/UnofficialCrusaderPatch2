using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.Bugfixes
{
    /// <summary>
    /// Interaction logic for Control_FireBallistaFix.xaml
    /// </summary>
    public partial class Control_fireballistafix : UserControl
    {
        private string Identifier = "u_fireballistafix";
        public Control_fireballistafix()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => u_fireballistafix.IsChecked == true)
                    .withSubControlConfig(new List<SubControlConfig>(){
                        new SubControlConfig()
                            .withIdentifier(this.Identifier)
                            .withSetEnabled((bool value) => u_fireballistafix.IsChecked = value)
                            .withEnabled(() => u_fireballistafix.IsChecked == true)
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
