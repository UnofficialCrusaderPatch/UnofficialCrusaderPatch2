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

        VersionHash version;
        public VersionHash Version { get { return this.version; } }

        bool backup;
        public bool Backup { get { return this.backup; } }

        public bool NotFound { get { return this.version == VersionHash.Unknown; } }

        public VersionInfo(FileInfo file, VersionHash version, bool backup)
        {
            this.file = file;
            this.version = version;
            this.backup = backup;
        }
    }

    static class Patcher
    {
        public const string BackupEnding = ".ori";

        public static VersionInfo SeekOriginal(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string path = Path.Combine(folderPath, "Stronghold Crusader.exe");

                FileInfo file = new FileInfo(path);
                VersionHash v = FileHash.CheckVersion(file);
                if (v != VersionHash.Unknown)
                    return new VersionInfo(file, v, false);

                file = new FileInfo(path + BackupEnding);
                v = FileHash.CheckVersion(file);
                if (v != VersionHash.Unknown)
                    return new VersionInfo(file, v, true);
            }
            return new VersionInfo(null, VersionHash.Unknown, false);
        }


        public delegate void SetPercentHandler(double percent);

        public static void Patch(VersionInfo info, SetPercentHandler SetPercent = null)
        {
            if (info.NotFound)
                throw new Exception("File not found!");

            FileInfo file = info.File;
            if (info.Backup)
            {
                // create backup copy
                file.CopyTo(file.FullName + BackupEnding, true);
            }
            else
            {
                // restore original and use that one instead
                string fullName = file.FullName;
                fullName = fullName.Remove(fullName.Length - BackupEnding.Length);
                file.CopyTo(fullName, true);
                file = new FileInfo(fullName);
            }

            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                int index = 0;
                double count = Version.Changes.Count() / 100.0;
                SetPercent?.Invoke(0);

                foreach (ChangeCollection change in Version.Changes)
                {
                    if (change.IsChecked)
                        change.Edit(fs);

                    SetPercent?.Invoke(++index / count);
                }

                SetPercent?.Invoke(100);
            }
        }
    }
}
