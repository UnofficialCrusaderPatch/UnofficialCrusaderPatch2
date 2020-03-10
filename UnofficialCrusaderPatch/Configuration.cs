using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UCP.Patching;
using UCP.AIC;

namespace UCP
{
    public static class Configuration
    {
        public const string ConfigFile = "ucp.cfg";

        private static Dictionary<String, String> settings = new Dictionary<string, string>()
        {
            { "Path", "" },
            { "Language", "-1" }
        };
        public static string Path
        {
            get
            {
                return settings["Path"];
            }
            set
            {
                settings["Path"] = value;
            }
        }

        public static int Language
        {
            get
            {
                return Convert.ToInt32(settings["Language"]);
            }
            set
            {
                settings["Language"] = value.ToString();
            }
        }

        static bool loading = false;

        public static void Save(string str = null)
        {
            if (loading) return;
            
            using (StreamWriter sw = new StreamWriter(ConfigFile))
            {
                // install path
                sw.Write("Path=");
                sw.WriteLine(Path);

                // language
                sw.Write("Language=");
                sw.WriteLine(Language);

                // edits
                foreach (Change change in Version.Changes)
                {
                    sw.WriteLine(change.ToString());
                }
            }
        }

        public static void Load(bool changesOnly = false, bool aionly = false)
        {
            if (File.Exists(ConfigFile))
            {
                using (StreamReader sr = new StreamReader(ConfigFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] changeLine = line.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).ToArray();
                        if (changeLine.Length < 2)
                        {
                            continue;
                        }
                        string changeKey = changeLine[0];
                        string changeSetting = changeLine[1];

                        if (!changesOnly)
                        {
                            if (changeKey == "Path")
                            {
                                Configuration.Path = changeSetting;
                            }
                            else if (changeKey == "Language")
                            {
                                if (int.TryParse(changeSetting, out int result))
                                    Configuration.Language = result;
                            }
                        } else
                        {
                            if (changeKey == "Path" || changeKey == "Language")
                            {
                                continue;
                            }
                            Change change = Version.Changes.Find(c => c.TitleIdent == changeKey);
                            if (change == null || (aionly == true && change.Type != ChangeType.AIC)) continue;

                            int numChanges = changeSetting.Count(ch => ch == '=');
                            string[] changes = changeSetting.Split(new char[] { '}' }, numChanges, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < numChanges; i++)
                            {
                                string headerKey = changes[i].Split('=')[0].Replace(" ", "");
                                string headerValue = changes[i].Split('=')[1].Replace(" ", "").Replace("{", "");
                                DefaultHeader header = change.FirstOrDefault(c => c.DescrIdent == headerKey);
                                header.LoadValueString(headerValue);
                            }
                        }
                    }
                }
                AICChange.Load();
            }
        }
    }
}
