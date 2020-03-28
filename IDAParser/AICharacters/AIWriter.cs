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
        StringBuilder sb;
        StreamWriter sw;
        int sections;
        bool commenting;
        public int Sections => sections;

        public AIWriter(Stream stream)
        {
            sw = new StreamWriter(stream);
            sb = new StringBuilder(20);
        }

        public void Dispose()
        {
            sw.Dispose();
            sb = null;
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

        public void OpenSec()
        {
            this.WriteLine("{");
            sections++;
        }

        public void CloseSec()
        {
            sections--;
            this.WriteLine("}");
        }

        public void OpenCommentSec()
        {
            commenting = true;
            this.WriteLine("/*");
        }

        public void CloseCommentSec()
        {
            this.WriteLine("*/");
            commenting = false;
        }
        
        public void Write(object o, string name = null)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            Type type = o.GetType();
            if (writeFuncs.TryGetValue(type, out WriteFunc func)
                || writeFuncs.TryGetValue(type.BaseType, out func))
            {
                func.Invoke(this, name, o);
            }
            else
            {
                this.WriteLine(name ?? type.Name);
                this.OpenSec();

                foreach(FieldInfo fi in type.GetFields(Flags))
                {
                    object value = fi.GetValue(o);                    
                    this.Write(value, fi.Name);
                }

                this.CloseSec();
            }
        }

        public const BindingFlags Flags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance;


        public void WriteInfo(Type type, string name = null, string comment = null)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            
            if (!String.IsNullOrWhiteSpace(name))
            {
                sb.Append(name);
                sb.Append(":\t");
            }

            sb.Append(type.Name);

            if (!String.IsNullOrWhiteSpace(comment))
            {
                sb.Append(",\t");
                sb.Append(comment);
            }

            this.WriteLine(sb.ToString());
            sb.Clear();

            if (!writeFuncs.ContainsKey(type) && !writeFuncs.ContainsKey(type.BaseType))
            {
                this.OpenSec();

                foreach (FieldInfo fi in type.GetFields(Flags))
                {
                    // get comment from attribute
                    object[] attributes = fi.GetCustomAttributes(typeof(RWComment), false);
                    string cmt = attributes.Length > 0 ? ((RWComment)attributes[0]).Comment : null;

                    this.WriteInfo(fi.FieldType, fi.Name, cmt);
                }

                this.CloseSec();
            }
        }

        delegate void WriteFunc(AIWriter w, string name, object o);
        static readonly Dictionary<Type, WriteFunc> writeFuncs = new Dictionary<Type, WriteFunc>()
        {
            { typeof(bool), WriteDefault },
            { typeof(int), WriteDefault },
            { typeof(string), WriteString },
            { typeof(Enum), WriteDefault },
        };

        static void WriteDefault(AIWriter w, string name, object o)
        {
            w.WriteLine(string.Format("{0}\t= {1}", name, o));
        }

        static readonly string[] lineBreakArr = { "\n" };
        static void WriteString(AIWriter w, string name, object o)
        {
            string input = (string)o;
            if (!input.Contains('\n'))
            {
                WriteDefault(w, name, o);
                return;
            }

            w.WriteLine(name);
            w.OpenSec();

            string[] lines = input.Split(lineBreakArr, StringSplitOptions.None);
            foreach (string line in lines)
                w.WriteLine(line);

            w.CloseSec();
        }


        public void WriteMarkdown(Type type, string name = null, string comment = null)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            sb.Append("| ");
            if (!String.IsNullOrWhiteSpace(name))
            {
                sb.Append(name);
            }

            sb.Append(" | ");
            sb.Append(type.Name);
            sb.Append(" | ");

            if (!String.IsNullOrWhiteSpace(comment))
            {
                sb.Append(comment);
            }
            sb.Append(" |");

            this.WriteLine(sb.ToString());
            sb.Clear();

            if (!writeFuncs.ContainsKey(type) && !writeFuncs.ContainsKey(type.BaseType))
            {
                foreach (FieldInfo fi in type.GetFields(Flags))
                {
                    // get comment from attribute
                    object[] attributes = fi.GetCustomAttributes(typeof(RWComment), false);
                    string cmt = attributes.Length > 0 ? ((RWComment)attributes[0]).Comment : null;

                    this.WriteMarkdown(fi.FieldType, fi.Name, cmt);
                }
            }
        }
    }
}
