using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Win32;
using UCP.AIC;
using UCP.AIV;
using UCP.Patching;
using UCP.Startup;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using TabControl = System.Windows.Controls.TabControl;
using TreeView = System.Windows.Controls.TreeView;

namespace UCP
{
    public partial class MainWindow : Window
    {
        static MainWindow()
        {
            Application.Current.DispatcherUnhandledException += DispatcherException;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://unofficialcrusaderpatch.github.io/");
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            DragMove();
        }

        private static void DispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.Error(e.Exception);
        }

        public MainWindow()
        {
            Configuration.Load();

            // choose language
            if (!LanguageWindow.ShowSelection(Configuration.Language))
            {
                Close();
                return;
            }

            if (Configuration.Language != Localization.LanguageIndex)
            {
                Configuration.Language = Localization.LanguageIndex;
            }
            StartTroopChange.Load();
            ResourceChange.Load();
            Version.AddExternalChanges();

            // init main window
            InitializeComponent();

            // set title
            Title = $"{Localization.Get("Name")} {Version.PatcherVersion}";

            // set search path in ui
            SetBrowsePath();

            SetLocalizedUIElements();
            DisplayLicense();   
        }

        #region Settings

        private void SetBrowsePath()
        {
            if (!Directory.Exists(Configuration.Path))
            {
                if (Patcher.CrusaderExists(Environment.CurrentDirectory))
                {
                    pTextBoxPath.Text = Environment.CurrentDirectory;
                }
                else if (Patcher.CrusaderExists(Path.Combine(Environment.CurrentDirectory, "..\\")))
                {
                    Configuration.Path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..\\"));
                }
                else
                {
                    // check if we can already find the steam path
                    const string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 40970";
                    RegistryKey myKey = Registry.LocalMachine.OpenSubKey(key, false);
                    if (myKey?.GetValue("InstallLocation") is string path && !string.IsNullOrWhiteSpace(path) && Patcher.CrusaderExists(path))
                    {
                        pTextBoxPath.Text = path;
                    }
                }
            }
            else
            {
                pTextBoxPath.Text = Configuration.Path;
            }
        }

        private void SetLocalizedUIElements()
        {
            pathBox.Text = Localization.Get("ui_searchpath");
            pButtonCancel.Content = Localization.Get("ui_cancel");
            pButtonContinue.Content = Localization.Get("ui_continue");
            pButtonSearch.Content = Localization.Get("ui_search");
            pButtonUninstall.Content = Localization.Get("ui_uninstall");
            iButtonBack.Content = Localization.Get("ui_back");
            iButtonInstall.Content = Localization.Get("ui_install");
            TextReferencer.SetText(linkLabel, Localization.Get("ui_welcometext"));
        }

        private void DisplayLicense()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream("UCP.license.txt"))
            using (StreamReader sr = new StreamReader(stream))
                linkLabel.Inlines.Add("\n\n\n\n\n\n" + sr.ReadToEnd());
        }

        #endregion

        #region Path finding

        private void pButtonUninstall_Click(object sender, RoutedEventArgs e)
        {
            string path = pTextBoxPath.Text;
            if (Directory.Exists(path))
            {
                Patcher.RestoreOriginals(path);
                Debug.Show(Localization.Get("ui_uninstalldone"), Localization.Get("ui_uninstall"));
            }
            else
            {
                Debug.Error(Localization.Get("ui_wrongpath"));
            }
        }

        private void bPathSearch_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;
                dialog.SelectedPath = Directory.Exists(pTextBoxPath.Text) ? pTextBoxPath.Text : null;
                dialog.Description = Localization.Get("ui_browsepath");

                DialogResult result = dialog.ShowDialog();
                if ((int)result == 1)
                {
                    pTextBoxPath.Text = dialog.SelectedPath;
                }
            }
        }

        private bool viewLoaded;

        private void pButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            string cPath = pTextBoxPath.Text;
            if (!Patcher.CrusaderExists(cPath))
            {
                Debug.Error(Localization.Get("ui_wrongpath"));
                return;
            }

            if (Configuration.Path != cPath)
            {
                Configuration.Path = cPath;
            }

            if (!viewLoaded)
            {
                // fill setup options list
                FillTreeView(Version.Changes);
                viewLoaded = true;
            }
            pathGrid.Visibility = Visibility.Hidden;
            installGrid.Visibility = Visibility.Visible;
        }

        private void pButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion

        #region Install

        private void iButtonBack_Click(object sender, RoutedEventArgs e)
        {
            installGrid.Visibility = Visibility.Hidden;
            pathGrid.Visibility = Visibility.Visible;
        }

        private void iButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            string path = pTextBoxPath.Text;
            if (!Patcher.CrusaderExists(path))
            {
                Debug.Error(Localization.Get("ui_wrongpath"));
                return;
            }
            Configuration.Save();
            iButtonInstall.IsEnabled = false;
            pButtonSearch.IsEnabled = false;
            pTextBoxPath.IsReadOnly = true;
            Version.Changes.ForEach(c => c.SetUIEnabled(false));
            pbLabel.Content = "";
            changeHint.Text = "";

            setupThread = new Thread(DoSetup);
            Closed += (s, args) => setupThread.Abort();
            setupThread.Start(pTextBoxPath.Text);
        }

        private Thread setupThread;

        private void DoSetup(object arg)
        {
            try
            {
                Patcher.Install((string)arg, SetPercent, false, true);

                Dispatcher.Invoke(DispatcherPriority.Render, new Action(() =>
                {
                    iButtonInstall.IsEnabled = true;
                    pButtonSearch.IsEnabled = true;
                    pTextBoxPath.IsReadOnly = false;
                    Version.Changes.ForEach(c => c.SetUIEnabled(true));
                    pbSetup.Value = 0;
                    pbLabel.Content = Localization.Get("ui_finished");
                }));
            }
            catch (Exception e)
            {
                if (!(e is TaskCanceledException || e is ThreadAbortException)) // in case of exit
                {
                    MessageBox.Show(e.ToString(), Localization.Get("ui_error"));
                }
            }
        }

        private void SetPercent(double value)
        {
            Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => pbSetup.Value = value * 100.0));
        }

        #endregion

        #region TreeView

        private void FillTreeView(IEnumerable<Change> changes)
        {
            foreach (ChangeType type in Enum.GetValues(typeof(ChangeType)))
            {
                string typeName = type.ToString();
                Grid grid = new Grid();
                TabItem tab = new TabItem
                              {
                    Header = Localization.Get("ui_" + typeName),
                    Content = grid,
                };
                tabControl.Items.Add(tab);

                switch (type)
                {
                    case ChangeType.Resource:
                        new ResourceView().InitUI(grid, View_SelectedItemChanged);
                        continue;
                    case ChangeType.AIV:
                        AIVView.InitUI(grid, View_SelectedItemChanged);
                        continue;
                    case ChangeType.AIC:
                        new AICView().InitUI(grid, View_SelectedItemChanged);
                        continue;
                    case ChangeType.StartTroops:
                        new StartTroopView().InitUI(grid, View_SelectedItemChanged);
                        continue;
                    case ChangeType.Bugfix:
                        break;
                    case ChangeType.AILords:
                        break;
                    case ChangeType.Troops:
                        break;
                    case ChangeType.Other:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                TreeView view = new TreeView
                                {
                                    Background = null,
                                    BorderThickness = new Thickness(0, 0, 0, 0),
                                    Focusable = false,
                                };
                view.SelectedItemChanged += View_SelectedItemChanged;
                grid.Children.Add(view);

                foreach (Change change in changes)
                {
                    if (change.Type != type)
                    {
                        continue;
                    }

                    change.InitUI();
                    view.Items.Add(change.UIElement);
                }   
            }
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tab = (TabControl) sender;
            pbLabel.Content = "";
            changeHint.Text = (String) (((TabItem) tab.SelectedItem).Header) == "AIC" ? "Ctrl+Click to select multiple aic files" : "";
        }

        private static void View_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView view = (TreeView)sender;
            view.SelectedItemChanged -= View_SelectedItemChanged;

            object[] items = new object[view.Items.Count];
            view.Items.CopyTo(items, 0);

            view.Items.Clear();

            foreach (object o in items)
                view.Items.Add(o);

            view.SelectedItemChanged += View_SelectedItemChanged;
        }
        #endregion
    }
}
