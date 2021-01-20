using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;

namespace UCP.CodeBlocks
{
    public class CodeBlockRetriever
    {

        public static Dictionary<string, string> CodeBlocks { get; set; }
        static CodeBlockRetriever() {
            Assembly asm = Assembly.GetExecutingAssembly();
            string file = string.Format("UCP.CodeBlocks.CodeBlocks.json");
            using (Stream stream = asm.GetManifestResourceStream(file))
                CodeBlocks = new JavaScriptSerializer().Deserialize<Dictionary<string, string>>(new StreamReader(stream).ReadToEnd());
        }
    }
}
