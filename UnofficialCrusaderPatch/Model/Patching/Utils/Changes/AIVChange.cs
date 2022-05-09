using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using UCP.Model;
using UCP.Patching;

namespace UCP.AIV
{
    class AIVChange : Change
    {
        string resFolder;

        internal AIVChange(string identifier)
            : base(identifier, ChangeType.AIV)
        {
        }

        internal AIVChange withResFolder(string resFolder)
        {
            this.resFolder = resFolder;
            return this;
        }

        internal bool CopyAIVs(DirectoryInfo destinationDir, bool overwrite, bool graphical)
        {

            Assembly asm = Assembly.GetExecutingAssembly();
            List<string> resourceFiles = Directory.GetFiles(Path.Combine("resources", "aiv", resFolder)).Where(x => x.EndsWith(".aiv")).Select(x => Path.GetFileName(ReplaceFirst(x, resFolder + Path.PathSeparator, ""))).ToList();

            // If same AIV exist in both check if contents identical
            bool isIdentical = true;
            if (destinationDir.GetFiles().ToList().Select(x => x.Name).Where(x => x.EndsWith(".aiv")).SequenceEqual(resourceFiles, StringComparer.CurrentCultureIgnoreCase))
            {
                foreach (string aivFile in resourceFiles)
                {
                    using (SHA256 SHA256Instance = SHA256.Create())
                    {
                        using (Stream dstStream =
                            new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                        {
                            using (Stream srcStream = new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
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
                return true;
            }

            // If overwrite, delete aiv contents of aiv directory and write new AIV files
            if (overwrite)
            {
                foreach (FileInfo file in destinationDir.GetFiles())
                {
                    file.Delete();
                }

                foreach (string aivFile in resourceFiles)
                {
                    using (Stream dstStream =
                        new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenWrite())
                    {
                        using (Stream srcStream = new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                        {
                            srcStream.CopyTo(dstStream);
                        }
                    }
                }
                return true;
            }

            /**
             * This logic runs when the contents of the backup and destination aiv folders are different.
             * If an existing subfolder named 'original' does not exist in the destination aiv folder
             *  - Create folder 'original'
             *  - Copy all files from destination aiv folder to 'original' folder
             *  - Copy new aiv files to destination aiv folder
             */
            DirectoryInfo backupDir = new DirectoryInfo(Path.Combine(destinationDir.FullName, "original"));
            if (!Directory.Exists(Path.Combine(destinationDir.FullName, "original")) || backupDir.GetFiles().Length == 0)
            {
                if (Directory.EnumerateFiles(destinationDir.FullName, "*", SearchOption.TopDirectoryOnly).Select(x => x.EndsWith(".aiv")).ToList().Count > 0)
                {
                    backupDir.Create();
                }
                foreach (string aivFile in destinationDir.GetFiles().ToList().Select(x => x.Name))
                {
                    using (Stream dstStream =
                        new FileInfo(Path.Combine(backupDir.FullName, aivFile)).OpenWrite())
                    {
                        using (Stream srcStream = new FileInfo(Path.Combine(destinationDir.FullName, aivFile)).OpenRead())
                        {
                            srcStream.CopyTo(dstStream);
                        }
                        File.Delete(Path.Combine(destinationDir.FullName, aivFile));
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
                    foreach (FileInfo file in destinationDir.GetFiles())
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
                    MessageBoxResult result = System.Windows.MessageBox.Show(System.Windows.Application.Current.Resources["aiv_prompt"].ToString(), "", MessageBoxButton.YesNoCancel);
                    if (result == MessageBoxResult.No)
                    {
                        foreach (FileInfo file in destinationDir.GetFiles())
                        {
                            file.Delete();
                        }
                    }
                    else if (result == MessageBoxResult.Yes) // Clear destination aiv folder of aiv files.
                    {
                        using (var dialog = new FolderBrowserDialog())
                        {
                            dialog.Description = System.Windows.Application.Current.Resources["backup_aiv_select"].ToString();
                            dialog.RootFolder = Environment.SpecialFolder.Desktop;
                            DialogResult folderResult = DialogResult.Cancel;
                            var thread = new Thread(obj => {
                                folderResult = dialog.ShowDialog();
                            });
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                            thread.Join();

                            if (folderResult == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                            {
                                DirectoryInfo savePath = new DirectoryInfo(dialog.SelectedPath);
                                string[] files = Directory.GetFiles(dialog.SelectedPath);

                                foreach (FileInfo file in destinationDir.GetFiles())
                                {
                                    file.MoveTo(Path.Combine(savePath.FullName, Path.GetFileName(file.Name)));
                                }
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else if (!backupIdentical)
                {
                    string input = "";
                    while (!input.ToLower().Equals("delete") && (input.IndexOfAny(Path.GetInvalidPathChars()) != -1 || (input.Equals("") || Directory.Exists(Path.Combine(destinationDir.FullName, input)))))
                    {
                        Console.WriteLine(System.Windows.Application.Current.Resources["aiv_cli_prompt"]);
                        input = Console.ReadLine().Replace("\n", "");
                    };

                    if (input.ToLower().Equals("delete"))
                    {
                        foreach (string file in Directory.EnumerateFiles(destinationDir.FullName, "*", SearchOption.TopDirectoryOnly))
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
                    using (Stream srcStream = new FileInfo(Path.Combine("resources", "aiv", resFolder, aivFile)).OpenRead())
                    {
                        srcStream.CopyTo(dstStream);
                    }
                }
            }
            return true;
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
