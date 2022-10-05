using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace UCP.AICharacters
{
    /// <summary>
    /// A tool to read text
    /// </summary>
    public class AIReader : IDisposable
    {
        private StreamReader sr;
        public  int          LineNumber { get; private set; }

        public AIReader(Stream stream)
        {
            sr = new StreamReader(stream);
        }

        public void Dispose()
        {
            sr.Dispose();
        }

        public void Reset()
        {
            sr.DiscardBufferedData();
            sr.BaseStream.Seek(0, SeekOrigin.Begin);
            LineNumber = 0;
        }

        public string ReadLine(bool skipEmpty = true)
        {
            string line;
            // read until EOS
            while ((line = sr.ReadLine()) != null)
            {
                LineNumber++;

                CheckComments(ref line);

                line = line.Trim(); // get rid of white spaces

                // empty line? continue reading if skipEmpty = true
                if (line.Length > 0 || !skipEmpty)
                {
                    break;
                }
            }
            return line;
        }

        private void CheckComments(ref string line)
        {
            // remove line comments
            int index = line.IndexOf("//");
            if (index >= 0)
            {
                line = line.Remove(index);
            }

            // remove comment sections
            index = line.IndexOf("/*");
            if (index >= 0)
            {
                int read;
                while ((read = sr.Read()) >= 0)
                {
                    if (read == '*' && sr.Peek() == '/')
                    {
                        sr.Read();
                        break;
                    }

                    if (read == '\n')
                    {
                        LineNumber++;
                    }
                }
            }
        }

        public T Read<T>() where T : new()
        {
            string line;
            Type type = typeof(T);
            do
            {
                line = ReadLine();
                if (line == null)
                {
                    return default(T);
                }
            } while (line != type.Name);
            
            T result = new T();
            Read(result);
            return result;
        }

        private const           BindingFlags BFlags         = BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public;
        private static readonly Type[]       emptyTypeArg   = new Type[0];
        private static readonly object[]     emptyObjectArg = Array.Empty<object>();
        private static readonly char[]       whiteSpaces    = { ' ', '\t' };

        private void Read(object o)
        {
            string line = ReadLine();
            if (line == null)
            {
                throw new FormatException("EOS");
            }

            if (line != "{")
            {
                throw new FormatException("{");
            }

            Dictionary<string, FieldInfo> fieldNames = GetFieldNames(o.GetType());
            while ((line = ReadLine()) != null)
            {
                if (line == "}")
                {
                    break;
                }

                // get name of field
                string fieldName;
                int endIndex = line.IndexOfAny(whiteSpaces);
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
                {
                    throw new FormatException("Unknown field name '" + fieldName + "' in line " + LineNumber);
                }

                object value;
                Type fieldType = fi.FieldType;
                if (readFuncs.TryGetValue(fieldType, out ReadFunc func)
                   || readFuncs.TryGetValue(fieldType.BaseType, out func))
                {
                    value = func.Invoke(this, line, fieldType);
                }
                else
                {
                    ConstructorInfo ci = fieldType.GetConstructor(emptyTypeArg);
                    value = ci.Invoke(emptyObjectArg);
                    Read(value);
                }
                fi.SetValue(o, value);
            }
        }

        private Dictionary<string, FieldInfo> GetFieldNames(Type type)
        {
            // get all fields from this type
            FieldInfo[] fields = type.GetFields(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);

            // create dictionary
            Dictionary<string, FieldInfo> result = new Dictionary<string, FieldInfo>(fields.Length, StringComparer.OrdinalIgnoreCase);

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
                            {
                                throw new Exception(altName + " duplicate FieldName");
                            }
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

        private delegate object ReadFunc(AIReader r, string line, Type valueType);

        private static readonly Dictionary<Type, ReadFunc> readFuncs = new Dictionary<Type, ReadFunc>
                                                                       {
                                                                           { typeof(bool), ReadBool },
                                                                           { typeof(int), ReadInt },
                                                                           { typeof(string), ReadString },
                                                                           { typeof(Enum), ReadEnum },
                                                                       };

        private static object ReadBool(AIReader r, string line, Type valueType)
        {
            int index = line.IndexOf('=');
            if (index < 0)
            {
                throw new FormatException("Missing '=' in line " + r.LineNumber);
            }

            line = line.Substring(index + 1).Trim();
            if (int.TryParse(line, out int value))
            {
                return value != 0;
            }

            return bool.Parse(line);
        }

        private static object ReadInt(AIReader r, string line, Type valueType)
        {
            int index = line.IndexOf('=');
            if (index < 0)
            {
                throw new FormatException("Missing '=' in line " + r.LineNumber);
            }

            line = line.Substring(index + 1).Trim();
            if (bool.TryParse(line, out bool value)) // in case the type gets changed between versions
            {
                return value ? 1 : 0;
            }

            return int.Parse(line);
        }

        private static object ReadEnum(AIReader r, string line, Type valueType)
        {
            int index = line.IndexOf('=');
            if (index < 0)
            {
                throw new FormatException("Missing '=' in line " + r.LineNumber);
            }

            line = line.Substring(index + 1).Trim();
            return Enum.Parse(valueType, line, true);
        }

        private static object ReadString(AIReader r, string line, Type valueType)
        {
            int index = line.IndexOf('=');
            if (index >= 0)
            {
                return line.Substring(index + 1).Trim();
            }

            line = r.ReadLine();
            if (line == null)
            {
                throw new FormatException("EOS");
            }

            if (line != "{")
            {
                throw new FormatException("{");
            }

            StringBuilder sb = new StringBuilder(20);
            while ((line = r.ReadLine()) != null)
            {
                if (line == "}")
                {
                    break;
                }

                sb.AppendLine(line);
            }

            return sb.ToString();
        }
    }
}
