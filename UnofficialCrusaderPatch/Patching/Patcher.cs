using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UCP.AIC;
using UCP.AIV;
using UCP.Startup;

namespace UCP.Patching
{
    public static class Patcher
    {
        public const string BackupIdent = "ucp_backup";
        public const string BackupFileEnding = "." + BackupIdent;

        const string CrusaderExe = "Stronghold Crusader.exe";
        const string XtremeExe = "Stronghold_Crusader_Extreme.exe";

        /// <summary>
        /// Test existence of SHC of SHC Extreme executables inside specified directory
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static bool CrusaderExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return false;

            string path = Path.Combine(folderPath, CrusaderExe);
            if (File.Exists(path)) return true;

            path = Path.Combine(folderPath, XtremeExe);
            return File.Exists(path);
        }
        public static void Install(string folderPath, Percentage.SetHandler SetPercent, bool overwrite, bool graphical)
        {
            Percentage perc = new Percentage(SetPercent);
            perc.SetTotal(0);

            perc.NextLimit = 0.1;

            try {
                AIVChange.DoChange(folderPath, overwrite, graphical);
            } catch (Exception)
            {
                Debug.Show(Localization.Get("install_abort"));
                return;
            }
            perc.Set(1.0);

            DoChanges(folderPath, perc);

            perc.SetTotal(1);
        }

        public static void RestoreOriginals(string dir)
        {
            RestoreBinary(dir, CrusaderExe);
            RestoreBinary(dir, XtremeExe);
            AIVChange.Restore(dir);
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


        static void DoChanges(string folderPath, Percentage perc)
        {
            string cPath = GetOriginalBinary(folderPath, CrusaderExe);
            string ePath = GetOriginalBinary(folderPath, XtremeExe);

            if (cPath != null)
            {
                perc.NextLimit = ePath == null ? 1.0 : 0.55;
                DoBinaryChanges(cPath, false, perc);
            }

            if (ePath != null)
            {
                perc.NextLimit = 1;
                DoBinaryChanges(ePath, true, perc);
            }
        }


        static void DoBinaryChanges(string filePath, bool xtreme, Percentage perc)
        {
            fails.Clear();
            SectionEditor.Reset();

            // Retrieve set of selected binary changes
            var changes = Version.Changes.Where(c => c.IsChecked && c is Change && !(c is ResourceChange) && !(c is StartTroopChange));
            List<Change> todoList = new List<Change>(changes);

            int todoIndex = 0;
            double todoCount = 9 + todoList.Count; // +2 for AIprops +3 for read, +1 for version edit, +3 for writing data

            // Read original data & perform section preparation adding .ucp section to binary
            byte[] oriData = File.ReadAllBytes(filePath);
            byte[] data = (byte[])oriData.Clone();
            SectionEditor.Init(data);
            todoIndex += 3;

            perc.Set(todoIndex / todoCount);

            ChangeArgs args = new ChangeArgs(data, oriData);

            // Change version display in main menu
            try
            {
                (xtreme ? Version.MenuChange_XT : Version.MenuChange).Activate(args);
            }
            catch (Exception e)
            {
                Debug.Error(e);
            }
            perc.Set(++todoIndex / todoCount);

            // Apply each selected binary change
            foreach (Change change in todoList)
            {
                change.Activate(args);
                perc.Set(++todoIndex / todoCount);
            }

            // Apply changes handled in their respective submodules
            AICChange.DoChange(args);
            StartTroopChange.DoChange(args);
            ResourceChange.DoChange(args);

            todoIndex += 2;
            perc.Set(todoIndex / todoCount);



            // Write everything to file
            data = SectionEditor.AttachSection(data);

            if (filePath.EndsWith(BackupFileEnding))
            {
                filePath = filePath.Remove(filePath.Length - BackupFileEnding.Length);
            }
            else
            {
                File.WriteAllBytes(filePath + BackupFileEnding, oriData); // create backup
            }
            File.WriteAllBytes(filePath, data);

            perc.Set(1);

            ShowFailures(filePath);
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

        static void ShowFailures(string filePath)
        {
            if (fails.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Version Differences in ");
                sb.Append(Path.GetFileName(filePath));
                sb.AppendLine(":");
                foreach (var f in fails)
                    sb.AppendLine(f.Ident + " " + f.Type);

                fails.Clear();
                Debug.Show(sb.ToString());
            }
        }

        struct EditFail
        {
            public string Ident;
            public EditFailure Type;
        }
        static readonly List<EditFail> fails = new List<EditFail>();
        public static void AddFailure(string ident, EditFailure failure)
        {
            fails.Add(new EditFail(){ Ident=ident, Type=failure });
        }
    }
}
