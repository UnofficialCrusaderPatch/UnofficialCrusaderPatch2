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
            // Armbrust dmg table: 0xB4ED20
            // Bogen dmg table: 0xB4EAA0
            // Lanzenträger hp: 10000

             // Armbrustschaden gegen Arab. Schwertkämpfer, original: 8000
            new ChangeInt32(0x4B*4 + 0xB4ED20, 3500),

            // Arab. Schwertkämpfer Angriffsanimation, ca. halbiert
            new ChangeBytes(0xB59CD0, 0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x10, 0x11, 0x12, 0x13, 0x14, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x00),
            
             // Armbrustschaden gegen Lanzenträger, original: 15000
            new ChangeInt32(0x18*4 + 0xB4ED20, 9999),
            
             // Bogenschaden gegen Lanzenträger, original: 3500
            new ChangeInt32(0x18*4 + 0xB4EAA0, 2000),
        };
    }
}
