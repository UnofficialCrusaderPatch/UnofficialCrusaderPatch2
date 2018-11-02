using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace CodeBlox
{
    public class CodeBlock
    {
        public const char Wildcard = '?';
        public const string WildcardStr = "?";

        public struct Element
        {
            byte value;
            public byte Value { get { return this.value; } }
            bool wildCard;
            public bool WildCard { get { return this.wildCard; } }

            public static implicit operator Element(byte input)
            {
                return new Element()
                {
                    value = input,
                    wildCard = false,
                };
            }

            public static implicit operator Element(bool wildcard)
            {
                return new Element()
                {
                    value = 0,
                    wildCard = wildcard,
                };
            }
        }

        Element[] elements;
        public int Length { get { return elements.Length; } }
        public IEnumerable<Element> Elements { get { return this.elements; } }

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
                            throw new Exception("Missing space after two characters!");

                        sb.Append(c);
                        continue;
                    }

                    if (c == Wildcard)
                    {
                        if (sb.Length == 2)
                            throw new Exception("Missing space after two characters!");

                        list.Add(true);
                        continue;
                    }

                    if (Char.IsWhiteSpace(c))
                    {
                        CheckConvert(sb, list);
                        continue;
                    }

                    throw new Exception("Illegal character in block: " + c);
                }
            }
            elements = list.ToArray();
        }

        void CheckConvert(StringBuilder sb, List<Element> list)
        {
            if (sb.Length < 1)
                return;

            string str = sb.ToString();
            if (!byte.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte result))
                throw new Exception("Could not convert " + str);

            list.Add(result);

            sb.Clear();
        }

        public int Seek(byte[] data, int startIndex)
        {
            int lastIndex = elements.Length - 1;
            for (int i = startIndex; i < data.Length - this.Length; i++)
            {
                for (int j = 0; j < elements.Length; j++)
                {
                    Element e = elements[j];
                    if (e.WildCard || e.Value == data[i + j])
                    {
                        if (j == lastIndex)
                            return i;
                    }
                    else break;
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
                index = this.Seek(data, index);
                if (index < 0)
                {
                    break;
                }
                else
                {
                    if (first == -1)
                        first = index;
                }
                count++;
                index++;
            }

            return count;
        }
    }
}
