using System;
using System.IO;

namespace UCP
{
    internal static class FileUtils
    {
        public static bool Transfer(String srcPath, String destPath, bool overwrite, bool log)
        {
            bool Message(string msg, bool result)
            {
                Console.WriteLine(msg);
                return result;
            }

            if (!Directory.Exists(srcPath))
            {
                return Message("Install failed. Source folder does not exist", false);
            }

            try
            {
                if (Directory.Exists(destPath) && overwrite == false)
                {
                    DirectoryInfo copyDir = Directory.CreateDirectory(Path.Combine(destPath, "bak-" + DateTime.Now.ToString("yyyy-MM-ddTHHmmss")));

                    if (log)
                    {
                        Console.WriteLine("Saving existing files to folder " + copyDir.FullName);
                    }

                    foreach (string file in Directory.EnumerateFiles(destPath))
                    {
                        File.Move(file, Path.Combine(copyDir.FullName, Path.GetFileName(file)));
                    }
                }
                Directory.CreateDirectory(destPath);
                if (log)
                {
                    Console.WriteLine("Copying the following files to destination " + destPath);
                }

                foreach (string file in Directory.EnumerateFiles(srcPath))
                {
                    if (log)
                    {
                        Console.WriteLine(Path.GetFileName(file));
                    }
                    File.Delete(Path.Combine(destPath, Path.GetFileName(file)));
                    File.Copy(file, Path.Combine(destPath, Path.GetFileName(file)));
                }
            } catch (Exception e)
            {
                return Message(e.Message, false);
            }

            return true;
        }
    }
}
