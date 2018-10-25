using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Reflection;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Diagnostics;

namespace UnofficialCrusaderPatch
{
    public partial class MainWindow : Window
    {
        // https://git.io/fxyw1

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this.Title = Version.Name + " - Setup";

                using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream("UnofficialCrusaderPatch.InfoText.txt"))
                using (StreamReader sr = new StreamReader(s))
                {
                    infoBox.Text = sr.ReadToEnd();
                }

                const string key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 40970";
                RegistryKey myKey = Registry.LocalMachine.OpenSubKey(key, false);
                if (myKey != null && myKey.GetValue("InstallLocation") is string path && !string.IsNullOrWhiteSpace(path))
                {
                    tbPath.Text = path;
                }
            }
            catch (Exception e)
            {
                File.WriteAllText("exceptions.txt", e.ToString());
            }
        }

        void SetLanguage(int index)
        {

        }

        #region Info

        void bInfoContinue_Click(object sender, RoutedEventArgs e)
        {
            infoGrid.Visibility = Visibility.Hidden;
            pathGrid.Visibility = Visibility.Visible;
        }

        #endregion

        #region Path finding

        void bPathSearch_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;
                dialog.SelectedPath = Directory.Exists(tbPath.Text) ? tbPath.Text : null;
                dialog.Description = "Bitte wähle dein Stronghold Crusader - Installationsverzeichnis.";

                var result = dialog.ShowDialog();
                if ((int)result == 1)
                    tbPath.Text = dialog.SelectedPath;
            }
        }

        void bPathBack_Click(object sender, RoutedEventArgs e)
        {
            pathGrid.Visibility = Visibility.Hidden;
            infoGrid.Visibility = Visibility.Visible;
        }

        string crusaderPath;
        void bPathInstall_Click(object sender, RoutedEventArgs e)
        {
            crusaderPath = tbPath.Text;
            if (Version.SeekOriginal(crusaderPath, out FileInfo whatever) == Version.Found.None)
            {
                MessageBox.Show("Nicht gefunden oder falsche Version! Steam Version benötigt.", "Fehler");
                return;
            }

            pathGrid.Visibility = Visibility.Hidden;
            setupGrid.Visibility = Visibility.Visible;

            setupThread = new Thread(DoSetup);
            setupThread.Start();
        }

        #endregion

        #region Setup

        Thread setupThread;
        void bSetupFinish_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        void DoSetup()
        {
            try
            {
                Thread.Sleep(100);
                Patcher.Patch(crusaderPath, SetPercent);
            }
            catch (Exception e)
            {
                if (!(e is System.Threading.Tasks.TaskCanceledException)) // in case of exit
                    File.WriteAllText("exceptions.txt", e.ToString());
            }
        }

        void SetPercent(double value, string textLine)
        {
            Dispatcher.Invoke(() =>
            {
                pbSetup.Value = value;
                setupBox.Text += textLine + "\n";
            }, DispatcherPriority.Render);
            Thread.Sleep(100);
        }

        #endregion

        void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            // start browser link
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
