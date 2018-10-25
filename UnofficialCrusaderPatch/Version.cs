using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace UnofficialCrusaderPatch
{
    class Version
    {
        #region Hashing

        public const string BackupEnding = ".ori";

        static byte[] originalHash = new byte[16] { 0xBC, 0x65, 0x24, 0xAF, 0xF1, 0x83, 0x1A, 0xFD, 0xF4, 0xEC, 0x07, 0x26, 0x77, 0x20, 0xBB, 0x6F };

        public enum Found
        {
            None,
            Normal,
            Backup,
        }

        public static Found SeekOriginal(string folderPath, out FileInfo file)
        {
            if (Directory.Exists(folderPath))
            {
                string path = Path.Combine(folderPath, "Stronghold Crusader.exe");

                file = new FileInfo(path);
                if (CheckVersion(file))
                    return Found.Normal;

                file = new FileInfo(path + BackupEnding);
                if (CheckVersion(file))
                    return Found.Backup;
            }
            file = null;
            return Found.None;
        }

        public static bool CheckVersion(FileInfo file)
        {
            if (file == null || !file.Exists)
                return false;

            byte[] computedHash;
            using (Stream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                computedHash = md5.ComputeHash(fs);
            }

            return computedHash.SequenceEqual(originalHash);
        }

        #endregion


        public static string Name { get { return name; } }
        static string name = "Inoffizieller Crusader Patch 1.0";

        public static IEnumerable<Change> Changes { get { return changes; } }
        static List<Change> changes = new List<Change>()
        {
            new ChangeInt32(0x4B*4 + 0xB4ED20, 3500, 
                "Armbrustschaden gegen Arab. Schwertkämpfer auf 3500 gesetzt."),
        };
    }
}
