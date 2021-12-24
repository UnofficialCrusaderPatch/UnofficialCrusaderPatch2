﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using UCP.Model;
using UCP.Patching;

namespace UCP.AIV
{
    static class AIVEnumerator
    {
        static AIVChange activeChange = null;
        static List<AIVChange> _changes = new List<AIVChange>();
        static Dictionary<string, AIVConfiguration> aivConfig;
        static AIVChange ActiveChange { get { return activeChange; } }

        static AIVEnumerator()
        {
            aivConfig = new Dictionary<string, AIVConfiguration>();
            Load();
        }

        private static void Load()
        {
            try
            {
                if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "aiv")))
                {
                    foreach (string aivDir in Directory.EnumerateDirectories(Path.Combine(Environment.CurrentDirectory, "resources", "aiv"), "*", SearchOption.TopDirectoryOnly))
                    {
                        _changes.Add(CreateExternal(Path.GetFileName(aivDir.TrimEnd(Path.DirectorySeparatorChar))));
                    }
                }
            }
            catch (ArgumentException)
            {
                aivConfig = null;
            }
        }

        internal static Dictionary<string, AIVConfiguration> GetAIVConfiguration()
        {
            _changes.Clear();
            aivConfig.Clear();
            Load();
            ResetAIVConfiguration();
            return aivConfig;
        }

        internal static void SetAIVConfiguration(string aivName)
        {
            activeChange = _changes.Where(x => x.Identifier.Equals(aivName)).ToList().SingleOrDefault();
        }

        internal static void ResetAIVConfiguration()
        {
            activeChange = null;
        }

        internal static void Install(string path, bool overwrite, bool graphical)
        {
            if (activeChange == null)
            {
                return;
            }
            AIVChange change = _changes.Single(x => x.Identifier.Equals(activeChange.Identifier));
            change.CopyAIVs(new DirectoryInfo(Path.Combine(path, "aiv")), overwrite, graphical);
        }

        internal static void Uninstall(string path)
        {
            DirectoryInfo destinationDir = new DirectoryInfo(Path.Combine(path, "aiv"));
            DirectoryInfo backupDir = new DirectoryInfo(Path.Combine(destinationDir.FullName, "original"));
            if (!backupDir.Exists)
            {
                return;
            } else
            {
                foreach (FileInfo file in destinationDir.GetFiles())
                {
                    if (file.Extension.Equals(".aiv"))
                    {
                        file.Delete();
                    }
                }

                foreach (FileInfo file in backupDir.GetFiles())
                {
                    file.MoveTo(Path.Combine(destinationDir.FullName, Path.GetFileName(file.Name)));
                }
                backupDir.Delete();
            }
        }

        private static AIVChange CreateExternal(string identifier, bool enabledDefault = false)
        {
            AIVChange change = new AIVChange(identifier).withResFolder(identifier);
            change.Add(new DefaultSubChange(identifier) { });
            aivConfig.Add(change.Identifier, new AIVConfiguration().withDescription(GetDescriptions(change.Identifier)));
            return change;
        }

        private static Dictionary<string, string> GetDescriptions(string identifier)
        {
            List<string> Languages = new List<string>() { "English", "German", "Polish", "Russian", "Chinese", "Hungarian" };
            string folderPath = Path.Combine("resources", "aiv", identifier);

            Dictionary<string, string> descriptions = new Dictionary<string, string>();

            foreach(string language in Languages)
            {
                try
                {
                    descriptions.Add(language, ReadDescription(Path.Combine(folderPath, language)));
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return descriptions;
        }

        private static String ReadDescription(String file)
        {
            String text = File.ReadAllText(file + ".txt");
            return text.Substring(0, Math.Min(text.Length, 1000));
        }
    }
}
