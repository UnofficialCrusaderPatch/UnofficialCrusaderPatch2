﻿using System;
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

            foreach (KeyValuePair<string, StartTroopConfiguration> aivOption in availableSelection)
            {
                string identifier = aivOption.Key;
                StartTroopConfiguration config = aivOption.Value;

                Expander mainControl = new Expander();
                CheckBox fullSelection = new CheckBox();
                fullSelection.Content = identifier;
                fullSelection.Tag = identifier;
                fullSelection.Click += OnClick;

                mainControl.Header = fullSelection;
                StartTroopStackpanel.Children.Add(mainControl);

                TextBlock descriptionView = BuildDescription(config.Description);
                mainControl.Content = descriptionView;

                fullSelection.IsChecked = identifier.Equals((this.DataContext as MainViewModel).ActiveStartTroop);
                StartTroopControlList.Add(fullSelection);
            }
            Utility.SetAutoSaveCallback(StartTroopStackpanel, (this.DataContext as MainViewModel));
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
            /*CheckBox control = StartTroopControlList.Where(x => x.IsChecked == true).SingleOrDefault();
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
