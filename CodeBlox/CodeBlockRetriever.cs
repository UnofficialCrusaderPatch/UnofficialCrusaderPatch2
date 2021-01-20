using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Script.Serialization;

namespace CodeBlox
{
    public class CodeBlockRetriever
    {

        public static Dictionary<string, string> CodeBlocks { get; set; }
        static CodeBlockRetriever()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string file = string.Format("UCP.CodeBlocks.CodeBlocks.json");
            using (Stream stream = asm.GetManifestResourceStream(file))
                CodeBlocks = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(new StreamReader(stream).ReadToEnd());
        }
    }
}
