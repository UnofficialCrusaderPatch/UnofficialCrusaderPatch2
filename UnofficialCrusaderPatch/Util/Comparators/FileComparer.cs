using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;


namespace UCP.Util.Comparators
{
    // This implementation defines a very simple comparison  
    // between two FileInfo objects. It only compares the name  
    // of the files being compared and their length in bytes.  
    class FileComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo f1, FileInfo f2)
        {
            return SHA256CheckSum(f1).Equals(SHA256CheckSum(f2));
        }

        // Return a hash that reflects the comparison criteria. According to the
        // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
        // also be equal. Because equality as defined here is a simple value equality, not  
        // reference identity, it is possible that two or more objects will produce the same  
        // hash code.  
        public int GetHashCode(FileInfo fi)
        {
            return SHA256CheckSum(fi).GetHashCode();
        }

        public string SHA256CheckSum(FileInfo fi)
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                using (FileStream fileStream = fi.OpenRead())
                    return Convert.ToBase64String(SHA256.ComputeHash(fileStream));
            }
        }
    }
}
