using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace UnofficialCrusaderPatch
{
    public class AIVChange : Change
    {
        string resourceFolder;

        public AIVChange(string ident, ChangeType type, string resourceFolder, bool checkedDefault = true)
            : base(ident, type, checkedDefault)
        {
            this.resourceFolder = "UnofficialCrusaderPatch." + resourceFolder;
        }

        public void Activate(DirectoryInfo dir)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            foreach (string res in asm.GetManifestResourceNames())
            {
                if (res.StartsWith(resourceFolder))
                {
                    string path = Path.Combine(dir.FullName, res.Substring(resourceFolder.Length + 1));
                    using (Stream stream = asm.GetManifestResourceStream(res))
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        stream.CopyTo(fs);
                    }
                }
            }
        }
    }
}
