using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnofficialCrusaderPatch
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

        public delegate void SetPercentHandler(double percent);
        public static void Install(string folderPath, SetPercentHandler SetPercent = null)
        {
            SetPercent?.Invoke(0);

            // Do aiv folder backup
            string dir = Path.Combine(folderPath, "aiv", BackupIdent);
            DirectoryInfo backupDir = new DirectoryInfo(dir);
            DirectoryInfo aivDir = backupDir.Parent;
            if (backupDir.Exists)
            {
                // restore originals
                foreach (FileInfo fi in backupDir.EnumerateFiles("*.aiv"))
                    fi.CopyTo(Path.Combine(aivDir.FullName, fi.Name), true);

                backupDir.Delete(true);
            }

            DoChanges(folderPath, false, aivDir, SetPercent);
            DoChanges(folderPath, true, aivDir, SetPercent);

            SetPercent?.Invoke(1);

            if (fails.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("The following changes could not be provided probably due to version differences:");
                foreach (var f in fails)
                    sb.AppendLine(f.Value + " " + f.Key);

                Debug.Show(sb.ToString());
            }
        }


        static void DoChanges(string folderPath, bool xtreme, DirectoryInfo aivFolder, SetPercentHandler SetPercent)
        {
            string filePath = GetOriginalBinary(folderPath, xtreme ? XtremeExe : CrusaderExe);

            SectionEditor.Reset();

            List<Change> todo = new List<Change>(Version.Changes.Where(c => c.IsChecked));

            int index = 0;
            double count = 4 + todo.Count; // +1 from folder backup above, +1 for read, +1 for version edit, +1 for writing data
            SetPercent?.Invoke(++index / count);


            // read original data & section preparation
            byte[] oriData = File.ReadAllBytes(filePath);
            byte[] data = (byte[])oriData.Clone();
            SectionEditor.Init(data);

            ChangeArgs args = new ChangeArgs(data, oriData, aivFolder);
            SetPercent?.Invoke(++index / count);


            // change version display in main menu
            try
            {
                if (xtreme)
                    Version.MenuChange_XT.Activate(args);
                else
                    Version.MenuChange.Activate(args);
            }
            catch (Exception e)
            {
                Debug.Error(e);
            }
            SetPercent?.Invoke(++index / count);

            // change stuff
            foreach (Change change in todo)
            {
                change.Activate(args);
                SetPercent?.Invoke(++index / count);
            }

            data = SectionEditor.AttachSection(data);

            if (filePath.EndsWith(BackupFileEnding))
            {
                filePath = filePath.Remove(filePath.Length - BackupFileEnding.Length);
            }
            else
            {
                // create backup
                File.WriteAllBytes(filePath + BackupFileEnding, oriData);
            }

            File.WriteAllBytes(filePath, data);
        }

        public static void RestoreOriginals(string dir)
        {
            RestoreBinary(dir, CrusaderExe);
            RestoreBinary(dir, XtremeExe);

            // restore aiv folder
            string backupPath = Path.Combine(dir, "aiv", BackupIdent);
            if (Directory.Exists(backupPath))
            {
                string oriPath = backupPath.Remove(backupPath.Length - BackupIdent.Length);
                foreach (string file in Directory.EnumerateFiles(backupPath, "*.aiv"))
                {
                    File.Copy(file, oriPath + Path.GetFileName(file), true);
                }

                Directory.Delete(backupPath, true);
            }
        }

        static void RestoreBinary(string dir, string exe)
        {
            string oriPath = Path.Combine(dir, exe);
            string backupPath = Path.Combine(oriPath, BackupFileEnding);
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
    }
}
