using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AIConversion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AIConversion.errors.json"), Encoding.UTF8);
            string errorText = reader.ReadToEnd();

            reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AIConversion.descriptions.json"), Encoding.UTF8);
            string errorHintText = reader.ReadToEnd();

            reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AIConversion.vanilla.json"), Encoding.UTF8);
            string text = reader.ReadToEnd();
            JavaScriptSerializer errorSerializer = new JavaScriptSerializer();
            Dictionary<String, String> errorMessages = errorSerializer.Deserialize<Dictionary<String, String>>(errorText);
            Dictionary<String, String> errorHints = errorSerializer.Deserialize<Dictionary<String, String>>(errorHintText);

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new ReadOnlyCollection<JavaScriptConverter>(new List<JavaScriptConverter>() { new AISerializer(errorMessages, errorHints) }));

            try
            {
                AICollection ch = serializer.Deserialize<AICollection>(text);
                Console.WriteLine(ch.AIDescription.DescrEng);
                Console.WriteLine(ch.AIDescription.DescrRus);
            } catch (AICSerializationException e)
            {
                Console.WriteLine(e.ToErrorString("AIConversion.vanilla.json"));
            }
        }
    }
}
