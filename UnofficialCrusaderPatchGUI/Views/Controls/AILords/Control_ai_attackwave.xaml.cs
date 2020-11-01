using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.AILords
{
    /// <summary>
    /// Interaction logic for Control_ai_attackwave.xaml
    /// </summary>
    public partial class Control_ai_attackwave : UserControl
    {
        private string Identifier = "ai_attackwave";
        public Control_ai_attackwave()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => ai_attackwave.IsChecked == true)
                    .withSubControlConfig(new List<SubControlConfig>(){
                        new SubControlConfig()
                            .withIdentifier(this.Identifier)
                            .withSetEnabled((bool value) => ai_attackwave.IsChecked = value)
                            .withEnabled(() => ai_attackwave.IsChecked == true)
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
