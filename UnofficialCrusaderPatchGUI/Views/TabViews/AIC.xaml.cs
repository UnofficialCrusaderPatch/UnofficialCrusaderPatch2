using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.TabViews
{
    /// <summary>
    /// Interaktionslogik für AIC.xaml
    /// </summary>
    public partial class AIC : UserControl
    {
        internal List<AICControlConfig> AICControlList { get; set; }
        internal LinkedList<AICControlConfig> selectedControlConfiguration { get; set; }
        List<AICConfiguration> availableSelection { get; set; }
        bool FirstLoad = true;

        public AIC()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
            availableSelection = UCP.API.ModAPIContract.ListAICConfigurations();
            selectedControlConfiguration = new LinkedList<AICControlConfig>();
        }

        void OnRefresh(object sender, RoutedEventArgs e)
        {
            this.FirstLoad = true;
            this.AICStackpanel.Children.Clear();
            availableSelection = UCP.API.ModAPIContract.ListAICConfigurations();
            LinkedList<AICControlConfig> previousSelection = selectedControlConfiguration;
            OnLoad(sender, e);
            foreach (AICControlConfig config in previousSelection)
            {
                if (!availableSelection.Select(x => x.Identifier).Contains(config.Identifier))
                {
                    previousSelection.Remove(config);
                }
            }
            selectedControlConfiguration = previousSelection;
            (this.DataContext as MainViewModel).WindowClicked(sender, e);
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(AICStackpanel);
            vm.GenerateAICConfiguration = GetSelectedConfiguration;
        }

        internal List<AICConfiguration> GetSelectedConfiguration()
        {
            List<AICConfiguration> selectedAICConfiguration = new List<AICConfiguration>();

            if (MainWindow.DEBUG || selectedControlConfiguration.Count > 0)
            {
                foreach (AICControlConfig controlConfig in AICControlList.Where(x => x.Enabled() == true))
                {
                    AICConfiguration currentConfig = new AICConfiguration()
                        .withIdentifier(controlConfig.Identifier)
                        .withCharacterList(controlConfig.CharacterList());
                    selectedAICConfiguration.Add(currentConfig);
                }
                return selectedAICConfiguration;
            }
            return null;
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            if (!FirstLoad || availableSelection == null)
            {
                return;
            }
            FirstLoad = false;

            AICControlList = new List<AICControlConfig>();
            selectedControlConfiguration = new LinkedList<AICControlConfig>();
            foreach (AICConfiguration configuration in availableSelection)
            {
                AddAICControl(configuration.Identifier, configuration);
            }
            ApplyConfig();
            Utility.SetAutoSaveCallback(AICStackpanel, (this.DataContext as MainViewModel));
        }

        private void ApplyConfig()
        {
            ApplyConfig((this.DataContext as MainViewModel).ActiveAIC);
        }

        internal void ApplyConfig(object aicConfiguration)
        {
            if (aicConfiguration == null)
            {
                return;
            }

            List<CheckBox> checkBoxes = Utility.FindLogicalChildren<CheckBox>(AICStackpanel).ToList();
            checkBoxes.ForEach(checkBox => checkBox.IsChecked = false);

            Utility.FindLogicalChildren<RadioButton>(AICStackpanel).ToList().ForEach(button => button.IsChecked = false);

            if (aicConfiguration is string)
            {
                checkBoxes.Single(x => x.Tag.Equals(aicConfiguration)).IsChecked = true;
            }
            selectedControlConfiguration.Clear();

            foreach (JObject rawAICConfig in (JArray)aicConfiguration)
            {
                Dictionary<string, object> aicConfig = rawAICConfig.ToObject<Dictionary<string, object>>();
                if (!aicConfig.ContainsKey("Identifier") || !checkBoxes.Exists(x => x.Tag.Equals(aicConfig["Identifier"])))
                {
                    continue;
                }

                if (aicConfig.TryGetValue("CharacterList", out object characters) && characters != null)
                {
                    // Set AIC option selection
                    checkBoxes.Single(x => x.Tag.Equals(aicConfig["Identifier"])).IsChecked = true;

                    // If DEBUG mode active set individual character selection
                    if (MainWindow.DEBUG)
                    {
                        ApplyCharacterSelection(aicConfig["Identifier"].ToString(), characters);
                    }
                    else
                    {
                        selectedControlConfiguration.AddLast(AICControlList.Single(config => config.Identifier.Equals(aicConfig["Identifier"])));
                    }
                }
            }
        }

        private void ApplyCharacterSelection(string name, object characters)
        {
            foreach (RadioButton button in Utility.FindLogicalChildren<RadioButton>(AICStackpanel).Where(x => x.Tag.Equals(name) && ((Object[])characters).Contains(x.GroupName)))
            {
                if (!Utility.FindLogicalChildren<RadioButton>(AICStackpanel).ToList()
                    .Exists(x => x.GroupName.Equals(button.GroupName) && x.IsChecked == true))
                {
                    button.IsChecked = true;
                }
            }
        }

        void OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox selection = sender as CheckBox;
            if (MainWindow.DEBUG)
            {
                if (selection.IsChecked == true)
                {
                    // Select the clicked AIC option
                    foreach (RadioButton aiCharacterBtn in Utility.FindLogicalChildren<RadioButton>(AICStackpanel))
                    {
                        aiCharacterBtn.IsChecked = aiCharacterBtn.Tag.ToString().Equals(selection.Tag.ToString());
                    }

                    // Update selection: if the change results in an AIC option with no characters selected deleted that AIC option
                    foreach (CheckBox aiOption in Utility.FindLogicalChildren<CheckBox>(AICStackpanel))
                    {
                        bool hasSelected = Utility.FindLogicalChildren<RadioButton>(AICStackpanel).ToList().Exists(button => button.IsChecked == true && button.Tag.ToString().Equals(aiOption.Tag.ToString()));
                        if (!hasSelected)
                        {
                            aiOption.IsChecked = false;
                        }
                    }
                }
                else
                {
                    // Deselect all redio buttons representing individual character selections
                    foreach (RadioButton aiCharacterBtn in Utility.FindLogicalChildren<RadioButton>(AICStackpanel).Where(x => x.Tag.ToString().Equals(selection.Tag.ToString())))
                    {
                        aiCharacterBtn.IsChecked = false;
                    }
                }
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) == 0) // Regular click
            {
                if (selection.IsChecked == true)
                {
                    selectedControlConfiguration.Clear();
                    selectedControlConfiguration.AddFirst(AICControlList.Single(config => config.Identifier == selection.Tag.ToString()));

                    // Deselect all other AIC options
                    Utility.FindVisualChildren<CheckBox>(AICStackpanel).Where(checkBox => !selection.Equals(checkBox)).ToList().ForEach(x => x.IsChecked = false);
                }
            }
            else if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) // Ctrl + Click
            {
                if (selection.IsChecked == true)
                {
                    selectedControlConfiguration.AddLast(AICControlList.Single(config => config.Identifier == selection.Tag.ToString()));
                } else
                {
                    selectedControlConfiguration.Remove(AICControlList.Single(config => config.Identifier == selection.Tag.ToString()));
                }
            }
        }

        void AddAICControl(string aicOption, AICConfiguration aiCConfig)
        {
            Expander mainControl = new Expander();
            CheckBox fullSelection = BuildMainAICCheckBox(aicOption);

            mainControl.Header = fullSelection;

            StackPanel aicLayout = new StackPanel();
            mainControl.Content = aicLayout;

            AICControlConfig aicControlConfiguration;

            Func<List<string>> getCharacterList;
            if (MainWindow.DEBUG)
            {
                Grid aicGrid = CreateAICGrid(aiCConfig);

                for (int i = 0; i < aiCConfig.CharacterList.Count; i++)
                {
                    RadioButton characterSelection = BuildCharacterSelectionButton(i, aicOption, fullSelection, aiCConfig);
                    Grid.SetRow(characterSelection, i / 6);
                    Grid.SetColumn(characterSelection, i % 6);
                    aicGrid.Children.Add(characterSelection);
                }
                aicLayout.Children.Add(aicGrid);
                getCharacterList = () => GetSelectedAIC(aicGrid);
            }
            else
            {
                getCharacterList = () => aiCConfig.CharacterList;
            }
            aicControlConfiguration = new AICControlConfig()
                .withIdentifier(aicOption)
                .withEnabled(() => fullSelection.IsChecked == true)
                .withCharacterList(getCharacterList);

            TextBlock descriptionView = BuildAICDescription(aiCConfig.Description);
            aicLayout.Children.Add(descriptionView);

            ConditionalAddExportButton(aicLayout, aicOption);
            aicLayout.Margin = new Thickness(20, 10, 20, 10);
            aicLayout.Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200));
            AICStackpanel.Children.Add(mainControl);
            AICControlList.Add(aicControlConfiguration);
        }

        private void ConditionalAddExportButton(StackPanel aicLayout, string aicOption)
        {
            if (Assembly.LoadFrom("UnofficialCrusaderPatch.dll").GetManifestResourceNames().Contains("UCP.Resources.AIC." + aicOption + ".json"))
            {
                Button exportButton = new Button()
                {
                    Height = 20,
                    Content = LanguageHelper.GetText("ui_aicExport"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 20, 10),
                    ToolTip = LanguageHelper.GetText("ui_aicHint"),
                };
                exportButton.Click += (s, e) => this.ExportFile(aicOption);
                aicLayout.Children.Add(exportButton);
            }
        }

        private void ExportFile(string aicOption)
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "aic", "exports", aicOption + ".json");
            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            if (File.Exists(fileName))
            {
                File.Move(fileName, backupFileName);
            }
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "resources", "aic", "exports"));
            File.WriteAllText(fileName, new StreamReader(Assembly.LoadFrom("UnofficialCrusaderPatch.dll").GetManifestResourceStream("UCP.Resources.AIC." + aicOption + ".json"), Encoding.UTF8).ReadToEnd());

            Debug.Show(LanguageHelper.GetText("ui_aicExport_success"), aicOption + ".json");
        }

        private TextBlock BuildAICDescription(Dictionary<string, string> description)
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
            /*textBlock.Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200));*/
            return textBlock;
        }

        private RadioButton BuildCharacterSelectionButton(int index, string aicOption, CheckBox fullSelection, AICConfiguration aiCConfig)
        {
            string aiCharacter = aiCConfig.CharacterList.ElementAt(index);
            string aiCustomCharacter = aiCConfig.CustomCharacterList.ElementAtOrDefault(index);

            RadioButton characterSelection = new RadioButton();
            TextBlock aiText = new TextBlock();
            aiText.Text = (aiCustomCharacter != default(string) && aiCustomCharacter.Trim() != "") ? aiCustomCharacter : aiCharacter;
            characterSelection.Content = aiText;
            characterSelection.Tag = aicOption;
            characterSelection.GroupName = aiCharacter;

            characterSelection.Click += (sender, e) => { if ((sender as RadioButton).IsChecked == true) { fullSelection.IsChecked = true; } };
            characterSelection.Click += (sender, e) => {
                foreach (RadioButton aiCharacterBtn in Utility.FindLogicalChildren<RadioButton>(AICStackpanel).Where(x => x.GroupName.Equals((sender as RadioButton).GroupName)))
                {
                    if (aiCharacterBtn != characterSelection)
                    {
                        aiCharacterBtn.IsChecked = false;
                    }
                }
            };
            return characterSelection;
        }

        private CheckBox BuildMainAICCheckBox(string aicOption)
        {
            CheckBox checkBox = new CheckBox();
            checkBox.Content = aicOption;
            checkBox.Tag = aicOption;
            checkBox.Click += (sender, e) => OnClick(sender, e);
            checkBox.IsChecked = aicOption.Equals((this.DataContext as MainViewModel).ActiveAIC);
            return checkBox;
        }

        private Grid CreateAICGrid(AICConfiguration aiCConfig)
        {
            Grid aicGrid = new Grid();
            RowDefinition rowDefinition = new RowDefinition();
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition.Height = new GridLength(20);
            rowDefinition1.Height = new GridLength(20);
            rowDefinition2.Height = new GridLength(20);
            aicGrid.RowDefinitions.Add(rowDefinition);
            aicGrid.RowDefinitions.Add(rowDefinition1);
            aicGrid.RowDefinitions.Add(rowDefinition2);

            for (int i = 0; i < aiCConfig.CharacterList.Count; i++)
            {
                ColumnDefinition columnDefinition = new ColumnDefinition();
                columnDefinition.Width = new GridLength(80);
                aicGrid.ColumnDefinitions.Add(columnDefinition);
            }
            return aicGrid;
        }

        List<string> GetSelectedAIC(Grid aicGrid)
        {
            List<string> selectedCharacters = new List<string>();
            foreach (RadioButton button in aicGrid.Children)
            {
                if (button.IsChecked == true)
                {
                    selectedCharacters.Add(button.GroupName);
                }
            }
            return selectedCharacters;
        }
    }
}
