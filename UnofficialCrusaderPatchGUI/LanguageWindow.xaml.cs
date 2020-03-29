using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

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
                string culture = "en";

                CultureInfo info = CultureInfo.CurrentCulture;
                if (info != null)
                {
                    for (int i = 0; i < 3; i++) // just to be safe, I don't know enough about cultureinfos
                    {
                        CultureInfo parent = info.Parent;
                        if (parent == null || string.IsNullOrWhiteSpace(parent.Name))
                            break;
                        info = parent;
                    }

                    culture = info.Name;
                }

                cb.SelectedIndex = Localization.GetLangByCulture(culture);
            }

            win.UpdateTexts();
            return win.ShowDialog() == true;
        }

        void UpdateTexts()
        {
            this.Title = Localization.Get("ui_chooseLangTitle");
            textBlock.Text = Localization.Get("ui_chooseLang");
            okButton.Content = Localization.Get("ui_accept");
            cancelButton.Content = Localization.Get("ui_cancel");
        }

        void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Localization.Load(comboBox.SelectedIndex);
            UpdateTexts();
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        void Accept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
