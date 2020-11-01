using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCP.Data;
using UCP.Model;
using UCP.Views.Utils;

namespace UCP.Views.TabViews
{
    /// <summary>
    /// Interaktionslogik für AIV.xaml
    /// </summary>
    public partial class AIV : UserControl
    {
        public List<CheckBox> AIVControlList;
        Dictionary<string, AIVConfiguration> availableSelection { get; set; }
        bool FirstLoad = false;

        public AIV()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChangedEvent;
            AIVControlList = new List<CheckBox>();
            availableSelection = UCP.API.ModAPIContract.ListAIVConfigurations();
        }

        void DataContextChangedEvent(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = e.NewValue as MainViewModel;
            vm.AddXamlObjects(AIVStackpanel);
        }

        internal void ApplyConfig(string aivOption)
        {
            foreach (CheckBox checkBox in AIVControlList)
            {
                checkBox.IsChecked = false;
            }
            if (AIVControlList.Exists(x => x.Tag.Equals(aivOption)))
            {
                AIVControlList.Single(x => x.Tag.Equals(aivOption)).IsChecked = true;
            }
        }

        void OnLoad(object sender, RoutedEventArgs e)
        {
            if (!FirstLoad)
            {
                FirstLoad = true;
            }
            else
            {
                return;
            }

            if (availableSelection == null)
            {
                return;
            }

            foreach (KeyValuePair<string, AIVConfiguration> aivOption in availableSelection)
            {
                string identifier = aivOption.Key;
                AIVConfiguration config = aivOption.Value;

                Expander mainControl = new Expander();
                CheckBox fullSelection = new CheckBox
                {
                    Content = identifier,
                    Tag = identifier
                };
                fullSelection.Click += OnClick;

                mainControl.Header = fullSelection;
                AIVStackpanel.Children.Add(mainControl);

                TextBlock descriptionView = BuildAIVDescription(config.Description);
                mainControl.Content = descriptionView;
                fullSelection.IsChecked = identifier.Equals((this.DataContext as MainViewModel).ActiveAIV);
                AIVControlList.Add(fullSelection);
            }
            Utility.SetAutoSaveCallback(AIVStackpanel, (this.DataContext as MainViewModel));
        }

        private TextBlock BuildAIVDescription(Dictionary<string, string> description)
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
            return textBlock;
        }


        void OnClick(object sender, RoutedEventArgs e)
        {
            CheckBox selection = sender as CheckBox;
            if (selection.IsChecked == true)
            {
                (this.DataContext as MainViewModel).ActiveAIV = selection.Tag.ToString();
                foreach (CheckBox aiCheckBox in Utility.FindVisualChildren<CheckBox>(AIVStackpanel))
                {
                    if (aiCheckBox != selection)
                    {
                        aiCheckBox.IsChecked = false;
                    }
                }
            }
            else
            {
                (this.DataContext as MainViewModel).ActiveAIV = null;
            }
        }

        internal string GetValue()
        {
            return (this.DataContext as MainViewModel).ActiveAIV;
            /*CheckBox control = AIVControlList.SingleOrDefault(x => x.IsChecked == true);
            if (control != default(CheckBox)) {
                return control.Tag.ToString();
            } else
            {
                return null;
            }*/
        }
    }
}
