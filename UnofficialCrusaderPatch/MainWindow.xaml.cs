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
            // choose language
            if (!LanguageWindow.ShowSelection())
            {
                Close();
                return;
            }

            // init main window
            InitializeComponent();

            // set title
            this.Title = string.Format("{0} {1}", Localization.Get("Name"), Version.PatcherVersion);

            // check if we can already find the steam path
            const string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 40970";
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey(key, false);
            if (myKey != null && myKey.GetValue("InstallLocation") is string path && !string.IsNullOrWhiteSpace(path))
            {
                pTextBoxPath.Text = path;
            }

            // fill setup options list
            FillTreeView(Version.Changes);

            // set translated ui elements
            pathBox.Text = Localization.Get("ui_searchpath");
            pButtonCancel.Content = Localization.Get("ui_cancel");
            pButtonContinue.Content = Localization.Get("ui_continue");
            pButtonSearch.Content = Localization.Get("ui_search");
            pButtonUninstall.Content = Localization.Get("ui_uninstall");
            iButtonBack.Content = Localization.Get("ui_back");
            iButtonInstall.Content = Localization.Get("ui_install");
            TextReferencer.SetText(linkLabel, "ui_welcometext");

            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream("UnofficialCrusaderPatch.license.txt"))
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

        void pButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            if (!Patcher.CrusaderExists(pTextBoxPath.Text))
            {
                Debug.Error(Localization.Get("ui_wrongpath"));
                return;
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

            iButtonInstall.Content = Localization.Get("ui_finished");
            iButtonInstall.IsEnabled = false;
            iButtonInstall.Click -= iButtonInstall_Click;
            iButtonInstall.Click += (s, a) => this.Close();

            pButtonSearch.IsEnabled = false;
            pTextBoxPath.IsReadOnly = true;

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

                Dispatcher.Invoke(DispatcherPriority.Render, new Action(() => iButtonInstall.IsEnabled = true));
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

                TabItem tab = new TabItem()
                {
                    Header = Localization.Get("ui_" + typeName),
                    Content = view,
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
