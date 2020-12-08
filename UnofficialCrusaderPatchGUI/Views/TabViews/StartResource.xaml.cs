using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.TabViews
{
    /// <summary>
    /// Interaktionslogik für AIV.xaml
    /// </summary>
    public partial class StartResource : UserControl
    {
        List<CheckBox> StartResourceControlList;
        Dictionary<string, StartResourceConfiguration> availableSelection { get; set; }
        bool FirstLoad = true;

        public StartResource()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
            StartResourceControlList = new List<CheckBox>();
            availableSelection = UCP.API.ModAPIContract.ListStartResourceConfigurations();
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(StartResourceStackpanel);
        }

        void OnRefresh(object sender, RoutedEventArgs e)
        {
            this.FirstLoad = true;
            this.StartResourceStackpanel.Children.Clear();
            availableSelection = UCP.API.ModAPIContract.ListStartResourceConfigurations();
            string previousSelection = (this.DataContext as MainViewModel).ActiveStartResource;
            OnLoad(sender, e);
            if (!availableSelection.Select(x => x.Key).Contains(previousSelection))
            {
                (this.DataContext as MainViewModel).ActiveStartResource = null;
            }
            (this.DataContext as MainViewModel).WindowClicked(sender, e);
        }

        internal void ApplyConfig(string resourceOption)
        {
            foreach (CheckBox checkBox in StartResourceControlList)
            {
                checkBox.IsChecked = false;
            }
            if (StartResourceControlList.Exists(x => x.Tag.Equals(resourceOption)))
            {
                StartResourceControlList.Single(x => x.Tag.Equals(resourceOption)).IsChecked = true;
            }
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            if (!FirstLoad || availableSelection == null)
            {
                return;
            }
            FirstLoad = false;

            foreach (KeyValuePair<string, StartResourceConfiguration> startResourceOption in availableSelection)
            {
                string identifier = startResourceOption.Key;
                StartResourceConfiguration config = startResourceOption.Value;

                Expander mainControl = new Expander();
                CheckBox fullSelection = new CheckBox();
                fullSelection.Content = identifier;
                fullSelection.Tag = identifier;
                fullSelection.Click += OnClick;

                mainControl.Header = fullSelection;
                StartResourceStackpanel.Children.Add(mainControl);

                TextBlock descriptionView = BuildDescription(config.Description);
                StackPanel layout = new StackPanel();
                layout.Children.Add(descriptionView);
                layout.Margin = new Thickness(20, 10, 20, 10);
                layout.Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200));
                ConditionalAddExportButton(layout, identifier);
                mainControl.Content = layout;

                fullSelection.IsChecked = identifier.Equals((this.DataContext as MainViewModel).ActiveStartResource);
                StartResourceControlList.Add(fullSelection);
            }
            Utility.SetAutoSaveCallback(StartResourceStackpanel, (this.DataContext as MainViewModel));
        }

        private void ConditionalAddExportButton(StackPanel resLayout, string aicOption)
        {
            if (Assembly.LoadFrom("UnofficialCrusaderPatch.dll").GetManifestResourceNames().Contains("UCP.Resources.StartResource." + aicOption + ".json"))
            {
                Button exportButton = new Button()
                {
                    Height = 20,
                    Content = LanguageHelper.GetText("ui_aicExport"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 20, 10),
                    ToolTip = LanguageHelper.GetText("ui_resourceExportHint"),
                };
                exportButton.Click += (s, e) => this.ExportFile(aicOption);
                resLayout.Children.Add(exportButton);
            }
        }

        private void ExportFile(string aicOption)
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "goods", "exports", aicOption + ".json");
            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            if (File.Exists(fileName))
            {
                File.Move(fileName, backupFileName);
            }
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "resources", "goods", "exports"));
            File.WriteAllText(fileName, new StreamReader(Assembly.LoadFrom("UnofficialCrusaderPatch.dll").GetManifestResourceStream("UCP.Resources.StartResource." + aicOption + ".json"), Encoding.UTF8).ReadToEnd());

            Debug.Show(LanguageHelper.GetText("ui_aicExport_success"), aicOption + ".json");
        }

        private TextBlock BuildDescription(Dictionary<string, string> description)
        {
            string currentLanguage = (string)Application.Current.FindResource((this.DataContext as MainViewModel).ActualLanguage.ToString());
            TextBlock textBlock = new TextBlock()
            {
                TextWrapping = TextWrapping.Wrap,
                Text = description.ContainsKey(currentLanguage) ?
                        description[currentLanguage] : description.ContainsKey("English") ?
                                description["English"] : description.FirstOrDefault().Value
            };
            textBlock.IsVisibleChanged += (s, e) => {
                string language = (string)Application.Current.FindResource((this.DataContext as MainViewModel).ActualLanguage.ToString());
                textBlock.Text = description.ContainsKey(language) ?
                        description[language] : description.ContainsKey("English") ?
                                description["English"] : description.FirstOrDefault().Value;
            };
            textBlock.Padding = new Thickness(5, 5, 5, 5);
            return textBlock;
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox selection = sender as CheckBox;
            if (selection.IsChecked == true)
            {
                (this.DataContext as MainViewModel).ActiveStartResource = selection.Tag.ToString();
                foreach (CheckBox aiCheckBox in Utility.FindVisualChildren<CheckBox>(StartResourceStackpanel))
                {
                    if (aiCheckBox != selection)
                    {
                        aiCheckBox.IsChecked = false;
                    }
                }
            }
            else
            {
                (this.DataContext as MainViewModel).ActiveStartResource = null;
            }
        }

        internal string GetValue()
        {
            return (this.DataContext as MainViewModel).ActiveStartResource;
            /*CheckBox control = StartResourceControlList.Where(x => x.IsChecked == true).SingleOrDefault();
            if (control != default(CheckBox))
            {
                return control.Tag.ToString();
            }
            else
            {
                return null;
            }*/
        }
    }
}
