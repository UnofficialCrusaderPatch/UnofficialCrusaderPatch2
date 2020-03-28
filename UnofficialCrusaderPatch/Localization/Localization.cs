using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace UCP
{
    public static class Localization
    {
        public class Language
        {
            string name;
            public string Name => name;

            string ident;
            public string Ident => ident;

            string culture;
            public string Culture => culture;

            public Language(string name, string ident, string culture)
            {
                this.name = name;
                this.ident = ident;
                this.culture = culture;
            }
        }

        static int[] loadOrder = { 1, 0, 2, 3 };
        public static IEnumerable<int> IndexLoadOrder => loadOrder;

        static List<Language> translations = new List<Language>()
        {
            new Language("Deutsch", "German", "de"),
            new Language("English", "English", "en"),
            new Language("Polski", "Polish", "pl"),
            new Language("Русский", "Russian", "ru"),
            new Language("中文", "Chinese", "ch")
        };
        public static IEnumerable<Language> Translations => translations;
        public static int GetLangByCulture(string culture)
        {
            return translations.FindIndex(l => l.Culture == culture);
        }


        static Dictionary<string, string> localStrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void Add(string identifier, string text)
        {
            localStrs.Add(identifier, text);
        }

        public static void Remove(string identifier)
        {
            localStrs.Remove(identifier);
        }

        public static string Get(string identifier)
        {
            if (localStrs.TryGetValue(identifier, out string text))
            {
                return text;
            }
            return string.Format("{{Unknown Identifier: {0}}}", identifier);

        }

        class Reader : IDisposable
        {
            StreamReader sr;
            int lineNum = 0;
            public int LineNumber { get { return this.lineNum; } }

            public Reader(Stream stream)
            {
                sr = new StreamReader(stream);
            }

            public void Dispose()
            {
                sr.Dispose();
            }

            public bool ReadLine(out string line)
            {
                while (true)
                {
                    line = sr.ReadLine(); lineNum++;
                    if (line == null)
                        break;

                    line = line.Trim();
                    if (line.Length == 0)
                        continue;

                    if (!line.StartsWith("//"))
                        return true;
                }
                line = null;
                return false;
            }

            StringBuilder chars = new StringBuilder(100);
            public bool ReadString(out string text)
            {
                int c;
                int oldLineNum = lineNum;

                while (true)
                {
                    if ((c = sr.Read()) < 0)
                        throw new Exception("ReadString: End of file after line " + oldLineNum);

                    if (c == '\n')
                        lineNum++;

                    if (char.IsWhiteSpace((char)c))
                        continue;

                    if (c != '\"')
                    {
                        sr.ReadLine();
                        lineNum++;
                        text = null;
                        return false;
                    }
                    else
                    {
                        break;
                    }
                }

                oldLineNum = lineNum;
                while (true)
                {
                    if ((c = sr.Read()) < 0)
                        throw new Exception("Could not find string closing after line " + oldLineNum);

                    if (c == '&')
                    {
                        int peek = sr.Peek();
                        if (peek == '\r')
                        {
                            sr.Read();
                            sr.Read();
                            lineNum++;
                            continue;
                        }
                    }

                    if (c == '\n')
                    {
                        lineNum++;
                    }

                    if (c == '\"')
                        break;

                    chars.Append((char)c);
                }

                sr.ReadLine(); lineNum++;
                if (chars.Length > 0)
                {
                    text = chars.ToString();
                    chars.Clear();
                    return true;
                }
                else
                {
                    text = null;
                    return false;
                }
            }
        }

        static int langIndex;
        public static int LanguageIndex => langIndex;

        public static void Load(int index)
        {
            try
            {
                langIndex = index;

                string path = string.Format("UCP.Localization.{0}.txt", translations[index].Ident);

                Assembly asm = Assembly.GetExecutingAssembly();
                using (var s = asm.GetManifestResourceStream(path))
                using (Reader r = new Reader(s))
                {
                    localStrs.Clear();
                    while (r.ReadLine(out string line))
                    {
                        string ident = line;

                        if (!r.ReadLine(out line) || !line.StartsWith("{"))
                            throw new Exception(ident + " Missing opening bracket at line " + r.LineNumber);

                        if (!r.ReadString(out string text))
                            throw new Exception(ident + " Missing string at " + r.LineNumber);

                        if (!r.ReadLine(out line) || !line.StartsWith("}"))
                            throw new Exception(ident + " Missing closing bracket at line " + r.LineNumber);

                        localStrs.Add(ident, text);
                    }
                }
            }

            catch (Exception e)
            {
                Debug.Error(e);
            }
        }
    }
}
