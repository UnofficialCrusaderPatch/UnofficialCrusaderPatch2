using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UCP.AICharacters
{
    /// <summary>
    /// A tool to read text
    /// </summary>
    public class LineReader : IDisposable
    {
        StreamReader sr;

        public LineReader(Stream stream)
        {
            sr = new StreamReader(stream, Encoding.UTF8, true);
        }

        public void Dispose()
        {
            sr.Dispose();
        }

        public string ReadLine(bool skipEmpty = true)
        {
            string line;
            // read until EOS
            while ((line = sr.ReadLine()) != null)
            {
                // remove comments
                int commentIndex = line.IndexOf("//");
                if (commentIndex >= 0)
                {
                    line = line.Remove(commentIndex);
                }

                line = line.Trim(); // get rid of white spaces

                // empty line? continue reading if skipEmpty = true
                if (line.Length > 0 || !skipEmpty)
                    break;                    
            }
            return line;
        }

        public bool OpenSec(string title)
        {
            string line = ReadLine(true);
            if (line == null) return false;

            if (!string.Equals(line, title, StringComparison.OrdinalIgnoreCase))
                throw new FormatException("OpenSection: " + title);

            line = ReadLine(true);
            if (line != "{")
                throw new FormatException("OpenSection bracket");

            return true;
        }

        public void CloseSec()
        {
            string line = ReadLine(true);
            if (line != "}")
                throw new FormatException("CloseSection bracket");
        }

        public TEnum ReadEnum<TEnum>()
        {
            string line = this.ReadLine();

            int startIndex = line.IndexOf('=');
            if (startIndex < 0)
                throw new FormatException("ReadEnum '='");

            line = line.Substring(startIndex + 1).Trim();

            return (TEnum)Enum.Parse(typeof(TEnum), line, true);
        }
    }
}
