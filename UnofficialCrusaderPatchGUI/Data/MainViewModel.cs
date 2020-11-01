using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCP.Model;
using UCP.Util;
using UCP.Views.Controls;
using UCP.Views.Utils;
using static UCP.Data.LanguageHelper;
using static UCP.Util.Constants;

namespace UCP.Data
{
    public partial class MainViewModel : ViewModelBase
    {
        private Languages actualLanguage = Languages.English;
        public Languages ActualLanguage
        {

            get => actualLanguage;
            set
            {
                actualLanguage = value;
                this.RaisePropertyChanged("ActualLanguage");
                SelectCulture(value);

                //update Lists
                this.RaisePropertyChanged("ImageResourceKey");
            }
        }

        public bool LOADED = false;
        public UCPConfig CurrentConfig { get; internal set; }
        public Dictionary<string, object> Preferences { get; internal set; }
        public Languages[] LanguagesComboBox { get; }

        private ObservableCollection<string> strongholdPaths = new ObservableCollection<string>();
        public ObservableCollection<string> StrongholdPaths
        {
            get => strongholdPaths;
            set
            {
                strongholdPaths = value;
                this.RaisePropertyChanged("StrongholdPaths");
            }
        }

        private bool validStrongholdPath;
        public bool ValidStrongholdPath
        {
            get => validStrongholdPath;
            set
            {
                validStrongholdPath = value;
                RaisePropertyChanged("ValidStrongholdPath");
            }
        }

        private string strongholdPath;
        public string StrongholdPath
        {

            get => strongholdPath;
            set
            {
                strongholdPath = value;
                Preferences[PATH] = value;
                Resolver.WritePreferences(Preferences);
                this.RaisePropertyChanged("StrongholdPath");
            }
        }

        public string ActiveAIV { get; internal set; }
        public string ActiveStartResource { get; internal set; }
        public string ActiveStartTroop { get; internal set; }

        public object ActiveAIC { get; internal set; }

        private Dictionary<string, object> _xamlObjects = new Dictionary<string, object>();
        internal Func<List<AICConfiguration>> GenerateAICConfiguration;

        public MainViewModel()
        {
            LanguagesComboBox = Enum.GetValues(typeof(Languages)).Cast<Languages>().ToArray();
            ValidStrongholdPath = false;
        }

        internal void AddXamlObjects(StackPanel mainStackPanel)
        {
            foreach (FrameworkElement element in Utility.FindLogicalChildren<FrameworkElement>(mainStackPanel)){
                if (element is CheckBox || element is Slider)
                {
                    _xamlObjects.Add(element.Name, element);
                }
            }
        }

        internal void WindowClicked(object sender, EventArgs e)
        {
            if (!LOADED) return;
             UCPConfig config = new UCPConfig()
                    .withAIV(ActiveAIV)
                    .withStartResource(ActiveStartResource)
                    .withStartTroop(ActiveStartTroop)
                    .withAIC(GenerateAICConfiguration())
                    .withGenericMods(UCPControls.GetModConfiguration())
                    .withPath(StrongholdPath)
                    .withSchema(CurrentConfig.schema)
                    .withHash(Resolver.GenerateHash());
            using (StreamWriter file = File.CreateText(UCP_JSON_PATH))
            {
                Resolver.Writer.Serialize(file, config);
            }
        }
    }
}
