using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using UCP.Patching;

namespace UCP.AIV
{
    class AIVChange : Change
    {
        public bool isInternal = false;
        
        string resFolder;
        const string BackupIdent = "ucp_backup";

        public static AIVChange activeChange = null;
        static List<AIVChange> _changes = new List<AIVChange>()
        {
            AIVChange.CreateDefault("Tatha"),
            AIVChange.CreateDefault("EvreyFixed"),
            AIVChange.CreateDefault("EvreyImproved"),
            AIVChange.CreateDefault("EvreyHistory"),
            AIVChange.CreateDefault("PitchWells"),
            AIVChange.CreateDefault("PitchSiege"),
        };
        
        private static string selectedChange = String.Empty;

        static AIVChange ActiveChange { get { return activeChange; } }

        public static List<AIVChange> changes { get { return _changes; } }

        /// <summary>
        /// Loads AIV sets from subfolders present in resources\aiv path using foldername as change title
        /// </summary>
        static AIVChange() {
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "aiv")))
            {
                foreach (string aivDir in Directory.EnumerateDirectories(Path.Combine(Environment.CurrentDirectory, "resources", "aiv"), "*", SearchOption.TopDirectoryOnly))
                {
                    changes.Add(CreateExternal(Path.GetFileName(aivDir.TrimEnd(Path.DirectorySeparatorChar))));
                }
            }
        }

        public AIVChange(string titleIdent, bool enabledDefault = false)
            : base("aiv_" + titleIdent, ChangeType.AIV, true, true)
        {
            this.resFolder = "UCP.AIV." + titleIdent;
            this.isInternal = true;
        }

        public AIVChange(string titleIdent, bool enabledDefault = false, bool isInternal = false)
            : base("aiv_" + titleIdent, ChangeType.AIV, true, true)
        {
            this.resFolder = titleIdent;
            this.isInternal = isInternal;
        }

        public override void InitUI()
        {
            if (!this.resFolder.StartsWith("UCP.AIV"))
            {
                string descr = GetLocalizedDescription(this.TitleIdent);
                descr = descr == String.Empty ? this.TitleIdent.Substring(4) : descr;
                Localization.Add(this.TitleIdent + "_descr", descr);
            }
            base.InitUI();
            this.titleBox.Background = this.resFolder.StartsWith("UCP.AIV") ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);

            if (!this.resFolder.StartsWith("UCP.AIV"))
            {
                ((TextBlock)this.titleBox.Content).Text = this.TitleIdent.Substring(4);
            }
            this.IsChecked = selectedChange.Equals(this.TitleIdent);
            this.titleBox.IsChecked = selectedChange.Equals(this.TitleIdent);
            if (this.IsChecked)
                activeChange = this;
        }

        public static AIVChange CreateDefault(string titleIdent, bool enabledDefault = false)
        {
            return new AIVChange(titleIdent, enabledDefault)
            {
                new DefaultHeader("aiv_" + titleIdent)
                {
                }
            };
        }

        public static AIVChange CreateExternal(string titleIdent, bool enabledDefault = false)
        {
            return new AIVChange(titleIdent, enabledDefault, false)
            {
                new DefaultHeader("aiv_" + titleIdent)
                {
                }
            };
        }

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Checked(sender, e);

            if (activeChange != null)
                activeChange.IsChecked = false;

            activeChange = this;
            selectedChange = this.TitleIdent;
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Unchecked(sender, e);

            if (activeChange == this)
            {
                activeChange = null;
                selectedChange = String.Empty;
            }
        }
        public static IEnumerable<string> GetConfiguration()
        {
            List<string> config = new List<string>();
            if (selectedChange != String.Empty)
            {
                config.Add(selectedChange + "= { " + selectedChange + "={True} }");
            }
            foreach (AIVChange change in changes)
            {
                if (selectedChange != null && !(change.TitleIdent.Equals(selectedChange))){
                    config.Add(change.TitleIdent + "= { " + change.TitleIdent + "={False} }");
                }
            }
            return config;
        }

        public static void LoadConfiguration(List<string> configuration = null)
        {
            if (configuration == null)
            {
                return;
            }

            foreach (string change in configuration)
            {
                string[] changeLine = change.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).ToArray();
                if (changeLine.Length < 2)
                {
                    continue;
                }

                string changeKey = changeLine[0];
                string changeSetting = changeLine[1];

                bool selected = Regex.Replace(@"\s+", "", changeSetting.Split('=')[1]).Contains("True");
                if (selected == true)
                {
                    selectedChange = changeKey;
                    activeChange = changes.Where(x => x.TitleIdent.Equals(changeKey)).First();
                }
            }
        }

        /// <summary>
        /// Copies AIV sets to aivDir (aiv subdirectory of SHC installation)
        /// </summary>
        /// <param name="aivDir"></param>
        internal bool CopyAIVs(DirectoryInfo destinationDir, bool overwrite, bool graphical)
        {
            bool IsInternal = this.resFolder.StartsWith("UCP.AIV");
            /*if (1 == 1) throw new Exception();*/
            Assembly asm = Assembly.GetExecutingAssembly();
            List<string> resourceFiles;
            if (IsInternal)
            {
                resourceFiles = asm.GetManifestResourceNames()
                    .Where(x => x.StartsWith(resFolder, StringComparison.OrdinalIgnoreCase)).Select(x => Path.GetFileName(x.Replace(resFolder + ".", ""))).ToList();
            }
            else
            {
                resourceFiles = Directory.GetFiles(Path.Combine("resources", "aiv", resFolder)).Where(x => x.EndsWith(".aiv")).Select(x => Path.GetFileName(ReplaceFirst(x, resFolder + Path.PathSeparator, ""))).ToList();
            }

            // If same AIV exist in both check if contents identical
            bool isIdentical = true;
            if (destinationDir.GetFiles().ToList().Select(x => x.Name).Where(x => x.EndsWith(".aiv")).SequenceEqual(resourceFiles, StringComparer.CurrentCultureIgnoreCase))
            {
                foreach (string aivFile in resourceFiles)
                {
                    using (SHA256 SHA256Instance = SHA256.Create())
                    {
                        using (Stream dstStream =
                            new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                        {
                            using (Stream srcStream = IsInternal ? asm.GetManifestResourceStream(resFolder + "." + aivFile) : new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                            {
                                if (!Convert.ToBase64String(SHA256Instance.ComputeHash(srcStream))
                                    .Equals(Convert.ToBase64String(SHA256Instance.ComputeHash(dstStream))))
                                {
                                    isIdentical = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                isIdentical = false;
            }

            if (isIdentical)
            {
                return true;
            }

            // If overwrite, delete aiv contents of aiv directory and write new AIV files
            if (overwrite)
            {
                foreach (FileInfo file in destinationDir.GetFiles())
                {
                    file.Delete();
                }

                foreach (string aivFile in resourceFiles)
                {
                    using (Stream dstStream =
                        new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenWrite())
                    {
                        using (Stream srcStream = IsInternal
                            ? asm.GetManifestResourceStream(resFolder + "." + aivFile)
                            : new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                        {
                            srcStream.CopyTo(dstStream);
                        }
                    }
                }
                return true;
            }

            /**
             * This logic runs when the contents of the backup and destination aiv folders are different.
             * If an existing subfolder named 'original' does not exist in the destination aiv folder
             *  - Create folder 'original'
             *  - Copy all files from destination aiv folder to 'original' folder
             *  - Copy new aiv files to destination aiv folder
             */
            DirectoryInfo backupDir = new DirectoryInfo(Path.Combine(destinationDir.FullName, "original"));
            if (!Directory.Exists(Path.Combine(destinationDir.FullName, "original")) || backupDir.GetFiles().Length == 0)
            {
                if (Directory.EnumerateFiles(destinationDir.FullName, "*", SearchOption.TopDirectoryOnly).Select(x => x.EndsWith(".aiv")).ToList().Count > 0)
                {
                    backupDir.Create();
                }
                foreach (string aivFile in destinationDir.GetFiles().ToList().Select(x => x.Name))
                {
                    using (Stream dstStream =
                        new FileInfo(Path.Combine(backupDir.FullName, aivFile)).OpenWrite())
                    {
                        using (Stream srcStream = new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                        {
                            srcStream.CopyTo(dstStream);
                        }
                        File.Delete(Path.Combine(destinationDir.FullName, aivFile));
                    }
                }
            }
            else
            {
                // Determine if the aiv files in destination folder are identical to backup folder
                bool backupIdentical = true;
                foreach (string aivFile in destinationDir.GetFiles().ToList().Where(x => x.Name.EndsWith(".aiv")).Select(x => x.Name))
                {
                    using (SHA256 SHA256Instance = SHA256.Create())
                    {
                        try
                        {
                            using (Stream dstStream =
                                new FileInfo(Path.Combine(backupDir.FullName, aivFile)).OpenRead())
                            {
                                using (Stream srcStream = new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                                {
                                    if (!Convert.ToBase64String(SHA256Instance.ComputeHash(srcStream))
                                        .Equals(Convert.ToBase64String(SHA256Instance.ComputeHash(dstStream))))
                                    {
                                        backupIdentical = false;
                                        break;
                                    }
                                }
                            }
                        }
                        catch (IOException)
                        {
                            backupIdentical = false;
                        }
                    }
                }

                // If backup contains all aiv files from destination folder then delete aiv from destination folder
                if (backupIdentical)
                {
                    foreach (FileInfo file in destinationDir.GetFiles())
                    {
                        if (file.Extension.Equals(".aiv"))
                        {
                            file.Delete();
                        }
                    }
                }

                /**
                  * This logic runs when the contents of the backup and destination aiv folders are different.
                  */
                if (!backupIdentical && graphical) // Clear backup folder of aiv files
                {
                    MessageBoxResult result = System.Windows.MessageBox.Show(Localization.Get("aiv_prompt"), "", MessageBoxButton.YesNoCancel);
                    if (result == MessageBoxResult.No)
                    {
                        foreach (FileInfo file in backupDir.GetFiles())
                        {
                            file.Delete();
                        }

                        foreach (string aivFile in destinationDir.GetFiles().ToList().Select(x => x.Name))
                        {
                            FileInfo srcFile = new FileInfo(Path.Combine(destinationDir.FullName, aivFile));
                            srcFile.MoveTo(Path.Combine(backupDir.FullName, aivFile));
                        }
                    }
                    else if (result == MessageBoxResult.Yes) // Clear destination aiv folder of aiv files.
                    {
                        using (var dialog = new FolderBrowserDialog())
                        {
                            dialog.Description = Localization.Get("backup_aiv_select");
                            dialog.RootFolder = Environment.SpecialFolder.Desktop;
                            DialogResult folderResult = DialogResult.Cancel;
                            var thread = new Thread(obj => {
                                folderResult = dialog.ShowDialog();
                            });
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();

                            if (folderResult == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                            {
                                DirectoryInfo savePath = new DirectoryInfo(dialog.SelectedPath);
                                string[] files = Directory.GetFiles(dialog.SelectedPath);

                                foreach (FileInfo file in destinationDir.GetFiles())
                                {
                                    file.MoveTo(Path.Combine(savePath.FullName, Path.GetFileName(file.Name)));
                                }
                            } else
                            {
                                throw new Exception();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (!backupIdentical)
                {
                    string input = "";
                    while (!input.ToLower().Equals("delete") && (input.IndexOfAny(Path.GetInvalidPathChars()) != -1 || (input.Equals("") || Directory.Exists(Path.Combine(destinationDir.FullName, input)))))
                    {
                        Console.WriteLine(Localization.Get("aiv_cli_prompt"));
                        input = Console.ReadLine().Replace("\n", "");
                    };

                    if (input.ToLower().Equals("delete"))
                    {
                        foreach (string file in Directory.EnumerateFiles(destinationDir.FullName, "*", SearchOption.TopDirectoryOnly))
                        {
                            File.Delete(file);
                        }
                    }
                    else
                    {
                        DirectoryInfo extraBackupDir = Directory.CreateDirectory(Path.Combine(destinationDir.FullName, input));
                        foreach (string file in Directory.EnumerateFiles(backupDir.FullName, "*", SearchOption.TopDirectoryOnly))
                        {
                            File.Move(file, Path.Combine(extraBackupDir.FullName, Path.GetFileName(file)));
                        }
                    }
                    foreach (string file in Directory.EnumerateFiles(destinationDir.FullName, "*", SearchOption.TopDirectoryOnly))
                    {
                        File.Move(file, Path.Combine(backupDir.FullName, Path.GetFileName(file)));
                    }
                }
            }

            foreach (string aivFile in resourceFiles)
            {
                using (Stream dstStream =
                    new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenWrite())
                {
                    using (Stream srcStream = IsInternal
                        ? asm.GetManifestResourceStream(resFolder + "." + aivFile)
                        : new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                    {
                        srcStream.CopyTo(dstStream);
                    }
                }
            }
            return true;
        }

        public static void Refresh()
        {
            changes.Clear();
            changes.Add(AIVChange.CreateDefault("Tatha"));
            changes.Add(AIVChange.CreateDefault("EvreyFixed", true));
            changes.Add(AIVChange.CreateDefault("EvreyImproved"));
            changes.Add(AIVChange.CreateDefault("EvreyHistory"));
            changes.Add(AIVChange.CreateDefault("PitchWells"));
            changes.Add(AIVChange.CreateDefault("PitchSiege"));

            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "aiv")))
            {
                foreach (string aivDir in Directory.EnumerateDirectories(Path.Combine(Environment.CurrentDirectory, "resources", "aiv"), "*", SearchOption.TopDirectoryOnly))
                {
                    changes.Add(CreateExternal(Path.GetFileName(aivDir.TrimEnd(Path.DirectorySeparatorChar))));
                }
            }


            Version.RemoveChanges(ChangeType.AIV);
            Version.Changes.AddRange(changes);
        }

        /// <summary>
        /// Restores the most recently backed-up AIV set found in the aiv subfolder of SHC installation
        /// </summary>
        /// <param name="dir"></param>
        internal static void Restore(string path)
        {
            DirectoryInfo destinationDir = new DirectoryInfo(Path.Combine(path, "aiv"));
            DirectoryInfo backupDir = new DirectoryInfo(Path.Combine(destinationDir.FullName, "original"));
            if (!backupDir.Exists)
            {
                return;
            }
            else
            {
                foreach (FileInfo file in destinationDir.GetFiles())
                {
                    if (file.Extension.Equals(".aiv"))
                    {
                        file.Delete();
                    }
                }

                foreach (FileInfo file in backupDir.GetFiles())
                {
                    file.MoveTo(Path.Combine(destinationDir.FullName, Path.GetFileName(file.Name)));
                }
                backupDir.Delete();
            }
        }

        /// <summary>
        /// Creates timestamped backup of existing AIV sets and copies selected AIV set to aivDir (aiv subdirectory of SHC installation)
        /// </summary>
        /// <param name="folderPath"></param>
        public static void DoChange(string folderPath, bool overwrite, bool graphical)
        {
            if (activeChange == null)
            {
                return;
            }
            activeChange.CopyAIVs(new DirectoryInfo(Path.Combine(folderPath, "aiv")), overwrite, graphical);
        }

        static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        private static String GetLocalizedDescription(string titleIdent)
        {
            string folderPath = Path.Combine("resources", "aiv", titleIdent.Substring(4));
            string currentLang = Localization.Translations.ToArray()[Localization.LanguageIndex].Ident;
            string descr = String.Empty;
            try
            {
                descr = ReadDescription(Path.Combine(folderPath, currentLang));
                if (descr == String.Empty)
                {
                    foreach (var lang in Localization.Translations)
                    {
                        try
                        {
                            descr = ReadDescription(Path.Combine(folderPath, lang.Ident));
                            if (descr != String.Empty)
                            {
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception)
            {
                foreach (var lang in Localization.Translations)
                {
                    try
                    {
                        descr = ReadDescription(Path.Combine(folderPath, lang.Ident));
                        if (descr != String.Empty)
                        {
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            return descr;
        }

        private static String ReadDescription(String file)
        {
            String text = File.ReadAllText(file + ".txt");
            return text.Substring(0, Math.Min(text.Length, 1000));
        }
    }
}
