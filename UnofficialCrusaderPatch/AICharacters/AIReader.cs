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
            sr = new StreamReader(stream, Encoding.ASCII, true);
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
            string line = ReadLine();
            if (line == null)
                return default(T);

            Type type = typeof(T);
            if (line != type.Name)
                throw new FormatException("name");

            T result = new T();
            this.Read(result);
            return result;
        }

        void Read(object o)
        {
            string line = ReadLine();
            if (line == null)
                throw new FormatException("EOS");
            if (line != "{")
                throw new FormatException("{");

            Dictionary<string, FieldInfo> fieldNames = GetFieldNames(o.GetType());
            while ((line = this.ReadLine()) != null)
            {
                if (line == "}")
                    break;

                // get name of field
                string fieldName;
                int endIndex = line.IndexOf('=');
                if (endIndex < 0)
                {
                    fieldName = line;
                }
                else
                {
                    fieldName = line.Remove(endIndex);
                }
                fieldName = fieldName.Trim();
                
                if (!fieldNames.TryGetValue(fieldName, out FieldInfo fi))
                    throw new FormatException(fieldName + " in line " + lineNum);
                
                object value;
                Type fieldType = fi.FieldType;
                if (readFuncs.TryGetValue(fieldType, out ReadFunc func)
                   || readFuncs.TryGetValue(fieldType.BaseType, out func))
                {
                    string valueStr = line.Substring(endIndex + 1).Trim();
                    value = func?.Invoke(valueStr, fieldType);
                }
                else
                {
                    ConstructorInfo ci = fieldType.GetConstructor(new Type[0]);
                    value = ci.Invoke(new object[0]);
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

        Dictionary<string, FieldInfo> GetFieldNames(Type type)
        {
            // get all fields from this type
            FieldInfo[] fields = type.GetFields(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

            // create dictionary
            var result = new Dictionary<string, FieldInfo>(fields.Length, StringComparer.OrdinalIgnoreCase);

            foreach (FieldInfo fi in fields)
            {
                result.Add(fi.Name, fi);

                // check for alternative / older names
                object[] attributes = fi.GetCustomAttributes(typeof(RWNames), false);
                if (attributes.Length > 0)
                {
                    foreach (string altName in ((RWNames)attributes[0]).Names)
                    {
                        if (result.TryGetValue(altName, out FieldInfo other))
                        {
                            if (other != fi)
                                throw new Exception(altName + " duplicate FieldName");
                        }
                        else
                        {
                            result.Add(altName, fi);
                        }
                    }
                }
            }

            return result;
        }
    }
}
