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

namespace UnofficialCrusaderPatch
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            try
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
                foreach (Change c in Version.Changes)
                {
                    ItemsControl treeView = c.Type == ChangeType.Balancing ? tviBalancing : tviBugfixes;
                    int index = treeView.Items.Add(c);
                }

                // set translated ui elements
                pathBox.Text = Localization.Get("ui_searchpath");
                pButtonCancel.Content = Localization.Get("ui_cancel");
                pButtonContinue.Content = Localization.Get("ui_continue");
                pButtonSearch.Content = Localization.Get("ui_search");
                pButtonUninstall.Content = Localization.Get("ui_uninstall");
                TextReferencer.SetText(linkLabel, "ui_welcometext");

                var asm = System.Reflection.Assembly.GetExecutingAssembly();
                using (Stream stream = asm.GetManifestResourceStream("UnofficialCrusaderPatch.license.txt"))
                using (StreamReader sr = new StreamReader(stream))
                    linkLabel.Inlines.Add("\n\n\n\n\n\n" + sr.ReadToEnd());
            }
            catch (Exception e)
            {
                Debug.Error(e.ToString());
            }
        }

        void TextBlock_Initialized(object sender, EventArgs e)
        {
            try
            {
                TextBlock tb = (TextBlock)sender;
                string ident = tb.Text;
                tb.Text = null;
                TextReferencer.SetText(tb, ident);
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.ToString());
            }
        }

        #region Path finding

        void pButtonUninstall_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception exc)
            {
                Debug.Error(exc);
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
            if (Patcher.GetOriginalBinary(pTextBoxPath.Text) == null)
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

        void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AIVLimitReached();
        }

        void iButtonBack_Click(object sender, RoutedEventArgs e)
        {
            installGrid.Visibility = Visibility.Hidden;
            pathGrid.Visibility = Visibility.Visible;
        }

        void iButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            string filePath = Patcher.GetOriginalBinary(pTextBoxPath.Text);
            if (filePath == null)
            {
                Debug.Error(Localization.Get("ui_wrongpath"));
                return;
            }

            if (AIVLimitReached())
            {
                Debug.Error(Localization.Get("ui_aivlimit"));
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
            setupThread.Start(filePath);
        }

        Thread setupThread;
        void DoSetup(object arg)
        {
            try
            {
                Patcher.Install((string)arg, SetPercent);

                Dispatcher.Invoke(() => iButtonInstall.IsEnabled = true, DispatcherPriority.Render);
            }
            catch (Exception e)
            {
                if (!(e is TaskCanceledException || e is ThreadAbortException)) // in case of exit
                    MessageBox.Show(e.ToString(), Localization.Get("ui_error"));
            }
        }

        void SetPercent(double value)
        {
            Dispatcher.Invoke(() => pbSetup.Value = value * 100.0, DispatcherPriority.Render);
        }

        void cbBugfix_Check(object sender, RoutedEventArgs e)
        {
            foreach (Change c in Version.Changes)
                if (c.Type == ChangeType.Bugfix)
                    c.IsChecked = (bool)cbBugfixes.IsChecked;
            tviBugfixes.Items.Refresh();
        }

        void cbBalancing_Check(object sender, RoutedEventArgs e)
        {
            foreach (Change c in Version.Changes)
                if (c.Type == ChangeType.Balancing)
                    c.IsChecked = (bool)cbBalancing.IsChecked;
            tviBalancing.Items.Refresh();
        }

        bool AIVLimitReached()
        {
            bool result = Version.Changes.Where(c => c.IsChecked && c is AIVChange).Count() > 1;

            foreach (Change change in Version.Changes)
                change.Marked = (result && change.IsChecked && change is AIVChange);

            tviBalancing.Items.Refresh();
            tviBugfixes.Items.Refresh();

            return result;
        }

        #endregion
    }
}
