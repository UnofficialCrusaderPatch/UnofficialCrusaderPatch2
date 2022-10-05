using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UCP.AICharacters
{
    /// <summary>
    /// A tool to write text
    /// </summary>
    public class AIWriter : IDisposable
    {
        private StringBuilder sb;
        private StreamWriter  sw;
        private bool          commenting;
        public  int           Sections { get; private set; }

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
            for (int i = 0; i < Sections; i++)
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
            WriteLine("{");
            Sections++;
        }

        public void CloseSec()
        {
            Sections--;
            WriteLine("}");
        }

        public void OpenCommentSec()
        {
            commenting = true;
            WriteLine("/*");
        }

        public void CloseCommentSec()
        {
            WriteLine("*/");
            commenting = false;
        }
        
        public void Write(object o, string name = null)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }

            Type type = o.GetType();
            if (writeFuncs.TryGetValue(type, out WriteFunc func)
                || writeFuncs.TryGetValue(type.BaseType, out func))
            {
                func.Invoke(this, name, o);
            }
            else
            {
                WriteLine(name ?? type.Name);
                OpenSec();

                foreach(FieldInfo fi in type.GetFields(Flags))
                {
                    object value = fi.GetValue(o);                    
                    Write(value, fi.Name);
                }

                CloseSec();
            }
        }

        public const BindingFlags Flags = BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance;


        public void WriteInfo(Type type, string name = null, string comment = null)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

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

            WriteLine(sb.ToString());
            sb.Clear();

            if (writeFuncs.ContainsKey(type) || writeFuncs.ContainsKey(type.BaseType))
            {
                return;
            }

            OpenSec();

            foreach (FieldInfo fi in type.GetFields(Flags))
            {
                // get comment from attribute
                object[] attributes = fi.GetCustomAttributes(typeof(RWComment), false);
                string   cmt        = attributes.Length > 0 ? ((RWComment)attributes[0]).Comment : null;

                WriteInfo(fi.FieldType, fi.Name, cmt);
            }

            CloseSec();
        }

        private delegate void WriteFunc(AIWriter w, string name, object o);

        private static readonly Dictionary<Type, WriteFunc> writeFuncs = new Dictionary<Type, WriteFunc>
                                                                         {
                                                                             { typeof(bool), WriteDefault },
                                                                             { typeof(int), WriteDefault },
                                                                             { typeof(string), WriteString },
                                                                             { typeof(Enum), WriteDefault },
                                                                         };

        private static void WriteDefault(AIWriter w, string name, object o)
        {
            w.WriteLine($"{name}\t= {o}");
        }

        private static readonly string[] lineBreakArr = { "\n" };

        private static void WriteString(AIWriter w, string name, object o)
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
            {
                throw new ArgumentNullException("type");
            }

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

            WriteLine(sb.ToString());
            sb.Clear();

            if (writeFuncs.ContainsKey(type) || writeFuncs.ContainsKey(type.BaseType))
            {
                return;
            }

            foreach (FieldInfo fi in type.GetFields(Flags))
            {
                // get comment from attribute
                object[] attributes = fi.GetCustomAttributes(typeof(RWComment), false);
                string   cmt        = attributes.Length > 0 ? ((RWComment)attributes[0]).Comment : null;

                WriteMarkdown(fi.FieldType, fi.Name, cmt);
            }
        }
    }
}
