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
using UCP;
using UCP.Patching;
using System.Windows.Media.Imaging;
using UCP.Views;
using UCP.Data;
using UCP.Helper;
using System.Windows.Media;
using UCP.Structs;
using System.Windows.Forms;
using Application = System.Windows.Application;


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
        MainViewModel _vm;
        public MainWindow()
        {
            MainViewModel vm = new MainViewModel();
            _vm = vm;
            this.DataContext = vm;


            //Configuration.LoadGeneral();
            //Configuration.LoadChanges();

            #region Select Language
            LanguageSelection languageSelection = new LanguageSelection();
            languageSelection.DataContext = vm;
            languageSelection.ShowDialog();
            #endregion

            #region SelectPath
            String path = Utility.CheckCrusaderPath();
            PathSelection pathSelection = new PathSelection(path);
            pathSelection.GetValueOnClose += NewPath;
            pathSelection.ShowDialog();
            #endregion


            this.Title = string.Format("{0} {1}",Utility.GetText("Name"), Version.PatcherVersion);

            InitializeComponent();



            _vm.LoadConfig(Directory.GetCurrentDirectory()+"\\ucp.cfg");
            
            //test();
          
        }

        //private async void test()
        //{
        //    try
        //    {
        //        var checker = new GithubDownloader("Sh0wdown", "UnofficialCrusaderPatch", null); // uses your Application.ProductVersion
        //        UpdateType update = await checker.CheckUpdate();
        //        if (update != UpdateType.None)
        //        {
        //                checker.DownloadAsset("Converter.zip"); // opens it in the user's browser
                    
        //        }
        //    }
        //    catch (Exception e)
        //    {

                
        //    }
           
        //}

        private void NewPath(object sender, CustomEventArgs e)
        {
            _vm.StrongholdPath = e.Text;
        }
        
        private void SaveConfig_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            saveFileDialog.Title = "Save Config";
            saveFileDialog.Filter = "cfg files (*.cfg)|*.cfg";
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            DialogResult dr = saveFileDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _vm.SaveConfig(saveFileDialog.FileName);
                }
                catch (Exception em)
                {

                    Debug.Error(em.Message);
                }

            }
        }
        private void LoadConfig_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            //todo
            openFileDialog.Title = "Load Config";
            openFileDialog.Filter = "cfg files (*.cfg)|*.cfg";
            openFileDialog.Multiselect = false;
            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    _vm.LoadConfig(openFileDialog.FileName);
                }
                catch (Exception em)
                {

                    Debug.Error(em.Message);
                }
                
            }
        }



       
    }
}
