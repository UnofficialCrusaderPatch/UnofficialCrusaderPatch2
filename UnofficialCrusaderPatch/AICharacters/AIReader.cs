using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace UCP.AICharacters
{
    /// <summary>
    /// A tool to read text
    /// </summary>
    public class AIReader : IDisposable
    {
        StreamReader sr;
        int lineNum = 0;
        public int LineNumber => lineNum;

        public AIReader(Stream stream)
        {
            sr = new StreamReader(stream, Encoding.Default, true);
        }

        public void Dispose()
        {
            sr.Dispose();
        }

        public void Reset()
        {
            sr.DiscardBufferedData();
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            lineNum = 0;
        }

        public string ReadLine(bool skipEmpty = true)
        {
            string line;
            // read until EOS
            while ((line = sr.ReadLine()) != null)
            {
                lineNum++;
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

        public T Read<T>() where T : new()
        {
            string line;
            Type type = typeof(T);
            do
            {
                line = ReadLine();
                if (line == null)
                    return default(T);

            } while (line != type.Name);
            
            T result = new T();
            this.Read(result);
            return result;
        }

        const BindingFlags BFlags = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
        static readonly Type[] emptyTypeArg = new Type[0];
        static readonly object[] emptyObjectArg = new object[0];

        void Read(object o)
        {
            string line = ReadLine();
            if (line == null)
                throw new FormatException("EOS");
            if (line != "{")
                throw new FormatException("{");

            var fields = o.GetType().GetFields(BFlags);

            int fieldIndex = 0;
            while ((line = this.ReadLine()) != null)
            {
                if (line == "}")
                    break;

                // read by index
                FieldInfo fi = fields[fieldIndex++];
                Type fieldType = fi.FieldType;

                object value;
                if (readFuncs.TryGetValue(fieldType, out ReadFunc func)
                   || readFuncs.TryGetValue(fieldType.BaseType, out func))
                {
                    int startIndex = line.IndexOf('=');
                    if (startIndex < 0)
                        throw new FormatException("=");

                    string valueStr = line.Substring(startIndex + 1).Trim();
                    value = func?.Invoke(valueStr, fieldType);
                }
                else
                {
                    ConstructorInfo ci = fieldType.GetConstructor(emptyTypeArg);
                    value = ci.Invoke(emptyObjectArg);
                    this.Read(value);
                }
                fi.SetValue(o, value);
            }
        }

        delegate object ReadFunc(string valueStr, Type valueType);
        static readonly Dictionary<Type, ReadFunc> readFuncs = new Dictionary<Type, ReadFunc>()
        {
            { typeof(int), (v, vt) => int.Parse(v) },
            { typeof(uint), (v, vt) => uint.Parse(v) },
            { typeof(string), (v, vt) => v },
            { typeof(Enum), (v, vt) => Enum.Parse(vt, v, true) },
        };

    }
}
