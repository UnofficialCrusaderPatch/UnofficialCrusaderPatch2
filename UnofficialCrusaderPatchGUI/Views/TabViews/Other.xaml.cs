using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaktionslogik für Other.xaml
    /// </summary>
    public partial class Other : UserControl
    {
        public Other()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
            desc.AddValueChanged(player1_color, someEventHandler);
        }

        private void someEventHandler(object sender, EventArgs e)
        {
            var image = sender as Image;
            if (image == null) return;

            image.SetValue(Grid.ColumnProperty, int.Parse(image.Tag.ToString()));
        }

        DependencyPropertyDescriptor desc = DependencyPropertyDescriptor.FromProperty(FrameworkElement.TagProperty, typeof(FrameworkElement));
        

        private void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(OtherStackpanel);
        }

        private void o_firecooldown_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (o_firecooldown_textbox != null) o_firecooldown_textbox.Text = e.NewValue.ToString();
        }

        private void Ignore_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            player1_color.Tag = button.Tag;
            
            if (int.Parse(button.Tag.ToString()) > 0)
            {
                o_playercolor.IsChecked = true;
            }
            else
            {
                o_playercolor.IsChecked = false;
            }
        }
    }
}
