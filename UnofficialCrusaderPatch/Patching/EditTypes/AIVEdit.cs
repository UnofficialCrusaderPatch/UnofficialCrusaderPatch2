using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace UnofficialCrusaderPatch
{
    public class AIVEdit : ChangeEdit
    {
        string resourceFolder;
        public string ResourceFolder => resourceFolder;

        public AIVEdit(string resourceFolder)
        {
            this.resourceFolder = "UnofficialCrusaderPatch." + resourceFolder;
        }

        public override EditResult Activate(ChangeArgs args)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            foreach (string res in asm.GetManifestResourceNames())
            {
                if (res.StartsWith(resourceFolder, StringComparison.OrdinalIgnoreCase))
                {
                    string path = Path.Combine(args.AIVDir.FullName, res.Substring(resourceFolder.Length + 1));
                    using (Stream stream = asm.GetManifestResourceStream(res))
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        stream.CopyTo(fs);
                    }
                }
            }

            return EditResult.NoErrors;
        }
        
        public static Change Change(string resourceFolder, ChangeType type, bool checkedDefault = true)
        {
            return new Change("aiv_" + resourceFolder, type, checkedDefault)
            {
                new AIVEdit(resourceFolder)
            };
        }
    }
}
