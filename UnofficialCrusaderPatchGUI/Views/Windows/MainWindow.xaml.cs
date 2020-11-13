using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using UCP.API;
using UCP.Data;
using UCP.Model;
using UCP.Util;
using UCP.Views.Controls;
using UCP.Views.Utils;
using static UCP.Util.Constants;

namespace UCP.Views
{
    public partial class MainWindow : Window
    {
        public static bool DEBUG = false;
        public static bool ENABLE_AUTOSAVE = false;
        readonly MainViewModel _vm;
        private UCPConfig startupConfig = null;
        private Dictionary<string, object> preferences;


        static MainWindow()
        {
            Application.Current.DispatcherUnhandledException += DispatcherException;
        }

        static void DispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.Error(e.Exception);
        }

        public MainWindow()
        {
            MainViewModel vm = new MainViewModel();
            _vm = vm;
            this.DataContext = vm;

            ApplyStartupConfiguration();
            if (!SelectLanguage())
            {
                Application.Current.Shutdown();
            }
            _vm.Preferences[KNOWN_PATHS] = _vm.StrongholdPaths;
            Resolver.WritePreferences(_vm.Preferences);

            // init main window
            InitializeComponent();

            // set title
            this.Title = "Unofficial Crusader Patch vX.XX" + "        " + "ucp.json";
        }

        private bool SelectLanguage()
        {
            Startup languageSelection = new Startup
            {
                DataContext = _vm
            };
            bool? result = languageSelection.ShowDialog().Value;
            return result.HasValue && result.Value;
        }

        void MainWindow_Loaded(object sender, EventArgs e)
        {
            if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control | ModifierKeys.Alt)) != 0)
            {
                DEBUG = true;
            }

            if (startupConfig == null)
            {
                return;
            }
            ApplyConfiguration(startupConfig);
            _vm.LOADED = true;
        }

        #region Event handlers
        void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = JSON_FILTER
            };
            bool? result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                UCPConfig loadedConfig = new JavaScriptSerializer().Deserialize<UCPConfig>(File.ReadAllText(dialog.FileName));

                if (preferences.ContainsKey(PREFER_PATH) && preferences[PREFER_PATH] != null &&
                    bool.Parse(preferences[PREFER_PATH].ToString()))
                {
                    if (loadedConfig.Path != null && loadedConfig.hash != null && Resolver.VerifyHash(loadedConfig.hash) && Resolver.isValidSHCPath(loadedConfig.Path))
                    {
                        MessageBoxResult pathBoxResult = MessageBox.Show("Change path to " + loadedConfig.Path + "?", "Path Selection",
                            MessageBoxButton.YesNoCancel);
                        switch (pathBoxResult)
                        {
                            case MessageBoxResult.Yes:
                                _vm.StrongholdPath = startupConfig.Path;
                                this.Title = "Unofficial Crusader Patch vX.XX" + "        " + Path.GetFileName(dialog.FileName);
                                break;
                            case MessageBoxResult.No:
                                this.Title = "Unofficial Crusader Patch vX.XX" + "        " + Path.GetFileName(dialog.FileName);
                                break;
                            case MessageBoxResult.Cancel:
                                return;
                        }
                    }
                }
                _vm.Preferences[MOST_RECENT_CONFIG] = dialog.FileName;
                Resolver.WritePreferences(_vm.Preferences);
                ApplyConfiguration(loadedConfig);
                _vm.CurrentConfig = loadedConfig;
            }
        }

        void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            bool? result = dialog.ShowDialog();
            dialog.Filter = JSON_FILTER;
            if (result.HasValue && result.Value)
            {
                UCPConfig config = new UCPConfig()
                    .withAIV(GenerateAIVConfiguration())
                    .withStartResource(GenerateStartResourceConfiguration())
                    .withStartTroop(GenerateStartTroopConfiguration())
                    .withAIC(GenerateAICConfiguration())
                    .withGenericMods(GenerateModConfiguration())
                    .withPath(_vm.StrongholdPath)
                    .withSchema(_vm.CurrentConfig.schema)
                    .withHash(Resolver.GenerateHash()); ;
                using (StreamWriter file = File.CreateText(dialog.FileName))
                {
                    Resolver.Writer.Serialize(file, config);
                }
            }
        }

        private void OnPathTextEdit(object sender, RoutedEventArgs e)
        {
            _vm.ValidStrongholdPath = pathComboBox.Text != null && Resolver.isValidSHCPath(pathComboBox.SelectedItem.ToString());
            if (_vm.ValidStrongholdPath && !_vm.StrongholdPaths.Contains(pathComboBox.Text))
            {
                _vm.StrongholdPaths.Add(pathComboBox.Text);
            }
        }

        void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(UCP_SITE_URL);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            //var vm = (this.DataContext as MainViewModel);
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result.Equals(System.Windows.Forms.DialogResult.OK) && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    pathComboBox.Text = dialog.SelectedPath;
                    bool validStrongholdPath = Resolver.isValidSHCPath(dialog.SelectedPath);
                    if (validStrongholdPath)
                    {
                        _vm.Preferences["knownPaths"] = _vm.StrongholdPaths;
                        Resolver.WritePreferences(_vm.Preferences);
                    }
                }
            }
        }
        #endregion

        #region StartupConfig
        private void ApplyStartupConfiguration()
        {
            preferences = Resolver.GetExistingOrWriteEmptyPreference();
            _vm.Preferences = preferences;
            ENABLE_AUTOSAVE = preferences.ContainsKey(AUTOSAVE) && preferences.TryGetValue(AUTOSAVE, out object autosave) && 
                bool.TryParse(autosave.ToString(), out bool enableAutosave) && enableAutosave;
            SetKnownPaths(preferences);
            UCPConfig config = GetStartingConfig(preferences);
            SetInstallationPath(preferences, config);
            startupConfig = config;
            _vm.CurrentConfig = config;
        }

        private void SetKnownPaths(Dictionary<string, object> preferences)
        {
            if (preferences.ContainsKey(KNOWN_PATHS) && preferences[KNOWN_PATHS] != null)
            {
                foreach (string path in (JArray)preferences[KNOWN_PATHS])
                {
                    _vm.StrongholdPaths.Add((path));
                }
            }
        }

        private UCPConfig GetStartingConfig(Dictionary<string, object> preferences)
        {
            UCPConfig config = null;
            if (!File.Exists(UCP_JSON_PATH) && preferences.ContainsKey(MOST_RECENT_CONFIG) && preferences[MOST_RECENT_CONFIG] != null &&
                    File.Exists(preferences[MOST_RECENT_CONFIG].ToString()))
            {
                using (StreamReader file = File.OpenText(preferences[MOST_RECENT_CONFIG].ToString()))
                {
                    config = (UCPConfig)Resolver.Writer.Deserialize(file, typeof(UCPConfig));
                }
            }
            else if (File.Exists(UCP_JSON_PATH))
            {
                _vm.Preferences[MOST_RECENT_CONFIG] = UCP_JSON_PATH;
                using (StreamReader file = File.OpenText(UCP_JSON_PATH))
                {
                    config = (UCPConfig)Resolver.Writer.Deserialize(file, typeof(UCPConfig));
                }
            }
            else if (File.Exists(UCP_CFG_PATH))
            {
                _vm.Preferences[MOST_RECENT_CONFIG] = UCP_CFG_PATH;
                config = Resolver.GetUCPConfigFromUncovertedCfg(UCP_CFG_PATH);
            }
            Resolver.WritePreferences(_vm.Preferences);
            return config;
        }

        private void SetInstallationPath(Dictionary<string, object> preferences, UCPConfig config)
        {
            bool pathSet = false;
            if (preferences.ContainsKey(PREFER_PATH) && preferences[PREFER_PATH] != null &&
                bool.Parse(preferences[PREFER_PATH].ToString()))
            {
                if (config.Path != null && config.hash != null && Resolver.VerifyHash(config.hash) && Resolver.isValidSHCPath(config.Path))
                {
                    _vm.StrongholdPath = config.Path;
                    pathSet = true;
                }
            }

            if (!pathSet && preferences.ContainsKey(PATH) && preferences[PATH] != null)
            {
                string path = preferences[PATH].ToString();
                if (Resolver.isValidSHCPath(path))
                {
                    _vm.StrongholdPath = path;
                }
            }
        }
        #endregion

        #region Installation
        private void ApplyConfiguration(UCPConfig ucpConfig)
        {
            UCPControls.ApplyConfiguration(ucpConfig);
            _vm.ActiveAIV = ucpConfig.AIV;
            _vm.ActiveAIC = ucpConfig.AIC;
            _vm.ActiveStartResource = ucpConfig.StartResource;
            _vm.ActiveStartTroop = ucpConfig.StartTroop;
        }

        private List<ChangeConfiguration> GenerateModConfiguration()
        {
            return UCPControls.GetModConfiguration();
        }

        private List<AICConfiguration> GenerateAICConfiguration()
        {
            return _vm.GenerateAICConfiguration();//AICTabControl.GetSelectedConfiguration();
        }

        private string GenerateAIVConfiguration()
        {
            return _vm.ActiveAIV;//AIVTabControl.GetValue();
        }

        private string GenerateStartTroopConfiguration()
        {
            return _vm.ActiveStartTroop;//StartTroopControl.GetValue();
        }

        private string GenerateStartResourceConfiguration()
        {
            return _vm.ActiveStartResource;//StartResourceControl.GetValue();
        }

        void OnInstall(object sender, RoutedEventArgs e)
        {
            try
            {
                ModAPIContract.Install(new UCPConfig()
                    .withAIV(GenerateAIVConfiguration())
                    .withStartResource(GenerateStartResourceConfiguration())
                    .withStartTroop(GenerateStartTroopConfiguration())
                    .withAIC(GenerateAICConfiguration())
                    .withGenericMods(GenerateModConfiguration())
                    .withPath(_vm.StrongholdPath), false, true);
            } catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            
        }
        #endregion

        /// <summary>
        /// Convenience method for exporting custom AICs
        /// </summary>
        /// <param name="json">Serialized JSON string</param>
        /// <returns></returns>
        private static String Format(String json)
        {
            return Regex.Unescape(
                json.Replace(",\"", ",\n\t\"")
                    .Replace("{", "{\n\t")
                    .Replace("}", "\n}")
                    .Replace("\\r\\n", "\r\n")
                    .Replace("[", "\n\t[")
            );
        }

        private void LanguageChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Utility.FindLogicalChildren<Expander>(this.MainTabControl).ToList().ForEach(x => x.IsExpanded = false);
        }
    }
}
