using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Diagnostics;

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
                foreach (ChangeCollection c in Version.Changes)
                {
                    if (c.Type == ChangeType.Balancing)
                        tviBalancing.Items.Add(c);
                    else tviBugfixes.Items.Add(c);
                }

                // set translated ui elements
                pathBox.Text = Localization.Get("ui_searchpath");
                pButtonCancel.Content = Localization.Get("ui_cancel");
                pButtonContinue.Content = Localization.Get("ui_continue");
                pButtonSearch.Content = Localization.Get("ui_search");

                // set info text with github reference
                const string linkKeyword = "[ref]";
                string infoText = Localization.Get("ui_welcomeText");
                int index = infoText.IndexOf(linkKeyword);
                if (index < 0)
                {
                    linkLabel.Text = infoText;
                }
                else
                {
                    linkLabel.Inlines.Add(infoText.Remove(index));

                    Hyperlink hyperlink = new Hyperlink(new Run(Version.GitHubRef));
                    hyperlink.NavigateUri = new Uri(Version.GitHubRef);
                    hyperlink.RequestNavigate += (s, e) => Process.Start(Version.GitHubRef);
                    linkLabel.Inlines.Add(hyperlink);

                    linkLabel.Inlines.Add(infoText.Substring(index + linkKeyword.Length));
                }
            }
            catch (Exception e)
            {
                File.WriteAllText("exceptions.txt", e.ToString());
                MessageBox.Show(e.ToString(), "Error");
            }
        }

        #region Path finding

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
            if (Patcher.SeekOriginal(pTextBoxPath.Text).NotFound)
            {
                MessageBox.Show(Localization.Get("ui_wrongpath"), Localization.Get("ui_error"));
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
            VersionInfo info = Patcher.SeekOriginal(pTextBoxPath.Text);
            if (info.NotFound)
            {
                MessageBox.Show(Localization.Get("ui_wrongpath"), Localization.Get("ui_error"));
                return;
            }

            iButtonInstall.Content = Localization.Get("ui_finished");
            iButtonInstall.IsEnabled = false;
            iButtonInstall.Click -= iButtonInstall_Click;
            iButtonInstall.Click += (s, a) => this.Close();

            pButtonSearch.IsEnabled = false;
            pTextBoxPath.IsReadOnly = true;

            setupThread = new Thread(DoSetup);
            setupThread.Start(info);
        }

        Thread setupThread;
        void DoSetup(object arg)
        {
            try
            {
                VersionInfo info = (VersionInfo)arg;

                Thread.Sleep(100);
                Patcher.Patch(info, SetPercent);

                Dispatcher.Invoke(() => iButtonInstall.IsEnabled = true, DispatcherPriority.Render);
            }
            catch (Exception e)
            {
                if (!(e is System.Threading.Tasks.TaskCanceledException)) // in case of exit
                    MessageBox.Show(e.ToString(), Localization.Get("ui_error"));
            }
        }

        void SetPercent(double value)
        {
            Dispatcher.Invoke(() => pbSetup.Value = value, DispatcherPriority.Render);
            Thread.Sleep(10);
        }

        void cbBugfix_Check(object sender, RoutedEventArgs e)
        {
            foreach (ChangeCollection c in Version.Changes)
                if (c.Type == ChangeType.Bugfix)
                    c.IsChecked = (bool)cbBugfixes.IsChecked;
            tviBugfixes.Items.Refresh();
        }

        void cbBalancing_Check(object sender, RoutedEventArgs e)
        {
            foreach (ChangeCollection c in Version.Changes)
                if (c.Type == ChangeType.Balancing)
                    c.IsChecked = (bool)cbBalancing.IsChecked;
            tviBalancing.Items.Refresh();
        }

        #endregion
    }
}
