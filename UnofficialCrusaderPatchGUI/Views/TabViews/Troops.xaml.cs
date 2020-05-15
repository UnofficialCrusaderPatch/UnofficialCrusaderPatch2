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
    /// Interaktionslogik für Troops.xaml
    /// </summary>
    public partial class Troops : UserControl
    {
        public Troops()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
        }

        private void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(TroopsStackpanel);
        }
    }
}
