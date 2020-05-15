using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCP.Structs;
using static UCP.Utility;
namespace UCP.Data
{
    public partial class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            languages = Enum.GetValues(typeof(Languages)).Cast<Languages>().ToArray();


        }

        public void LoadUCPData()
        {

        }


        private Languages actualLanguage = Languages.English;
        public Languages ActualLanguage
        {

            get => actualLanguage;
            set
            {
                actualLanguage = value;
                this.RaisePropertyChanged("ActualLanguage");
                SelectCulture(value);
                Configuration.Language = (int)value;
                Configuration.Save("Language");

                //update Lists
                this.RaisePropertyChanged("ImageResourceKey");
            }
        }

        private Dictionary<string, object> _xamlObjects = new Dictionary<string, object>();
        internal void AddXamlObjects(StackPanel mainStackPanel)
        {
            try
            {
                foreach (var item in mainStackPanel.Children)
                {
                    if (item is Expander)
                    {
                        var expander = item as Expander;
                        var frameworkElement = expander.Header as FrameworkElement;
                        if (frameworkElement != null && !String.IsNullOrEmpty(frameworkElement.Name))
                        {
                            _xamlObjects.Add(frameworkElement.Name, expander.Header);
                            if (expander.Content.GetType() != typeof(TextBlock))
                            {
                                var panel = expander.Content as Panel;
                                LoadXamlObjectsFromStackPanel(panel);
                            }
                        }
                        else
                        {
                            Debug.Error("Framework element not found, check if names are Set in TabView " + mainStackPanel.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Debug.Error("Cant add Xaml Object to Dictionary, check if there is a x:name set double in the xaml TabViews" + mainStackPanel.Name+ "\n" + e.Message);
            }
           
        }

        private void LoadXamlObjectsFromStackPanel(Panel stackPanel)
        {
            if (stackPanel == null) return;
            foreach (var treeViewChild in stackPanel.Children)
            {
                if (treeViewChild.GetType() != typeof(TextBlock) && treeViewChild.GetType() != typeof(Grid))
                {
                    if (treeViewChild.GetType() == typeof(StackPanel))
                    {
                        LoadXamlObjectsFromStackPanel(treeViewChild as StackPanel);
                    }
                    else
                    {
                        var frameworkElementChild = treeViewChild as FrameworkElement;
                        if (String.IsNullOrEmpty(frameworkElementChild.Name))
                        {
                            Debug.Error("Framework element not found, check if names are Set in TabView " + stackPanel.Name);
                            continue;
                        }else if (frameworkElementChild.Name.StartsWith("Ignore"))
                        {
                            continue;
                        }
                        _xamlObjects.Add(frameworkElementChild.Name, frameworkElementChild);
                    }
                    
                }
            }
        }

        private readonly Languages[] languages;


        /// <summary>
        /// This Loads the UCP Config File and apply it to the GUI Elements
        /// </summary>
        /// <param name="path">The path to the UCP Config File</param>
        public void LoadConfig(String path)
        {
            try
            {
                var fileText = File.ReadAllText(path);
                var config = JsonConvert.DeserializeObject<ConfigFile>(fileText);
                for (int i = 0; i < config.Configs.Length; i++)
                {
                    object xamlObject;
                    if (_xamlObjects.TryGetValue(config.Configs[i].XamlObjectName, out xamlObject))
                    {
                        var dataType = xamlObject.GetType();
                        if (dataType == typeof(CheckBox))
                        {
                            var checkbox = xamlObject as CheckBox;
                            checkbox.IsChecked = (bool)config.Configs[i].ObjectValue;
                        }
                        else if (dataType == typeof(Slider))
                        {
                            var slider = xamlObject as Slider;
                            slider.Value = (double)config.Configs[i].ObjectValue;
                        }
                        else if (dataType == typeof(Image))
                        {

                            var image = xamlObject as Image;
                            image.Tag = config.Configs[i].ObjectValue;
                        }
                        else
                        {
                            Debug.Error("Cant find Object in Dictionary: " + config.Configs[i].XamlObjectName);
                        }

                    }
                    else
                    {
                        Debug.Error("Error while loading Object: " + config.Configs[i].XamlObjectName);
                    }
                }
            }
            catch (Exception e)
            {
                //todo fileformat checking
                Debug.Error("ucp.cfg is maybe old fileformat: " + e.Message);
            }
            

            
        }

        #region Debug
        public void SaveConfig(String path)
        {
            var newConfig = new ConfigFile();
            newConfig.Language = ActualLanguage;
            newConfig.StrongholdPath = StrongholdPath;
            newConfig.Configs = new SavedConfig[_xamlObjects.Count];

            int c = 0;
            foreach (var item in _xamlObjects)
            {
                newConfig.Configs[c].XamlObjectName = item.Key;
               
                if (item.Value.GetType() == typeof(CheckBox))
                {
                    var checkbox = item.Value as CheckBox;
                    if (checkbox.IsChecked.HasValue) newConfig.Configs[c].ObjectValue = checkbox.IsChecked.Value;
                    else newConfig.Configs[c].ObjectValue = false;
                }
                else if (item.Value.GetType() == typeof(Slider))
                {
                    var slider = item.Value as Slider;
                    newConfig.Configs[c].ObjectValue = slider.Value;
                }
                else if (item.Value.GetType() == typeof(Image))
                {
                    var image = item.Value as Image;
                    newConfig.Configs[c].ObjectValue = image.Tag;
                }
                else
                {
                    Debug.Error("Error while Saving Object: " + newConfig.Configs[c].XamlObjectName);
                }
                c++;
            }

            var data = JsonConvert.SerializeObject(newConfig, Formatting.Indented);
            File.WriteAllText(path, data);
        }
        #endregion


        public Languages[] LanguagesComboBox
        {
            get => languages;
        }
        

        private String imageResourceKey = "ui_Bugfix";
        public String ImageResourceKey
        {

            get => imageResourceKey;
            set
            {
                imageResourceKey = value;
                this.RaisePropertyChanged("ImageResourceKey");
            }
        }
        private String strongholdPath;
        public String StrongholdPath
        {

            get => strongholdPath;
            set
            {
                strongholdPath = value;
                this.RaisePropertyChanged("StrongholdPath");
            }
        }

    }
}
