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

        public static bool CrusaderExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return false;

            string path = Path.Combine(folderPath, CrusaderExe);
            if (File.Exists(path)) return true;

            path = Path.Combine(folderPath, XtremeExe);
            return File.Exists(path);
        }
        public static void Install(string folderPath, Percentage.SetHandler SetPercent)
        {
            Percentage perc = new Percentage(SetPercent);
            perc.SetTotal(0);

            perc.NextLimit = 0.1;
            AIVChange.DoChange(folderPath);
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
                // We need to clear global labels, because if we use the same Change for Extreme
                // or we name two labels the same in both Changes, then it will throw an entry
                // duplication exception for the GlobalLabels collection.
                GlobalLabels.Clear();
                
                perc.NextLimit = 1;
                DoBinaryChanges(ePath, true, perc);
            }
        }


        static void DoBinaryChanges(string filePath, bool xtreme, Percentage perc)
        {
            fails.Clear();
            SectionEditor.Reset();
            
            List<Change> changes = new List<Change>();

            // Get shallow copy of modifications.
            List<Mod> mods = Version.Modifications.GetRange(0, Version.Modifications.Count);

            // Go through modifications.
            foreach (Mod mod in mods)
            {
                // Get extreme version of mod.
                if (xtreme)
                {
                    // Init extreme change of mod.
                    mod.InitExtremeChange();
                    changes.Add(mod.ExtremeChange);
                }
                // Get normal version of mod.
                else
                {
                    changes.Add(mod.Change);
                }
            }

            int todoIndex = 0;
            double todoCount = 9 + changes.Count; // +2 for AIprops +3 for read, +1 for version edit, +3 for writing data

            // Read original data & section preparation.
            byte[] oriData = File.ReadAllBytes(filePath);
            byte[] data = (byte[])oriData.Clone();
            SectionEditor.Init(data);
            todoIndex += 3;

            perc.Set(todoIndex / todoCount);

            ChangeArgs args = new ChangeArgs(data, oriData);

            // Change version display in MainMenu.
            try
            {
                (xtreme ? Version.MenuChange_XT : Version.MenuChange).Activate(args);
            }
            catch (Exception e)
            {
                Debug.Error(e);
            }
            
            perc.Set(++todoIndex / todoCount);

            // Execute changes.
            foreach (Change change in changes)
            {
                change.Activate(args);
                perc.Set(++todoIndex / todoCount);
            }
            
            // Execute AIC changes.
            AICChange.DoChange(args);
            
            // Execute StartTroop changes.
            StartTroopChange.DoChange(args);
            
            // Execute Resource changes.
            ResourceChange.DoChange(args);

            todoIndex += 2;
            perc.Set(todoIndex / todoCount);



            // Write everything to file.
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
