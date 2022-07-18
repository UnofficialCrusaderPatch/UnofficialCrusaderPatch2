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
            public  string Name { get; }

            public  string Ident { get; }

            public  string Culture { get; }

            public Language(string name, string ident, string culture)
            {
                Name = name;
                Ident = ident;
                Culture = culture;
            }
        }

        private static int[] loadOrder = { 1, 0, 2, 3, 4};
        public static IEnumerable<int> IndexLoadOrder => loadOrder;

        private static List<Language> translations = new List<Language>
                                                     {
                                                         new Language("Deutsch", "German", "de"),
                                                         new Language("English", "English", "en"),
                                                         new Language("Polski", "Polish", "pl"),
                                                         new Language("Русский", "Russian", "ru"),
                                                         new Language("中文", "Chinese", "ch"),
                                                         new Language("Magyar", "Hungarian", "hu")
                                                     };
        public static IEnumerable<Language> Translations => translations;
        public static int GetLangByCulture(string culture)
        {
            return translations.FindIndex(l => l.Culture == culture);
        }


        private static Dictionary<string, string> localStrs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static void Add(string identifier, string text)
        {
            if (localStrs.ContainsKey(identifier))
            {
                localStrs[identifier] = text;
            } 
            else
            {
                localStrs.Add(identifier, text);
            }
        }

        public static void Remove(string identifier)
        {
            if (localStrs.ContainsKey(identifier))
            {
                localStrs.Remove(identifier);
            }
        }

        public static string Get(string identifier)
        {
            if (localStrs.TryGetValue(identifier, out string text))
            {
                return text;
            }
            return $"{{Unknown Identifier: {identifier}}}";

        }

        private class Reader : IDisposable
        {
            private StreamReader sr;
            public  int          LineNumber { get; private set; }

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
                    line = sr.ReadLine(); LineNumber++;
                    if (line == null)
                    {
                        break;
                    }

                    line = line.Trim();
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (!line.StartsWith("//"))
                    {
                        return true;
                    }
                }
                line = null;
                return false;
            }

            private StringBuilder chars = new StringBuilder(100);
            public bool ReadString(out string text)
            {
                int c;
                int oldLineNum = LineNumber;

                while (true)
                {
                    if ((c = sr.Read()) < 0)
                    {
                        throw new Exception("ReadString: End of file after line " + oldLineNum);
                    }

                    if (c == '\n')
                    {
                        LineNumber++;
                    }

                    if (char.IsWhiteSpace((char)c))
                    {
                        continue;
                    }

                    if (c != '\"')
                    {
                        sr.ReadLine();
                        LineNumber++;
                        text = null;
                        return false;
                    }

                    break;
                }

                oldLineNum = LineNumber;
                while (true)
                {
                    if ((c = sr.Read()) < 0)
                    {
                        throw new Exception("Could not find string closing after line " + oldLineNum);
                    }

                    if (c == '&')
                    {
                        int peek = sr.Peek();
                        if (peek == '\r')
                        {
                            sr.Read();
                            sr.Read();
                            LineNumber++;
                            continue;
                        }
                    }

                    if (c == '\n')
                    {
                        LineNumber++;
                    }

                    if (c == '\"')
                    {
                        break;
                    }

                    chars.Append((char)c);
                }

                sr.ReadLine(); LineNumber++;
                if (chars.Length > 0)
                {
                    text = chars.ToString();
                    chars.Clear();
                    return true;
                }

                text = null;
                return false;
            }
        }

        public static  int LanguageIndex { get; private set; }

        public static void Load(int index)
        {
            try
            {
                LanguageIndex = index;

                string path = $"UCP.Localization.{translations[index].Ident}.txt";

                Assembly asm = Assembly.GetExecutingAssembly();
                using (Stream s = asm.GetManifestResourceStream(path))
                using (Reader r = new Reader(s))
                {
                    localStrs.Clear();
                    while (r.ReadLine(out string line))
                    {
                        string ident = line;

                        if (!r.ReadLine(out line) || !line.StartsWith("{"))
                        {
                            throw new Exception(ident + " Missing opening bracket at line " + r.LineNumber);
                        }

                        if (!r.ReadString(out string text))
                        {
                            throw new Exception(ident + " Missing string at " + r.LineNumber);
                        }

                        if (!r.ReadLine(out line) || !line.StartsWith("}"))
                        {
                            throw new Exception(ident + " Missing closing bracket at line " + r.LineNumber);
                        }

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
