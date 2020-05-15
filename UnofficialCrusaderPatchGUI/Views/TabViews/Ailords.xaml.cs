using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UCP.Data;

namespace UCP.Views.TabViews
{
    /// <summary>
    /// Interaktionslogik für Ailords.xaml
    /// </summary>
    public partial class Ailords : UserControl
    {
        public Ailords()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
        }

        private void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(AiLordsStackpanel);
        }

        private void ai_addattack_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ai_addattack_textbox != null) ai_addattack_textbox.Text = e.NewValue.ToString();
            if (ai_addattack_checkbox != null && ai_addattack != null && ai_addattack_slider1 != null) {
                if (e.NewValue == 5)
                {
                    ai_addattack.IsChecked = ai_addattack_checkbox1.IsChecked;
                    ai_addattack_checkbox.IsChecked = false;
                }
                else
                {
                    ai_addattack.IsChecked = true;
                    ai_addattack_checkbox.IsChecked = true;
                    ai_addattack_slider1.Value = 0;
                }
            }
        }

        private void ai_addattack_slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ai_addattack_textbox1 != null) ai_addattack_textbox1.Text = e.NewValue.ToString();
            if (ai_addattack_checkbox1 != null && ai_addattack != null && ai_addattack_slider != null)
            {
                if (e.NewValue == 0)
                {
                    ai_addattack.IsChecked = ai_addattack_checkbox.IsChecked;
                    ai_addattack_checkbox1.IsChecked = false;
                }
                else
                {
                    ai_addattack.IsChecked = true;
                    ai_addattack_checkbox1.IsChecked = true;
                    ai_addattack_slider.Value = 5;
                }
            }
        }
        private void ai_attacklimit_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ai_attacklimit_textbox != null) ai_attacklimit_textbox.Text = e.NewValue.ToString();
        }

        private void ai_attacktargetChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox.IsChecked.Value)
            {
                if (checkbox == ai_attacktarget_nearest)
                {
                    ai_attacktarget_richest.IsChecked = false;
                    ai_attacktarget_weakest.IsChecked = false;
                }
                else if (checkbox == ai_attacktarget_richest)
                {
                    ai_attacktarget_nearest.IsChecked = false;
                    ai_attacktarget_weakest.IsChecked = false;
                }
                else // checkbox == ai_attacktarget_weakest
                {
                    ai_attacktarget_nearest.IsChecked = false;
                    ai_attacktarget_richest.IsChecked = false;
                }
                ai_attacktarget.IsChecked = true;
            }
            else
            {
                if (!ai_attacktarget_nearest.IsChecked.Value && !ai_attacktarget_richest.IsChecked.Value && !ai_attacktarget_weakest.IsChecked.Value)
                {
                    ai_attacktarget.IsChecked = false;
                }
            }

        }

        private void ai_demolishChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox.IsChecked.Value)
            {
                if (checkbox == ai_demolish_walls)
                {
                    ai_demolish_eco.IsChecked = false;
                    ai_demolish_trapped.IsChecked = false;
                }
                else if (checkbox == ai_demolish_eco)
                {
                    ai_demolish_walls.IsChecked = false;
                    ai_demolish_trapped.IsChecked = false;
                }
                else // checkbox == ai_demolish_trapped
                {
                    ai_demolish_walls.IsChecked = false;
                    ai_demolish_eco.IsChecked = false;
                }
                ai_demolish.IsChecked = true;
            }
            else
            {
                if (!ai_demolish_walls.IsChecked.Value && !ai_demolish_eco.IsChecked.Value && !ai_demolish_trapped.IsChecked.Value)
                {
                    ai_demolish.IsChecked = false;
                }
            }

        }
        private void ai_addattack1Changed(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox.IsChecked.Value)
            {
                ai_addattack_slider1.Value = 0.3;
            }
            else
            {
                ai_addattack_slider1.Value = 0;
            }
        }
        private void ai_addattackChanged(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox.IsChecked.Value)
            {
                ai_addattack_slider.Value = 12;
            }
            else{
                ai_addattack_slider.Value = 5;
            }
        }
    }
}
