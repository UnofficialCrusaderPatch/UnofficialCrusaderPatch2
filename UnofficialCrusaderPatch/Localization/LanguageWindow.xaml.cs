using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Globalization;

namespace UnofficialCrusaderPatch
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

        static readonly LanguageItem[] Languages = new LanguageItem[]
        {
            new LanguageItem("ui_german"),
            new LanguageItem("ui_english"),
            new LanguageItem("ui_polish"),
            new LanguageItem("ui_russian"),
        };

        struct LanguageItem
        {
            string ident;
            public string Name { get { return Localization.Get(ident); } }

            public LanguageItem(string ident)
            {
                this.ident = ident;
            }
        }

        public static bool ShowSelection()
        {
            LanguageWindow win = new LanguageWindow();
            win.comboBox.ItemsSource = Languages;

            CultureInfo info = CultureInfo.CurrentCulture;
            int index = 1;
            if (info != null)
            {
                for (int i = 0; i < 3; i++) // just to be safe, I don't know enough about cultureinfos
                {
                    CultureInfo parent = info.Parent;
                    if (parent == null || string.IsNullOrWhiteSpace(parent.Name))
                        break;
                    info = parent;
                }

                switch (info.Name)
                {
                    case "en":
                    default:
                        index = 1;
                        break;
                    case "de":
                        index = 0;
                        break;
                    case "pl":
                        index = 2;
                        break;
                    case "ru":
                        index = 3;
                        break;
                }
            }

            Localization.LanguageIndex = index;
            win.comboBox.SelectedIndex = index;
            win.UpdateTexts();

            return win.ShowDialog() == true;
        }

        void UpdateTexts()
        {
            this.Title = Localization.Get("ui_chooseLangTitle");
            textBlock.Text = Localization.Get("ui_chooseLang");
            okButton.Content = Localization.Get("ui_accept");
            cancelButton.Content = Localization.Get("ui_cancel");

            comboBox.Items.Refresh();
        }

        void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Localization.LanguageIndex == comboBox.SelectedIndex)
                return;

            Localization.LanguageIndex = comboBox.SelectedIndex;
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
