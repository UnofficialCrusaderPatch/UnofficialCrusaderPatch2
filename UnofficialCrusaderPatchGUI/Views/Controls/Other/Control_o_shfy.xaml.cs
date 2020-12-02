using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.Controls.Other
{
    /// <summary>
    /// Interaction logic for Control_o_shfy.xaml
    /// </summary>
    public partial class Control_o_shfy : UserControl
    {
        private string Identifier = "o_shfy";
        public Control_o_shfy()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;

            UCPControls.Controls.Add(
                new ControlConfig()
                    .withIdentifier(this.Identifier)
                    .withEnabled(() => o_shfy.IsChecked == true)
                    .withSubControlConfig(
                        new List<SubControlConfig>()
                        {
                            new SubControlConfig()
                                .withIdentifier("o_shfy_beer")
                                .withSetEnabled((bool value) => o_shfy_beer.IsChecked = value)
                                .withEnabled(() => o_shfy_beer.IsChecked == true)
                                .withValue(() => null),
                            new SubControlConfig()
                                .withIdentifier("o_shfy_religion")
                                .withSetEnabled((bool value) => o_shfy_religion.IsChecked = value)
                                .withEnabled(() => o_shfy_religion.IsChecked == true)
                                .withValue(() => null),
                            new SubControlConfig()
                                .withIdentifier("o_shfy_peasantspawnrate")
                                .withSetEnabled((bool value) => o_shfy_peasantspawnrate.IsChecked = value)
                                .withEnabled(() => o_shfy_peasantspawnrate.IsChecked == true)
                                .withValue(() => null),
                            new SubControlConfig()
                                .withIdentifier("o_shfy_resourcequantity")
                                .withSetEnabled((bool value) => o_shfy_peasantspawnrate.IsChecked = value)
                                .withEnabled(() => o_shfy_peasantspawnrate.IsChecked == true)
                                .withValue(() => null)
                        })
                );
        }

        void OnMainCheckBoxClicked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == false)
            {
                o_shfy_beer.IsChecked = false;
                o_shfy_peasantspawnrate.IsChecked = false;
                o_shfy_religion.IsChecked = false;
                o_shfy_resourcequantity.IsChecked = false;
            }
            else
            {
                o_shfy_beer.IsChecked = true;
                o_shfy_religion.IsChecked = true;
                o_shfy_resourcequantity.IsChecked = true;
            }
        }

        void OnBeerChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                o_shfy.IsChecked = true;
            }
            else
            {
                if (o_shfy_peasantspawnrate.IsChecked == false && o_shfy_religion.IsChecked == false && o_shfy_resourcequantity.IsChecked == false)
                {
                    o_shfy.IsChecked = false;
                }
            }
        }

        void OnPeasantSpawnRateChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                o_shfy.IsChecked = true;
            }
            else
            {
                if (o_shfy_beer.IsChecked == false && o_shfy_religion.IsChecked == false && o_shfy_resourcequantity.IsChecked == false)
                {
                    o_shfy.IsChecked = false;
                }
            }
        }

        void OnReligionChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                o_shfy.IsChecked = true;
            }
            else
            {
                if (o_shfy_beer.IsChecked == false && o_shfy_peasantspawnrate.IsChecked == false && o_shfy_resourcequantity.IsChecked == false)
                {
                    o_shfy.IsChecked = false;
                }
            }
        }

        void OnResourceChecked(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked == true)
            {
                o_shfy.IsChecked = true;
            }
            else
            {
                if (o_shfy_beer.IsChecked == false && o_shfy_peasantspawnrate.IsChecked == false && o_shfy_religion.IsChecked == false)
                {
                    o_shfy.IsChecked = false;
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
