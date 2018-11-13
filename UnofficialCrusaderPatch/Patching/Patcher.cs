using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnofficialCrusaderPatch
{
    static class Patcher
    {
        const string BackupIdent = "ucp_backup";
        const string BackupFileEnding = "." + BackupIdent;

        public static string GetOriginalBinary(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                // remove old file ending from v1.0
                string path = Path.Combine(folderPath, "Stronghold Crusader.exe.ori");
                if (File.Exists(path))
                {
                    string dest = path.Remove(path.Length - ".ori".Length);
                    File.Delete(dest);
                    File.Move(path, dest);
                }

                path = Path.Combine(folderPath, "Stronghold Crusader.exe" + BackupFileEnding);
                if (File.Exists(path))
                    return path;

                path = path.Remove(path.Length - BackupFileEnding.Length);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        public delegate void SetPercentHandler(double percent);
        public static void Install(string filePath, SetPercentHandler SetPercent = null)
        {
            if (!File.Exists(filePath))
                throw new Exception("File not found!");

            SetPercent?.Invoke(0);

            // Do aiv folder backup
            string dir = Path.Combine(Path.GetDirectoryName(filePath), "aiv", BackupIdent);
            DirectoryInfo backupDir = new DirectoryInfo(dir);
            DirectoryInfo aivDir = backupDir.Parent;
            if (backupDir.Exists)
            {
                // restore originals
                foreach (FileInfo fi in backupDir.EnumerateFiles("*.aiv"))
                    fi.CopyTo(Path.Combine(aivDir.FullName, fi.Name), true);
            }
            else
            {
                // create backup
                backupDir.Create();
                foreach (FileInfo fi in aivDir.EnumerateFiles("*.aiv"))
                    fi.CopyTo(Path.Combine(backupDir.FullName, fi.Name));
            }

            DoChanges(filePath, aivDir, SetPercent);
            SetPercent?.Invoke(1);
        }


        static void DoChanges(string filePath, DirectoryInfo aivFolder, SetPercentHandler SetPercent)
        {
            List<Change> todo = new List<Change>(Version.Changes.Where(c => c.IsChecked));

            int index = 0;
            double count = 4 + todo.Count; // +1 from folder backup above, +1 for read, +1 for version edit, +1 for writing data
            SetPercent?.Invoke(++index / count);



            // read original data & preparation
            byte[] oriData = File.ReadAllBytes(filePath);
            byte[] data = (byte[])oriData.Clone();
            ChangeArgs args = new ChangeArgs(data, oriData, aivFolder);
            SetPercent?.Invoke(++index / count);



            // change version display in main menu
            var displayResult = Version.MenuChange.Activate(args);
            if (displayResult != EditResult.NoErrors)
            {
                const string str = "Your version is currently unsupported: {0} in menu display edit.";
                string message = string.Format(str, displayResult);
                throw new Exception(message);
            }
            SetPercent?.Invoke(++index / count);



            // change stuff
            foreach (Change change in todo)
            {
                change.Activate(args);
                SetPercent?.Invoke(++index / count);
            }



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

        public static void RestoreOriginals(string dirPath)
        {
            // restore binary
            string backupPath = Path.Combine(dirPath, "Stronghold Crusader.exe." + BackupIdent);
            if (File.Exists(backupPath))
            {
                string oriPath = backupPath.Remove(backupPath.Length - (BackupIdent.Length + 1));
                if (File.Exists(oriPath))
                    File.Delete(oriPath);

                File.Move(backupPath, oriPath);
            }

            // restore aiv folder
            backupPath = Path.Combine(dirPath, "aiv", BackupIdent);
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
    }
}
