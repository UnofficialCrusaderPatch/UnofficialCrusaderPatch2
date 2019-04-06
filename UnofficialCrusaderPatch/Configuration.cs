using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UCP.Patching;

namespace UCP
{
    static class Configuration
    {
        public const string ConfigFile = "ucp.cfg";

        public static string Path = "";
        public static int Language = -1;

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
                    sw.Write(change.TitleIdent);
                    sw.Write("={ ");
                    foreach (DefaultHeader h in change)
                    {
                        sw.Write(h.DescrIdent);
                        sw.Write("={");
                        sw.Write(h.GetValueString());
                        sw.Write("} ");
                    }
                    sw.WriteLine("}");
                }
            }
        }

        public static void LoadGeneral()
        {
            loading = true;
            if (File.Exists(ConfigFile))
                using (StreamReader sr = new StreamReader(ConfigFile))
                {
                    string line;
                    int counter = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int index = line.IndexOf('=');
                        if (index < 0) continue;

                        string changeStr = line.Remove(index).Trim();
                        if (changeStr == "Path")
                        {
                            Path = line.Substring(index + 1);
                            counter++;
                        }
                        else if (changeStr == "Language")
                        {
                            if (int.TryParse(line.Substring(index + 1), out int result))
                                Language = result;

                            counter++;
                        }

                        if (counter >= 2)
                            break;
                    }
                }
            loading = false;
        }

        public static void LoadChanges()
        {
            loading = true;
            if (File.Exists(ConfigFile))
                using (StreamReader sr = new StreamReader(ConfigFile))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        int index = line.IndexOf('=');
                        if (index < 0) continue;

                        // change
                        string changeStr = line.Remove(index).Trim();
                        Change change = Version.Changes.Find(c => c.TitleIdent == changeStr);
                        if (change == null) continue;

                        int startIndex = line.IndexOf('{', index + 1);
                        if (startIndex < 0) continue;
                        startIndex++;

                        int endIndex = line.LastIndexOf('}');
                        if (endIndex < 0) continue;

                        // headers
                        while (true)
                        {
                            index = line.IndexOf('=', startIndex);
                            if (index < 0) break;

                            string headerStr = line.Substring(startIndex, index - startIndex).Trim();
                            startIndex = index + 1;

                            DefaultHeader header = change.FirstOrDefault(c => c.DescrIdent == headerStr);
                            if (header == null) continue;

                            startIndex = line.IndexOf('{', startIndex);
                            if (startIndex < 0)
                                break;

                            index = startIndex + 1;
                            startIndex = line.IndexOf('}', index);
                            if (startIndex < 0 || startIndex == endIndex)
                                break;

                            header.LoadValueString(line.Substring(index, startIndex - index));
                            startIndex++;
                        }
                    }
                }
            loading = false;
        }
    }
}
