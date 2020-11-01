using System.Windows;
using System.Windows.Controls;
using UCP.Data;

namespace UCP.Views.TabViews
{
    /// <summary>
    /// Interaktionslogik für Bugfixes.xaml
    /// </summary>
    public partial class Bugfixes : UserControl
    {
        public Bugfixes()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(BugfixesStackpanel);
        }
    }
}
