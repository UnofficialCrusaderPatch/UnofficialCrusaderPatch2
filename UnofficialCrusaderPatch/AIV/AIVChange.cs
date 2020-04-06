using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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

        public void CopyAIVs(DirectoryInfo aivDir)
        {
            if (this.resFolder.StartsWith("UCP.AIV"))
            {
                int len = resFolder.Length + 1;
                Assembly asm = Assembly.GetExecutingAssembly();
                foreach (string res in asm.GetManifestResourceNames())
                {
                    if (!res.StartsWith(resFolder, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string path = Path.Combine(aivDir.FullName, res.Substring(len));
                    using (Stream stream = asm.GetManifestResourceStream(res))
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        stream.CopyTo(fs);
                    }
                }
            }
            else
            {
                foreach (var file in Directory.EnumerateFiles(Path.Combine("resources","aiv",this.resFolder), "*.aiv", SearchOption.TopDirectoryOnly))
                {
                    string path = Path.Combine(aivDir.FullName, Path.GetFileName(file));
                    File.Copy(file, path);
                }
            }
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

        public static void Restore(string dir)
        {
            string aivPath = Path.Combine(dir, "aiv");
            foreach (string file in Directory.EnumerateFiles(aivPath, "*.aiv", SearchOption.TopDirectoryOnly))
            {
                File.Delete(file);
            }
            try
            {
                DirectoryInfo bupDir = new DirectoryInfo(Path.Combine(aivPath)).GetDirectories().OrderByDescending(d => d.LastWriteTimeUtc).First();
                if (bupDir.Exists)
                {
                    foreach (FileInfo fi in bupDir.EnumerateFiles("*.aiv"))
                        fi.CopyTo(Path.Combine(aivPath, fi.Name), true);

                    bupDir.Delete(true);
                }
            }
            catch (InvalidOperationException) { }
        }

        public static void DoChange(string folderPath)
        {
            DirectoryInfo aivDir = new DirectoryInfo(Path.Combine(folderPath, "aiv"));

            if (AIVChange.ActiveChange != null)
            {
                // create backup of current AIVs
                DirectoryInfo bupDir = Directory.CreateDirectory(Path.Combine(aivDir.FullName, "bak-" + DateTime.Now.ToString("yyyy-MM-ddTHHmmss")));
                foreach (string file in Directory.EnumerateFiles(aivDir.FullName, "*.aiv", SearchOption.TopDirectoryOnly))
                {
                    File.Move(file, Path.Combine(bupDir.FullName, Path.GetFileName(file)));
                }
                // copy modded AIVs
                AIVChange.ActiveChange.CopyAIVs(aivDir);
            }
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
