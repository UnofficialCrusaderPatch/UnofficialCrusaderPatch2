using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using UCP.Data;
using UCP.Util;

namespace UCP.Views
{
    /// <summary>
    /// Interaktionslogik für LanguageSelection.xaml
    /// </summary>
    public partial class Startup : Window
    {
        MainViewModel _vm;
        public Startup()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = e.NewValue as MainViewModel;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            //var vm = (this.DataContext as MainViewModel);
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result.Equals(System.Windows.Forms.DialogResult.OK) && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    pathComboBox.Text = dialog.SelectedPath;
                    _vm.ValidStrongholdPath = Resolver.isValidSHCPath(pathComboBox.Text);
                    if (_vm.ValidStrongholdPath && !_vm.StrongholdPaths.Contains(pathComboBox.Text))
                    {
                        _vm.StrongholdPaths.Add(dialog.SelectedPath);
                    }
                }
            }
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            _vm = (this.DataContext as MainViewModel);
            _vm.ValidStrongholdPath = Resolver.isValidSHCPath(pathComboBox.Text);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _vm.ValidStrongholdPath = pathComboBox.SelectedItem != null && Resolver.isValidSHCPath(pathComboBox.SelectedItem.ToString());
        }

        private void OnPathTextEdit(object sender, RoutedEventArgs e)
        {
            _vm.ValidStrongholdPath = pathComboBox.Text != null && Resolver.isValidSHCPath(pathComboBox.Text.ToString());
            if (_vm.ValidStrongholdPath && !_vm.StrongholdPaths.Contains(pathComboBox.Text))
            {
                _vm.StrongholdPaths.Add(pathComboBox.Text);
            }
        }
    }
}
