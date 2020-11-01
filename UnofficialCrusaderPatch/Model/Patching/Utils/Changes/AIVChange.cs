using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using UCP.Model;
using UCP.Patching;

namespace UCP.AIV
{
    class AIVChange : Change
    {
        string resFolder;
        public bool IsInternal = false;

        internal AIVChange(string identifier)
            : base(identifier, ChangeType.AIV)
        {
        }

        internal AIVChange withResFolder(string resFolder)
        {
            this.resFolder = resFolder;
            return this;
        }

        internal AIVChange withInternalResFolder(string resFolder)
        {
            this.IsInternal = true;
            this.resFolder = "UCP.Resources.AIV." + resFolder;
            return this;
        }

        internal void CopyAIVs(DirectoryInfo destinationDir, bool overwrite, bool graphical)
        {
            
            Assembly asm = Assembly.GetExecutingAssembly();
            List<string> resourceFiles;
            if (IsInternal)
            {
                resourceFiles = asm.GetManifestResourceNames()
                    .Where(x => x.StartsWith(resFolder, StringComparison.OrdinalIgnoreCase)).Select(x => Path.GetFileName(x.Replace(resFolder + ".", ""))).ToList();
            }
            else
            {
                resourceFiles = Directory.GetFiles(Path.Combine("resources", "aiv", resFolder)).Where(x => x.EndsWith(".aiv")).Select(x => Path.GetFileName(ReplaceFirst(x, resFolder + Path.PathSeparator, ""))).ToList();
            }
             
            // If same AIV exist in both check if contents identical
            bool isIdentical = true;
            if (destinationDir.GetFiles().ToList().Select(x => x.Name).SequenceEqual(resourceFiles, StringComparer.CurrentCultureIgnoreCase))
            {
                foreach (string aivFile in resourceFiles)
                {
                    using (SHA256 SHA256Instance = SHA256.Create())
                    {
                        using (Stream dstStream =
                            new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                        {
                            using (Stream srcStream = IsInternal ? asm.GetManifestResourceStream(resFolder + "." + aivFile) : new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                            {
                                if (!Convert.ToBase64String(SHA256Instance.ComputeHash(srcStream))
                                    .Equals(Convert.ToBase64String(SHA256Instance.ComputeHash(dstStream))))
                                {
                                    isIdentical = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                isIdentical = false;
            }

            if (isIdentical)
            {
                return;
            }

            // If overwrite, delete aiv contents of aiv directory and write new AIV files
            if (overwrite)
            {
                MessageBoxResult msg = MessageBox.Show("overwrite", "", MessageBoxButton.YesNoCancel);
                foreach(FileInfo file in destinationDir.GetFiles())
                {
                    if (file.Extension.Equals(".aiv"))
                    {
                        file.Delete();
                    }
                }

                foreach (string aivFile in resourceFiles)
                {
                    using (Stream dstStream =
                        new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenWrite())
                    {
                        using (Stream srcStream = IsInternal
                            ? asm.GetManifestResourceStream(resFolder + "." + aivFile)
                            : new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                        {
                            srcStream.CopyTo(dstStream);
                        }
                    }
                }
                return;
            }

            /**
             * This logic runs when the contents of the backup and destination aiv folders are different.
             * If an existing subfolder named 'original' does not exist in the destination aiv folder
             *  - Create folder 'original'
             *  - Copy all files from destination aiv folder to 'original' folder
             *  - Copy new aiv files to destination aiv folder
             */
            DirectoryInfo backupDir = new DirectoryInfo(Path.Combine(destinationDir.FullName, "original"));
            if (!Directory.Exists(Path.Combine(destinationDir.FullName, "original")))
            {
                MessageBoxResult msg = MessageBox.Show("first backup", "", MessageBoxButton.YesNoCancel);
                backupDir.Create();
                foreach (string aivFile in destinationDir.GetFiles().ToList().Select(x => x.Name))
                {
                    using (Stream dstStream =
                        new FileInfo(Path.Combine(backupDir.FullName, aivFile)).OpenWrite())
                    {
                        using (Stream srcStream = new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                        {
                            srcStream.CopyTo(dstStream);
                        }
                    }
                }
            }
            else
            {
                // Determine if the aiv files in destination folder are identical to backup folder
                bool backupIdentical = true;
                foreach (string aivFile in destinationDir.GetFiles().ToList().Where(x => x.Name.EndsWith(".aiv")).Select(x => x.Name))
                {
                    using (SHA256 SHA256Instance = SHA256.Create())
                    {
                        try
                        {
                            using (Stream dstStream =
                                new FileInfo(Path.Combine(backupDir.FullName, aivFile)).OpenRead())
                            {
                                using (Stream srcStream = new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                                {
                                    if (!Convert.ToBase64String(SHA256Instance.ComputeHash(srcStream))
                                        .Equals(Convert.ToBase64String(SHA256Instance.ComputeHash(dstStream))))
                                    {
                                        backupIdentical = false;
                                        break;
                                    }
                                }
                            }
                        }
                        catch (IOException)
                        {
                            backupIdentical = false;
                        }
                    }
                }

                // If backup contains all aiv files from destination folder then delete aiv from destination folder
                if (backupIdentical)
                {
                    foreach(FileInfo file in destinationDir.GetFiles())
                    {
                        if (file.Extension.Equals(".aiv"))
                        {
                            file.Delete();
                        }
                    }
                }

                /**
                  * This logic runs when the contents of the backup and destination aiv folders are different.
                  */
                if (!backupIdentical && graphical) // Clear backup folder of aiv files
                {
                    MessageBoxResult result = MessageBox.Show(Application.Current.Resources["aiv_prompt"].ToString(), "", MessageBoxButton.YesNoCancel);
                    if (result == MessageBoxResult.Yes)
                    {
                        foreach (FileInfo file in backupDir.GetFiles())
                        {
                            if (file.Extension.Equals(".aiv"))
                            {
                                file.Delete();
                            }
                        }

                        foreach (string aivFile in destinationDir.GetFiles().ToList().Where(x => x.Name.EndsWith(".aiv")).Select(x => x.Name))
                        {
                            using (Stream dstStream =
                                new FileInfo(Path.Combine(backupDir.FullName, aivFile)).OpenWrite())
                            {
                                using (Stream srcStream = new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                                {
                                    srcStream.CopyTo(dstStream);
                                }
                            }
                        }
                    }
                    else if (result == MessageBoxResult.No) // Clear destination aiv folder of aiv files.
                    {
                        foreach (FileInfo file in destinationDir.GetFiles())
                        {
                            if (file.Extension.Equals(".aiv"))
                            {
                                file.Delete();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Installation cancelled");
                    }
                }
                else if (!backupIdentical)
                {
                    string input = "";
                    while (!input.ToLower().Equals("delete") && (input.IndexOfAny(Path.GetInvalidPathChars()) != -1 || (input.Equals("") || Directory.Exists(input)))) {
                        Console.WriteLine("Custom modified AIVs detected.Enter 'delete' to erase or enter folder name to copy files to.");
                        input = Console.ReadLine().Replace("\n","");
                    };

                    if (input.ToLower().Equals("delete"))
                    {
                        foreach (string file in Directory.EnumerateFiles(destinationDir.FullName, "*.aiv", SearchOption.TopDirectoryOnly))
                        {
                            File.Delete(file);
                        }
                    }
                    else
                    {
                        DirectoryInfo extraBackupDir = Directory.CreateDirectory(Path.Combine(destinationDir.FullName, input));
                        foreach (string file in Directory.EnumerateFiles(backupDir.FullName, "*", SearchOption.TopDirectoryOnly))
                        {
                            File.Move(file, Path.Combine(extraBackupDir.FullName, Path.GetFileName(file)));
                        }
                    }
                    foreach (string file in Directory.EnumerateFiles(destinationDir.FullName, "*", SearchOption.TopDirectoryOnly))
                    {
                        File.Move(file, Path.Combine(backupDir.FullName, Path.GetFileName(file)));
                    }
                }
            }

            foreach (string aivFile in resourceFiles)
            {
                using (Stream dstStream =
                    new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenWrite())
                {
                    using (Stream srcStream = IsInternal
                        ? asm.GetManifestResourceStream(resFolder + "." + aivFile)
                        : new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                    {
                        srcStream.CopyTo(dstStream);
                    }
                }
            }
        }



        static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

    }
}
