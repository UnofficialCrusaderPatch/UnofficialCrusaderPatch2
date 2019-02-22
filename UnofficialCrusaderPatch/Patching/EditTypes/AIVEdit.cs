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
        bool done = false;
        string resourceFolder;
        public string ResourceFolder => resourceFolder;

        public AIVEdit(string resourceFolder)
        {
            this.resourceFolder = "UnofficialCrusaderPatch." + resourceFolder;
        }

        public override bool Initialize(ChangeArgs args) => true;
        public override void Activate(ChangeArgs args)
        {
            if (done) return;

            var aivDir = args.AIVDir;

            // create backup
            string backupDir = Path.Combine(args.AIVDir.FullName, Patcher.BackupIdent);
            Directory.CreateDirectory(backupDir);
            foreach (FileInfo fi in aivDir.EnumerateFiles("*.aiv"))
                fi.CopyTo(Path.Combine(backupDir, fi.Name));



            // copy aiv castles
            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {
                if (res.StartsWith(resourceFolder, StringComparison.OrdinalIgnoreCase))
                {
                    string path = Path.Combine(aivDir.FullName, res.Substring(resourceFolder.Length + 1));
                    using (Stream stream = asm.GetManifestResourceStream(res))
                    using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        stream.CopyTo(fs);
                    }
                }
            }

            done = true;
        }

        public static DefaultHeader Header(string resourceFolder, bool isEnabled)
        {
            string ident = "aiv_" + resourceFolder;
            return new DefaultHeader(ident, isEnabled)
            {
                new AIVEdit(resourceFolder),
            };
        }
    }
}
