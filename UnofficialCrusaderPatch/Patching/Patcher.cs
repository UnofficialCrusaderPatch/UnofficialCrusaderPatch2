using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnofficialCrusaderPatch
{
    struct VersionInfo
    {
        FileInfo file;
        public FileInfo File { get { return this.file; } }

        bool backup;
        public bool IsBackup { get { return this.backup; } }

        public bool NotFound { get { return file == null; } }

        public VersionInfo(FileInfo file, bool backup)
        {
            this.file = file;
            this.backup = backup;
        }
    }

    static class Patcher
    {
        public const string BackupIdent = "ucp_backup";

        public static VersionInfo SeekOriginal(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string path = Path.Combine(folderPath, "Stronghold Crusader.exe." + BackupIdent);

                FileInfo file = new FileInfo(path);
                if (file.Exists)
                    return new VersionInfo(file, true);

                file = new FileInfo(path.Remove(path.Length - (BackupIdent.Length + 1)));
                if (file.Exists)
                    return new VersionInfo(file, false);
            }
            return new VersionInfo(null, false);
        }

        public delegate void SetPercentHandler(double percent);
        public static void Install(VersionInfo info, SetPercentHandler SetPercent = null)
        {
            if (info.NotFound)
                throw new Exception("File not found!");

            SetPercent?.Invoke(0);

            // Do binary backup
            FileInfo file = info.File;
            if (info.IsBackup)
            {
                // restore original and use that one instead
                string fullName = file.FullName;
                fullName = fullName.Remove(fullName.Length - (1 + BackupIdent.Length));
                file.CopyTo(fullName, true);
                file = new FileInfo(fullName);
            }
            else
            {
                // create backup copy
                file.CopyTo(file.FullName + "." + BackupIdent, true);
            }

            // Do aiv folder backup
            DirectoryInfo aivDir = new DirectoryInfo(Path.Combine(file.DirectoryName, "aiv"));
            DirectoryInfo backupDir = new DirectoryInfo(Path.Combine(aivDir.FullName, BackupIdent));

            if (backupDir.Exists)
            {
                // restore originals
                foreach (FileInfo fi in backupDir.EnumerateFiles("*.aiv"))
                {
                    fi.CopyTo(Path.Combine(aivDir.FullName, fi.Name), true);
                }
            }
            else
            {
                // create backup
                backupDir.Create();
                foreach (FileInfo fi in aivDir.EnumerateFiles("*.aiv"))
                {
                    fi.CopyTo(Path.Combine(backupDir.FullName, fi.Name));
                }
            }

            DoChanges(file, aivDir, SetPercent);
            SetPercent?.Invoke(1);
        }


        static void DoChanges(FileInfo binFile, DirectoryInfo aivFolder, SetPercentHandler SetPercent)
        {
            int totalCount = Version.Changes.Count();
            List<BinaryChange> binTodo = new List<BinaryChange>(totalCount);
            foreach (Change change in Version.Changes)
            {
                if (change.IsChecked)
                {
                    if (change is BinaryChange bc)
                        binTodo.Add(bc);
                }
            }

            int index = 0;
            double count = binTodo.Count;

            AIVChange aivChange = (AIVChange)Version.Changes.FirstOrDefault(c => c.IsChecked && c is AIVChange);
            if (aivChange != null)
            {
                count++;
                aivChange.Activate(aivFolder);
                SetPercent?.Invoke(++index / count);
            }

            using (FileStream fs = binFile.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                byte[] oriData = new byte[fs.Length];
                fs.Read(oriData, 0, oriData.Length);

                foreach (BinaryChange change in binTodo)
                {
                    change.Edit(fs, oriData);
                    SetPercent?.Invoke(++index / count);
                }
            }
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
