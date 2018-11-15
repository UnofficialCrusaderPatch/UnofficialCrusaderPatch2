using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace UnofficialCrusaderPatch
{
    static class Localization
    {
        public static int LanguageIndex = 0;

        static Dictionary<string, string[]> LocalStrings = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        public static string Get(string identifier)
        {
            if (LocalStrings.TryGetValue(identifier, out string[] texts))
            {
                if (LanguageIndex < texts.Length) return texts[LanguageIndex];
                else return string.Format("{{Missing Translation|{0}}}", identifier);
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

        static Localization()
        {
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                using (var s = asm.GetManifestResourceStream("UnofficialCrusaderPatch.Localization.Localization.txt"))
                using (Reader r = new Reader(s))
                {
                    List<string> texts = new List<string>(5);
                    while (r.ReadLine(out string line))
                    {
                        string ident = line;
                        if (!r.ReadLine(out line) || !line.StartsWith("{"))
                            throw new Exception(ident + " Missing opening bracket at line " + r.LineNumber);

                        while ((r.ReadString(out string text)))
                            texts.Add(text);

                        LocalStrings.Add(ident, texts.ToArray());
                        texts.Clear();
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
