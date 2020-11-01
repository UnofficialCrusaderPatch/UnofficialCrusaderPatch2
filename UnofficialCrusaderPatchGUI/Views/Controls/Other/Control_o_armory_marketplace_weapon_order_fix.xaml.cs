using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.Other
{
    /// <summary>
    /// Interaction logic for Control_o_armory_marketplace_weapon_order_fix.xaml
    /// </summary>
    public partial class Control_o_armory_marketplace_weapon_order_fix : UserControl
    {
        private string Identifier = "o_armory_marketplace_weapon_order_fix";
        public Control_o_armory_marketplace_weapon_order_fix()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => o_armory_marketplace_weapon_order_fix.IsChecked == true)
                    .withSubControlConfig(new List<SubControlConfig>(){
                        new SubControlConfig()
                            .withIdentifier(this.Identifier)
                            .withSetEnabled((bool value) => o_armory_marketplace_weapon_order_fix.IsChecked = value)
                            .withEnabled(() => o_armory_marketplace_weapon_order_fix.IsChecked == true)
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
