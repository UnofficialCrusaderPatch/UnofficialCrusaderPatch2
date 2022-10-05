using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace UCP
{
    /// <summary>
    /// Interaktionslogik für LanguageWindow.xaml
    /// </summary>
    public partial class LanguageWindow : Window
    {
        public LanguageWindow()
        {
            InitializeComponent();
        }

        public static bool ShowSelection(int preset)
        {
            LanguageWindow win = new LanguageWindow();
            ComboBox cb = win.comboBox;
            cb.ItemsSource = Localization.Translations.Select(t => t.Name);
            if (preset >= 0)
            {
                cb.SelectedIndex = preset;
            }
            else
            {
                CultureInfo info = CultureInfo.CurrentCulture;
                for (int i = 0; i < 3; i++) // just to be safe, I don't know enough about cultureinfos
                {
                    CultureInfo parent = info.Parent;
                    if (string.IsNullOrWhiteSpace(parent.Name))
                    {
                        break;
                    }

                    info = parent;
                }

                string culture = info.Name;

                cb.SelectedIndex = Localization.GetLangByCulture(culture);
            }

            win.UpdateTexts();
            return win.ShowDialog() == true;
        }

        private void UpdateTexts()
        {
            Title = Localization.Get("ui_chooseLangTitle");
            textBlock.Text = Localization.Get("ui_chooseLang");
            okButton.Content = Localization.Get("ui_accept");
            cancelButton.Content = Localization.Get("ui_cancel");
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Localization.Load(comboBox.SelectedIndex);
            UpdateTexts();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
