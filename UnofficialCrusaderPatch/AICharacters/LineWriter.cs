using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UCP.AICharacters
{
    /// <summary>
    /// A tool to write text
    /// </summary>
    public class LineWriter : IDisposable
    {
        StreamWriter sw;
        int sections;
        public int Sections => sections;

        public LineWriter(Stream stream)
        {
            sw = new StreamWriter(stream, Encoding.UTF8);
        }

        public void Dispose()
        {
            sw.Dispose();
        }
        
        public void WriteLine(string line = null, string comment = null)
        {
            string indentation = new string('	', sections);
            sw.Write(indentation);

            if (string.IsNullOrWhiteSpace(comment))
            {
                sw.WriteLine(line);
            }
            else
            {
                sw.Write(line);
                sw.Write("	// ");
                sw.WriteLine(comment);
            }
        }

        public void OpenSec(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new FormatException("Title cannot be empty!");

            this.WriteLine(title);
            this.WriteLine("{");
            sections++;
        }

        public void CloseSec()
        {
            sections--;
            this.WriteLine("}");
        }

        public void WriteEnum(string fieldName, Enum value)
        {
            this.WriteLine(string.Format("{0}	= {1}", fieldName, value));
        }
    }
}
