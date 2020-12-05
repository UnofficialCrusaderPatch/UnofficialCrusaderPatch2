using System;
using System.Collections.Generic;
using System.Linq;
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
        bool FirstLoad = false;

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

            foreach (KeyValuePair<string, StartResourceConfiguration> aivOption in availableSelection)
            {
                string identifier = aivOption.Key;
                StartResourceConfiguration config = aivOption.Value;

                Expander mainControl = new Expander();
                CheckBox fullSelection = new CheckBox();
                fullSelection.Content = identifier;
                fullSelection.Tag = identifier;
                fullSelection.Click += OnClick;

                mainControl.Header = fullSelection;
                StartResourceStackpanel.Children.Add(mainControl);

                TextBlock descriptionView = BuildDescription(config.Description);
                mainControl.Content = descriptionView;

                fullSelection.IsChecked = identifier.Equals((this.DataContext as MainViewModel).ActiveStartResource);
                StartResourceControlList.Add(fullSelection);
            }
            Utility.SetAutoSaveCallback(StartResourceStackpanel, (this.DataContext as MainViewModel));
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
            textBlock.Margin = new Thickness(20, 10, 20, 10);
            textBlock.Padding = new Thickness(5, 5, 5, 5);
            textBlock.Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200));
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
