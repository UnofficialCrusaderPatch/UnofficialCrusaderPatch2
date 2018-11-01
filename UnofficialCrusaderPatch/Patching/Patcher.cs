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
                string path = Path.Combine(folderPath, "Stronghold Crusader.exe" + BackupFileEnding);

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
            // count binary changes
            List<BinaryChange> binTodo = new List<BinaryChange>(Version.Changes.Count());
            foreach (Change change in Version.Changes)
            {
                if (change.IsChecked && change is BinaryChange bc)
                    binTodo.Add(bc);
            }

            int index = 0;
            double count = binTodo.Count;

            // aiv changes
            AIVChange aivChange = (AIVChange)Version.Changes.FirstOrDefault(c => c.IsChecked && c is AIVChange);
            if (aivChange != null)
            {
                count++;
                aivChange.Activate(aivFolder);
                SetPercent?.Invoke(++index / count);
            }

            // read original data
            byte[] oriData = File.ReadAllBytes(filePath);
            byte[] data = (byte[])oriData.Clone();

            foreach (BinaryChange change in binTodo)
            {
                change.Edit(data, oriData);
                SetPercent?.Invoke(++index / count);
            }

            if (filePath.EndsWith(BackupFileEnding))
            {
                filePath.Remove(filePath.Length - BackupFileEnding.Length);
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
