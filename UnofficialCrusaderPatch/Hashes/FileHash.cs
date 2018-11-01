using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Reflection;
using System.IO;

namespace UnofficialCrusaderPatch
{
    public enum VersionHash
    {
        Unknown,
        Steam141,
    }

    static class FileHash
    {
        struct HashPair
        {
            public VersionHash Version;
            public byte[] Hash;
        }

        readonly static List<HashPair> hashes = new List<HashPair>();

        static FileHash()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] names = asm.GetManifestResourceNames();
            foreach (VersionHash version in Enum.GetValues(typeof(VersionHash)))
            {
                string nameSpace = string.Format("UnofficialCrusaderPatch.Hashes.{0}.hash", version);
                if (!names.Contains(nameSpace))
                    continue;

                byte[] computedHash = new byte[32];
                using (Stream s = asm.GetManifestResourceStream(nameSpace))
                    s.Read(computedHash, 0, 32);

                hashes.Add(new HashPair()
                {
                    Version = version,
                    Hash = computedHash
                });
            }
        }

        public static VersionHash CheckVersion(FileInfo file)
        {
            if (file == null || !file.Exists)
                return VersionHash.Unknown;

            byte[] computedHash;
            using (Stream fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            using (SHA256 sha = new SHA256CryptoServiceProvider())
            {
                computedHash = sha.ComputeHash(fs);
            }

            foreach (HashPair pair in hashes)
                if (computedHash.SequenceEqual(pair.Hash))
                    return pair.Version;

            return VersionHash.Unknown;
        }
    }
}
