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
    public partial class StartTroop : UserControl
    {
        List<CheckBox> StartTroopControlList;
        Dictionary<string, StartTroopConfiguration> availableSelection { get; set; }
        bool FirstLoad = true;

        public StartTroop()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
            StartTroopControlList = new List<CheckBox>();
            availableSelection = UCP.API.ModAPIContract.ListStartTroopConfigurations();
            StartTroopControlList = new List<CheckBox>();
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(StartTroopStackpanel);
        }

        void OnRefresh(object sender, RoutedEventArgs e)
        {
            this.FirstLoad = true;
            this.StartTroopStackpanel.Children.Clear();
            availableSelection = UCP.API.ModAPIContract.ListStartTroopConfigurations();
            string previousSelection = (this.DataContext as MainViewModel).ActiveStartTroop;
            OnLoad(sender, e);
            if (!availableSelection.Select(x => x.Key).Contains(previousSelection))
            {
                (this.DataContext as MainViewModel).ActiveStartTroop = null;
            }
            (this.DataContext as MainViewModel).WindowClicked(sender, e);
        }

        internal void ApplyConfig(string resourceOption)
        {
            foreach (CheckBox checkBox in StartTroopControlList)
            {
                checkBox.IsChecked = false;
            }
            if (StartTroopControlList.Exists(x => x.Tag.Equals(resourceOption)))
            {
                StartTroopControlList.Single(x => x.Tag.Equals(resourceOption)).IsChecked = true;
            }
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            if (!FirstLoad || availableSelection == null)
            {
                return;
            }
            FirstLoad = false;

            foreach (KeyValuePair<string, StartTroopConfiguration> startTroopOption in availableSelection)
            {
                string identifier = startTroopOption.Key;
                StartTroopConfiguration config = startTroopOption.Value;

                Expander mainControl = new Expander();
                CheckBox fullSelection = new CheckBox();
                fullSelection.Content = identifier;
                fullSelection.Tag = identifier;
                fullSelection.Click += OnClick;

                mainControl.Header = fullSelection;
                StartTroopStackpanel.Children.Add(mainControl);

                TextBlock descriptionView = BuildDescription(config.Description);
                StackPanel layout = new StackPanel();
                layout.Children.Add(descriptionView);
                layout.Margin = new Thickness(20, 10, 20, 10);
                layout.Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200));
                ConditionalAddExportButton(layout, identifier);
                mainControl.Content = layout;

                fullSelection.IsChecked = identifier.Equals((this.DataContext as MainViewModel).ActiveStartTroop);
                StartTroopControlList.Add(fullSelection);
            }
            Utility.SetAutoSaveCallback(StartTroopStackpanel, (this.DataContext as MainViewModel));
        }

        private void ConditionalAddExportButton(StackPanel troopLayout, string aicOption)
        {
            if (Assembly.LoadFrom("UnofficialCrusaderPatch.dll").GetManifestResourceNames().Contains("UCP.Resources.StartTroop." + aicOption + ".json"))
            {
                Button exportButton = new Button()
                {
                    Height = 20,
                    Content = LanguageHelper.GetText("ui_aicExport"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 20, 10),
                    ToolTip = LanguageHelper.GetText("ui_troopExportHint"),
                };
                exportButton.Click += (s, e) => this.ExportFile(aicOption);
                troopLayout.Children.Add(exportButton);
            }
        }

        private void ExportFile(string aicOption)
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "troops", "exports", aicOption + ".json");
            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            if (File.Exists(fileName))
            {
                File.Move(fileName, backupFileName);
            }
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "resources", "troops", "exports"));
            File.WriteAllText(fileName, new StreamReader(Assembly.LoadFrom("UnofficialCrusaderPatch.dll").GetManifestResourceStream("UCP.Resources.StartTroop." + aicOption + ".json"), Encoding.UTF8).ReadToEnd());

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
                (this.DataContext as MainViewModel).ActiveStartTroop = selection.Tag.ToString();
                foreach (CheckBox aiCheckBox in Utility.FindVisualChildren<CheckBox>(StartTroopStackpanel))
                {
                    if (aiCheckBox != selection)
                    {
                        aiCheckBox.IsChecked = false;
                    }
                }
            }
            else
            {
                (this.DataContext as MainViewModel).ActiveStartTroop = null;
            }
        }

        internal string GetValue()
        {
            return (this.DataContext as MainViewModel).ActiveStartResource;
        }
    }
}
