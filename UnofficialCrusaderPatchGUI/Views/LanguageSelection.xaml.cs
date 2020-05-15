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
using System.Windows.Shapes;
using static UCP.Utility;

namespace UCP.Views
{
    /// <summary>
    /// Interaktionslogik für LanguageSelection.xaml
    /// </summary>
    public partial class LanguageSelection : Window
    {
        public LanguageSelection()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

      

        internal void Closed()
        {
            this.Close();
        }
    }
}
