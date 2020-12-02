using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.AILords
{
    /// <summary>
    /// Interaction logic for Control_ai_demolish.xaml
    /// </summary>
    public partial class Control_ai_demolish : UserControl
    {
        private string Identifier = "ai_demolish";
        public Control_ai_demolish()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => ai_demolish.IsChecked == true)
                    .withSubControlConfig(
                        new List<SubControlConfig>()
                        {
                            new SubControlConfig()
                                .withIdentifier("ai_demolish_walls")
                                .withSetEnabled((bool value) => ai_demolish_walls.IsChecked = value)
                                .withEnabled(() => ai_demolish_walls.IsChecked == true)
                                .withValue(() => null),
                            new SubControlConfig()
                                .withIdentifier("ai_demolish_eco")
                                .withSetEnabled((bool value) => ai_demolish_eco.IsChecked = value)
                                .withEnabled(() => ai_demolish_eco.IsChecked == true)
                                .withValue(() => null),
                            new SubControlConfig()
                                .withIdentifier("ai_demolish_trapped")
                                .withSetEnabled((bool value) => ai_demolish_trapped.IsChecked = value)
                                .withEnabled(() => ai_demolish_trapped.IsChecked == true)
                                .withValue(() => null)
                        })
                );
        }

        void OnMainCheckBoxClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == false)
            {
                ai_demolish_walls.IsChecked = false;
                ai_demolish_trapped.IsChecked = false;
                ai_demolish_eco.IsChecked = false;
            } else
            {
                ai_demolish_walls.IsChecked = true;
            }
        }

        void OnAIDemolishWallsChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                ai_demolish.IsChecked = true;
            } else
            {
                if (ai_demolish_eco.IsChecked == false && ai_demolish_trapped.IsChecked == false)
                {
                    ai_demolish.IsChecked = false;
                }
            }
        }

        void OnAIDemolishEcoChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                ai_demolish.IsChecked = true;
            }
            else
            {
                if (ai_demolish_trapped.IsChecked == false && ai_demolish_walls.IsChecked == false)
                {
                    ai_demolish.IsChecked = false;
                }
            }
        }

        void OnAIDemolishTrappedChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                ai_demolish.IsChecked = true;
            }
            else
            {
                if (ai_demolish_walls.IsChecked == false && ai_demolish_eco.IsChecked == false)
                {
                    ai_demolish.IsChecked = false;
                }
            }
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            Utility.SetAutoSaveCallback(this.expander, vm);
        }
    }
}
