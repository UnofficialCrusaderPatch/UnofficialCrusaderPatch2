using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace UCP.AICharacters
{
    /// <summary>
    /// A tool to write text
    /// </summary>
    public class AIWriter : IDisposable
    {
        StreamWriter sw;
        int sections;
        public int Sections => sections;

        public AIWriter(Stream stream)
        {
            sw = new StreamWriter(stream, Encoding.Default);
        }

        public void Dispose()
        {
            sw.Dispose();
        }

        public void WriteLine(string line = null, string comment = null)
        {
            for (int i = 0; i < sections; i++)
                sw.Write('\t'); // tab character

            if (string.IsNullOrWhiteSpace(comment))
            {
                sw.WriteLine(line);
            }
            else
            {
                sw.Write(line);
                sw.Write("\t// ");
                sw.WriteLine(comment);
            }
        }
        
        public void Write(object o, string name = null, string comment = null)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            Type type = o.GetType();
            if (writeTypes.Contains(type) || writeTypes.Contains(type.BaseType))
            {
                this.WriteLine(string.Format("{0}\t= {1}", name, o.ToString()), comment);
            }
            else
            {
                this.WriteLine(name ?? type.Name);
                this.WriteLine("{");
                sections++;

                foreach(FieldInfo fi in type.GetFields(Flags))
                {
                    object value = fi.GetValue(o);

                    // get comment from attribute
                    object[] attributes = fi.GetCustomAttributes(typeof(RWComment), false);
                    string cmt = attributes.Length > 0 ? ((RWComment)attributes[0]).Comment : null;
                    
                    this.Write(value, fi.Name, cmt);
                }

                sections--;
                this.WriteLine("}");
            }
        }

        public const BindingFlags Flags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance;

        static readonly Type[] writeTypes = new Type[]
        {
            typeof(int),
            typeof(uint),
            typeof(string),
            typeof(Enum),
        };
    }
}
