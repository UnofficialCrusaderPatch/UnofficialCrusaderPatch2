using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using UCP.Patching;

namespace UCP.AIV
{
    class AIVChange : Change
    {
        string resFolder;
        const string BackupIdent = "ucp_backup";
        static AIVChange activeChange = null;
        static List<AIVChange> _changes = new List<AIVChange>()
        {
            AIVChange.CreateDefault("Tatha"),
            AIVChange.CreateDefault("EvreyFixed", true),
            AIVChange.CreateDefault("EvreyImproved"),
            AIVChange.CreateDefault("EvreyHistory"),
            AIVChange.CreateDefault("PitchWells"),
            AIVChange.CreateDefault("PitchSiege"),
        };

        static AIVChange ActiveChange { get { return activeChange; } }

        public static List<AIVChange> changes { get { return _changes; } }

        public AIVChange(string titleIdent, bool enabledDefault = false)
            : base("aiv_" + titleIdent, ChangeType.AIV, enabledDefault, true)
        {
            this.resFolder = "UCP.AIV." + titleIdent;
        }

        public override void InitUI()
        {
            base.InitUI();
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

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Checked(sender, e);

            if (activeChange != null)
                activeChange.IsChecked = false;

            activeChange = this;
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Unchecked(sender, e);

            if (activeChange == this)
                activeChange = null;
        }

        public void CopyAIVs(DirectoryInfo aivDir)
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

        public static void Restore(string dir)
        {
            string aivPath = Path.Combine(dir, "aiv");
            DirectoryInfo bupDir = new DirectoryInfo(Path.Combine(aivPath, BackupIdent));

            if (bupDir.Exists)
            {
                foreach (FileInfo fi in bupDir.EnumerateFiles("*.aiv"))
                    fi.CopyTo(Path.Combine(aivPath, fi.Name), true);

                bupDir.Delete(true);
            }
        }

        public static void DoChange(string folderPath)
        {
            DirectoryInfo aivDir = new DirectoryInfo(Path.Combine(folderPath, "aiv"));

            // Restore Backup
            Restore(folderPath);

            if (AIVChange.ActiveChange != null)
            {
                // create backup of current AIVs
                string bupPath = Path.Combine(aivDir.FullName);

                Directory.CreateDirectory(bupPath);
                foreach (string file in Directory.EnumerateFiles(aivDir.FullName, "*.aiv", SearchOption.TopDirectoryOnly))
                {
                    File.Move(file, Path.Combine(bupPath, file));
                }
                // copy modded AIVs
                AIVChange.ActiveChange.CopyAIVs(aivDir);
            }
        }
    }
}
