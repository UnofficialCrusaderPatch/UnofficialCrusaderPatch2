using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows;

namespace UnofficialCrusaderPatch
{
    class AIVChange : Change
    {
        static AIVChange activeChange = null;
        public static AIVChange ActiveChange { get { return activeChange; } }
        
        string resFolder;

        public AIVChange(string titleIdent, bool enabledDefault = false)
            : base("aiv_" + titleIdent, ChangeType.AIV, enabledDefault, true)
        {
            this.resFolder = "UnofficialCrusaderPatch.AIVs." + titleIdent;
        }

        public static AIVChange CreateDefault(string titleIdent, bool enabledDefault = false)
        {
            return new AIVChange(titleIdent, enabledDefault)
            {
                new DefaultHeader("aiv_" + titleIdent)
                {
                }
            };
        }

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Checked(sender, e);

            if (activeChange != null)
                activeChange.IsChecked = false;

            activeChange = this;
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Unchecked(sender, e);

            if (activeChange == this)
                activeChange = null;
        }

        public void CopyAIVs(DirectoryInfo aivDir)
        {
            int len = resFolder.Length + 1;

            Assembly asm = Assembly.GetExecutingAssembly();
            foreach (string res in asm.GetManifestResourceNames())
            {
                if (!res.StartsWith(resFolder, StringComparison.OrdinalIgnoreCase))
                    continue;

                string path = Path.Combine(aivDir.FullName, res.Substring(len));
                using (Stream stream = asm.GetManifestResourceStream(res))
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    stream.CopyTo(fs);
                }
            }
        }
    }
}
