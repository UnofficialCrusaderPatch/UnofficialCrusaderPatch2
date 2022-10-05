using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CodeBlox
{
    public class CodeBlock
    {
        public const char Wildcard = '?';
        public const string WildcardStr = "?";

        public struct Element
        {
            public  byte Value { get; private set; }

            public  bool WildCard { get; private set; }

            public static implicit operator Element(byte input)
            {
                return new Element
                       {
                    Value = input,
                    WildCard = false,
                };
            }

            public static implicit operator Element(bool wildcard)
            {
                return new Element
                       {
                    Value = 0,
                    WildCard = wildcard,
                };
            }
        }

        private Element[]            elements;
        public  int                  Length   => elements.Length;
        public  IEnumerable<Element> Elements => elements;

        public CodeBlock(Stream stream)
        {
            List<Element> list = new List<Element>((int)stream.Length / 2);
            using (StreamReader sr = new StreamReader(stream))
            {
                StringBuilder sb = new StringBuilder(2);

                while (true)
                {
                    int next = sr.Read();
                    if (next < 0)
                    {   // last character
                        CheckConvert(sb, list);
                        break;
                    }

                    char c = (char)next;
                    if (Char.IsLetterOrDigit(c))
                    {
                        if (sb.Length == 2)
                        {
                            throw new Exception("Missing space after two characters!");
                        }

                        sb.Append(c);
                        continue;
                    }

                    if (c == Wildcard)
                    {
                        if (sb.Length == 2)
                        {
                            throw new Exception("Missing space after two characters!");
                        }

                        list.Add(true);
                        continue;
                    }

                    if (!Char.IsWhiteSpace(c))
                    {
                        throw new Exception("Illegal character in block: " + c);
                    }

                    CheckConvert(sb, list);
                }
            }
            elements = list.ToArray();
        }

        private static void CheckConvert(StringBuilder sb, List<Element> list)
        {
            if (sb.Length < 1)
            {
                return;
            }

            string str = sb.ToString();
            if (!byte.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte result))
            {
                throw new Exception("Could not convert " + str);
            }

            list.Add(result);

            sb.Clear();
        }

        public int Seek(byte[] data, int startIndex)
        {
            int lastIndex = elements.Length - 1;
            for (int i = startIndex; i < data.Length - Length; i++)
            {
                for (int j = 0; j < elements.Length; j++)
                {
                    Element e = elements[j];
                    if (e.WildCard || e.Value == data[i + j])
                    {
                        if (j == lastIndex)
                        {
                            return i;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return -1;
        }

        public int SeekCount(byte[] data, out int first)
        {
            first = -1;
            int count = 0;
            int index = 0;
            while (true)
            {
                index = Seek(data, index);
                if (index < 0)
                {
                    break;
                }

                if (first == -1)
                {
                    first = index;
                }

                count++;
                index++;
            }

            return count;
        }
    }
}
