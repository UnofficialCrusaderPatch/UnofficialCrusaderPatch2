using System.Windows;
using System.Windows.Controls;
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

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(TroopsStackpanel);
        }
    }
}
