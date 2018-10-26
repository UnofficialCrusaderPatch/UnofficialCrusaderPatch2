using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace UnofficialCrusaderPatch
{
    static class Patcher
    {
        public delegate void SetPercentHandler(double percent);

        public static void Patch(string filePath, SetPercentHandler SetPercent = null)
        {
            var found = Version.SeekOriginal(filePath, out FileInfo file);
            if (found == Version.Found.Normal)
            {
                // create backup copy
                file.CopyTo(file.FullName + Version.BackupEnding, true);
            }
            else if (found == Version.Found.Backup)
            {
                // restore original and use that one instead
                string fullName = file.FullName;
                fullName = fullName.Remove(fullName.Length - Version.BackupEnding.Length);
                file.CopyTo(fullName, true);
                file = new FileInfo(fullName);
            }
            
            using (FileStream fs = file.Open(FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                int index = 0;
                double count = Version.Changes.Count() / 100.0;
                SetPercent?.Invoke(0);

                foreach(Change change in Version.Changes)
                {
                    change.Edit(fs);
                    SetPercent?.Invoke(++index / count);
                }

                SetPercent?.Invoke(100);
            }
        }
    }
}
