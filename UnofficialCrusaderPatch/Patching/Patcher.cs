using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UCP.Patching
{
    static class Patcher
    {
        public const string BackupIdent = "ucp_backup";
        public const string BackupFileEnding = "." + BackupIdent;

        const string CrusaderExe = "Stronghold Crusader.exe";
        const string XtremeExe = "Stronghold_Crusader_Extreme.exe";

        public static bool CrusaderExists(string folderPath)
        {
            return GetOriginalBinary(folderPath, CrusaderExe) != null
                  || GetOriginalBinary(folderPath, XtremeExe) != null;
        }

        static string GetOriginalBinary(string folderPath, string exe)
        {
            if (Directory.Exists(folderPath))
            {
                // remove old file ending from v1.0
                string path = Path.Combine(folderPath, exe + ".ori");
                if (File.Exists(path))
                {
                    string dest = path.Remove(path.Length - ".ori".Length);
                    File.Delete(dest);
                    File.Move(path, dest);
                }

                path = Path.Combine(folderPath, exe + BackupFileEnding);
                if (File.Exists(path))
                    return path;

                path = path.Remove(path.Length - BackupFileEnding.Length);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        public static void Install(string folderPath, Percentage.SetHandler SetPercent)
        {
            Percentage perc = new Percentage(SetPercent);
            perc.SetTotal(0);

            perc.NextLimit = 0.1;
            DoAIVChange(folderPath, perc);

            string cPath = GetOriginalBinary(folderPath, CrusaderExe);
            string ePath = GetOriginalBinary(folderPath, XtremeExe);

            if (cPath != null)
            {
                perc.NextLimit = ePath == null ? 1.0 : 0.55;
                DoChanges(cPath, false, perc);
            }

            if (ePath != null)
            {
                perc.NextLimit = 1;
                DoChanges(ePath, true, perc);
            }

            perc.SetTotal(1);
        }


        static void DoChanges(string filePath, bool xtreme, Percentage perc)
        {
            SectionEditor.Reset();

            // only take binary changes
            var changes = Version.Changes.Where(c => c.IsChecked && c.GetType() == typeof(Change));
            List<Change> todoList = new List<Change>(changes);

            int todoIndex = 0;
            double todoCount = 9 + todoList.Count; // +2 for AIprops +3 for read, +1 for version edit, +3 for writing data

            // read original data & section preparation
            byte[] oriData = File.ReadAllBytes(filePath);
            byte[] data = (byte[])oriData.Clone();
            SectionEditor.Init(data);
            todoIndex += 3;

            perc.Set(todoIndex / todoCount);

            ChangeArgs args = new ChangeArgs(data, oriData);    
            
            // change version display in main menu
            try
            {
                (xtreme ? Version.MenuChange_XT : Version.MenuChange).Activate(args);
            }
            catch (Exception e)
            {
                Debug.Error(e);
            }
            perc.Set(++todoIndex / todoCount);



            // change stuff
            foreach (Change change in todoList)
            {
                change.Activate(args);
                perc.Set(++todoIndex / todoCount);
            }



            // change AI properties
            AICChange.DoEdit(args);
            todoIndex += 2;
            perc.Set(todoIndex / todoCount);



            // Write everything to file

            data = SectionEditor.AttachSection(data);

            if (filePath.EndsWith(BackupFileEnding))
            {
                filePath = filePath.Remove(filePath.Length - BackupFileEnding.Length);
            }
            else
            {
                File.WriteAllBytes(filePath + BackupFileEnding, oriData); // create backup
            }

            File.WriteAllBytes(filePath, data);

            perc.Set(1);
            
            // Show failures
            if (fails.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Version Differences in ");
                sb.Append(Path.GetFileName(filePath));
                sb.AppendLine(":");
                foreach (var f in fails)
                    sb.AppendLine(f.Value + " " + f.Key);

                fails.Clear();
                Debug.Show(sb.ToString());
            }
        }

        public static void RestoreOriginals(string dir)
        {
            RestoreBinary(dir, CrusaderExe);
            RestoreBinary(dir, XtremeExe);
            RestoreAIVs(dir);
        }

        static void RestoreAIVs(string dir)
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

        static void RestoreBinary(string dir, string exe)
        {
            string oriPath = Path.Combine(dir, exe);
            string backupPath = oriPath + BackupFileEnding;
            if (File.Exists(backupPath))
            {
                if (File.Exists(oriPath))
                    File.Delete(oriPath);

                File.Move(backupPath, oriPath);
            }
        }

        static readonly Dictionary<string, EditFailure> fails = new Dictionary<string, EditFailure>();
        public static void AddFailure(string ident, EditFailure failure)
        {
            fails.Add(ident, failure);
        }

        static void DoAIVChange(string folderPath, Percentage perc)
        {
            DirectoryInfo aivDir = new DirectoryInfo(Path.Combine(folderPath, "aiv"));

            // Restore Backup
            RestoreAIVs(folderPath);

            if (AIVChange.ActiveChange != null)
            {
                // create backup of current AIVs
                string bupPath = Path.Combine(aivDir.FullName, BackupIdent);

                Directory.CreateDirectory(bupPath);
                foreach (FileInfo fi in aivDir.EnumerateFiles("*.aiv"))
                    fi.CopyTo(Path.Combine(bupPath, fi.Name), true);

                // copy modded AIVs
                AIVChange.ActiveChange.CopyAIVs(aivDir);
            }

            perc.Set(1.0);
        }
    }
}
