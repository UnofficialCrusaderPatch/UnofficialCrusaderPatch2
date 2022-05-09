using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UCP.Patching;

namespace UCP.Model.Patching
{
    class Installer
    {

        public const string BackupIdent = "ucp_backup";
        public const string BackupFileEnding = "." + BackupIdent;

        const string CrusaderExe = "Stronghold Crusader.exe";
        const string XtremeExe = "Stronghold_Crusader_Extreme.exe";

        static Func<string, string> crusaderPath = (folderPath) => GetOriginalBinary(folderPath, CrusaderExe);
        static Func<string, string> extremePath = (folderPath) => GetOriginalBinary(folderPath, XtremeExe);

        static byte[] crusaderOriData;
        static byte[] crusaderData;

        static byte[] extremeOriData;
        static byte[] extremeData;

        static string crusaderFilePath;
        static string extremeFilePath;

        internal static ChangeArgs? crusaderArgs;
        internal static ChangeArgs? extremeArgs;

        public static string PatcherVersion = "2.14";

        // change version 0x424EF1 + 1
        public static readonly SubChange MenuChange = new SubChange()
        {
            BinRedirect.CreateEdit("menuversion", false, Encoding.ASCII.GetBytes("V1.%d UCP" + PatcherVersion + '\0'))
        };
        public static readonly SubChange MenuChange_XT = new SubChange()
        {
            BinRedirect.CreateEdit("menuversion", false, Encoding.ASCII.GetBytes("V1.%d-E UCP" + PatcherVersion + '\0'))
        };

        public static void RestoreOriginals(string dir)
        {
            RestoreBinary(dir, CrusaderExe);
            RestoreBinary(dir, XtremeExe);
        }

        public static void Initialize(string shcPath)
        {
            crusaderFilePath = crusaderPath(shcPath);
            crusaderfails.Clear();
            SectionEditor.Reset();

            // Read original data & perform section preparation adding .ucp section to binary
            if (crusaderFilePath != null)
            {
                crusaderOriData = File.ReadAllBytes(crusaderFilePath);
                crusaderData = (byte[])crusaderOriData.Clone();
                SectionEditor.Init(crusaderData);
                crusaderArgs = new ChangeArgs(crusaderData, crusaderOriData);

                // Change version display in main menu
                MenuChange.Activate(crusaderArgs.Value);
            }
        }

        public static void InitializeExtreme(string shcPath)
        {
            extremeFilePath = extremePath(shcPath);
            extremefails.Clear();
            SectionEditor.Reset();

            // Read original data & perform section preparation adding .ucp section to binary
            if (extremeFilePath != null)
            {
                extremeOriData = File.ReadAllBytes(extremeFilePath);
                extremeData = (byte[])extremeOriData.Clone();
                SectionEditor.Init(extremeData);
                extremeArgs = new ChangeArgs(extremeData, extremeOriData);

                // Change version display in main menu
                MenuChange_XT.Activate(extremeArgs.Value);
            }
        }

        internal static string WriteFinalize()
        {
            if (crusaderArgs != null)
            {
                // Write everything to file
                crusaderData = SectionEditor.AttachSection(crusaderData);
                if (crusaderFilePath.EndsWith(BackupFileEnding))
                {
                    crusaderFilePath = crusaderFilePath.Remove(crusaderFilePath.Length - BackupFileEnding.Length);
                }
                else
                {
                    File.WriteAllBytes(crusaderFilePath + BackupFileEnding, crusaderOriData); // create backup
                }
                File.WriteAllBytes(crusaderFilePath, crusaderData);
            }
            string crusaderFailures = ListFailures(crusaderFilePath);
            return crusaderFailures;
        }


        internal static string WriteFinalizeExtreme()
        {
            if (extremeArgs != null)
            {
                // Write everything to file
                extremeData = SectionEditor.AttachSection(extremeData);
                if (extremeFilePath.EndsWith(BackupFileEnding))
                {
                    extremeFilePath = extremeFilePath.Remove(extremeFilePath.Length - BackupFileEnding.Length);
                }
                else
                {
                    File.WriteAllBytes(extremeFilePath + BackupFileEnding, extremeOriData); // create backup
                }
                File.WriteAllBytes(extremeFilePath, extremeData);
            }
            string extremeFailures = ListFailures(extremeFilePath);
            return extremeFailures;
        }









        // Retrieve path to the original SHC or SHC Extreme binary
        static string GetOriginalBinary(string folderPath, string exe)
        {
            if (Directory.Exists(folderPath))
            {
                // remove old file ending from v1.0
                string path = Path.Combine(folderPath, exe + ".ori");
                if (File.Exists(path))
                {
                    string dest = path.Remove(path.Length - ".ori".Length);
                    File.Delete(dest);
                    File.Move(path, dest);
                }

                path = Path.Combine(folderPath, exe + BackupFileEnding);
                if (File.Exists(path))
                    return path;

                path = path.Remove(path.Length - BackupFileEnding.Length);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }


        /// <summary>
        /// Rename the backed-up version of SHC and/or SHC Extreme executable
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="exe"></param>
        static void RestoreBinary(string dir, string exe)
        {
            string oriPath = Path.Combine(dir, exe);
            string backupPath = oriPath + BackupFileEnding;
            if (File.Exists(backupPath))
            {
                if (File.Exists(oriPath))
                    File.Delete(oriPath);

                File.Move(backupPath, oriPath);
            }
        }

        static string ListFailures(string filePath)
        {
            if (crusaderfails.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Version Differences in ");
                sb.Append(Path.GetFileName(filePath));
                sb.AppendLine(":");
                foreach (var f in crusaderfails)
                    sb.AppendLine(f.Ident + " " + f.Type);

                crusaderfails.Clear();
                return sb.ToString();
            }
            return null;
        }

        struct EditFail
        {
            public string Ident;
            public EditFailure Type;
        }
        static readonly List<EditFail> crusaderfails = new List<EditFail>();
        static readonly List<EditFail> extremefails = new List<EditFail>();
        public static void AddFailure(string ident, EditFailure failure)
        {
            crusaderfails.Add(new EditFail() { Ident = ident, Type = failure });
        }

        public static void AddExtremeFailure(string ident, EditFailure failure)
        {
            extremefails.Add(new EditFail() { Ident = ident, Type = failure });
        }
    }
}
