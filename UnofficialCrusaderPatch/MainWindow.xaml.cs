using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using UCP.Patching;
using System.Windows.Media.Imaging;

namespace UCP
{
    public partial class MainWindow : Window
    {
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
            Configuration.LoadGeneral();

            // choose language
            if (!LanguageWindow.ShowSelection(Configuration.Language))
            {
                Close();
                return;
            }

            if (Configuration.Language != Localization.LanguageIndex)
            {
                Configuration.Language = Localization.LanguageIndex;
                Configuration.Save("Language");
            }

            // init main window
            InitializeComponent();

            // set title
            this.Title = string.Format("{0} {1}", Localization.Get("Name"), Version.PatcherVersion);
            
            if (!Directory.Exists(Configuration.Path))
            {
                // check if we can already find the steam path
                const string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 40970";
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey(key, false);
                if (myKey != null && myKey.GetValue("InstallLocation") is string path 
                    && !string.IsNullOrWhiteSpace(path) && Patcher.CrusaderExists(path))
                {
                    pTextBoxPath.Text = path;
                }
                else if (Patcher.CrusaderExists(Environment.CurrentDirectory))
                {
                    pTextBoxPath.Text = Environment.CurrentDirectory;
                }
            }
            else
            {
                pTextBoxPath.Text = Configuration.Path;
            }

            // set translated ui elements
            pathBox.Text = Localization.Get("ui_searchpath");
            pButtonCancel.Content = Localization.Get("ui_cancel");
            pButtonContinue.Content = Localization.Get("ui_continue");
            pButtonSearch.Content = Localization.Get("ui_search");
            pButtonUninstall.Content = Localization.Get("ui_uninstall");
            iButtonBack.Content = Localization.Get("ui_back");
            iButtonInstall.Content = Localization.Get("ui_install");
            TextReferencer.SetText(linkLabel, Localization.Get("ui_welcometext"));

            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream("UCP.license.txt"))
            using (StreamReader sr = new StreamReader(stream))
                linkLabel.Inlines.Add("\n\n\n\n\n\n" + sr.ReadToEnd());
        }

        #region Path finding

        void pButtonUninstall_Click(object sender, RoutedEventArgs e)
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

        void bPathSearch_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;
                dialog.SelectedPath = Directory.Exists(pTextBoxPath.Text) ? pTextBoxPath.Text : null;
                dialog.Description = "Bitte wähle dein Stronghold Crusader - Installationsverzeichnis.";

                var result = dialog.ShowDialog();
                if ((int)result == 1)
                    pTextBoxPath.Text = dialog.SelectedPath;
            }
        }

        bool viewLoaded = false;
        void pButtonContinue_Click(object sender, RoutedEventArgs e)
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
                Configuration.Save("Path");
            }

            if (!viewLoaded)
            {
                // load aic files
                AICChange.LoadFiles();
                Configuration.LoadChanges();

                // fill setup options list
                FillTreeView(Version.Changes);
                viewLoaded = true;
            }

            pathGrid.Visibility = Visibility.Hidden;
            installGrid.Visibility = Visibility.Visible;
        }

        void pButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Install

        void iButtonBack_Click(object sender, RoutedEventArgs e)
        {
            installGrid.Visibility = Visibility.Hidden;
            pathGrid.Visibility = Visibility.Visible;
        }

        void iButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            string path = pTextBoxPath.Text;
            if (!Patcher.CrusaderExists(path))
            {
                Debug.Error(Localization.Get("ui_wrongpath"));
                return;
            }

            iButtonInstall.IsEnabled = false;
            pButtonSearch.IsEnabled = false;
            pTextBoxPath.IsReadOnly = true;
            Version.Changes.ForEach(c => c.SetUIEnabled(false));
            pbLabel.Content = "";

            setupThread = new Thread(DoSetup);
            this.Closed += (s, args) => setupThread.Abort();
            setupThread.Start(pTextBoxPath.Text);
        }

        Thread setupThread;
        void DoSetup(object arg)
        {
            try
            {
                Patcher.Install((string)arg, SetPercent);

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
                    MessageBox.Show(e.ToString(), Localization.Get("ui_error"));
            }
        }

        void SetPercent(double value)
        {
            Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => pbSetup.Value = value * 100.0));
        }

        #endregion

        #region TreeView

        void FillTreeView(IEnumerable<Change> changes)
        {
            foreach (ChangeType type in Enum.GetValues(typeof(ChangeType)))
            {
                string typeName = type.ToString();
                TreeView view = new TreeView()
                {
                    Background = null,
                    BorderThickness = new Thickness(0, 0, 0, 0),
                    Focusable = false,
                };
                view.SelectedItemChanged += View_SelectedItemChanged;

                Grid grid = new Grid();
                grid.Children.Add(view);

                if (type == ChangeType.AIC)
                {
                    AICChange.View = view;

                    Button button = new Button
                    {
                        ToolTip = "Reload .aics",
                        Width = 20,
                        Height = 20,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 5, 5),
                        Content = new Image()
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/UnofficialCrusaderPatch;component/Graphics/refresh.png")),
                        }
                    };
                    button.Click += (s, e) => AICChange.RefreshLocalFiles();
                    grid.Children.Add(button);
                }

                TabItem tab = new TabItem()
                {
                    Header = Localization.Get("ui_" + typeName),
                    Content = grid,
                };
                tabControl.Items.Add(tab);

                foreach (Change change in changes)
                {
                    if (change.Type != type)
                        continue;

                    change.InitUI();
                    view.Items.Add(change.UIElement);
                }
            }
        }

        static void View_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
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
