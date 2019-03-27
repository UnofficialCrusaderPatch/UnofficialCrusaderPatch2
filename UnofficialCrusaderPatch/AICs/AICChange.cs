using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UCP.AICharacters;
using System.Reflection;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace UCP.Patching
{
    public class AICChange : Change
    {
        bool intern = false;

        AICCollection collection;
        public AICCollection Collection => collection;

        public AICChange(string title, AICCollection coll, bool enabledDefault = false, bool isIntern = false)
            : base(title, ChangeType.AIC, enabledDefault, false)
        {
            this.NoLocalization = true;
            this.collection = coll;
            this.intern = isIntern;

            string descr = collection.Header.DescrByIndex(Localization.LanguageIndex);
            this.Add(new DefaultHeader(descr, true, true));
        }

        #region UI

        static Brush ColorBrush = new SolidColorBrush(Colors.Bisque);
        static Brush ButtonBrush = new SolidColorBrush(Color.FromArgb(10, 0, 0, 0));
        public override void InitUI()
        {
            base.InitUI();

            if (!this.intern)
                this.titleBox.Background = ColorBrush;
            else
            {
                Button button = new Button()
                {
                    //Width = 40,
                    Height = 20,
                    Content = Localization.Get("ui_aicexport"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 5, 5),
                    ToolTip = Localization.Get("ui_aichint"),
                    Background = ButtonBrush,
                };
                button.Click += (s, e) => this.ExportFile();

                this.grid.Children.Add(button);
            }
        }

        static string aicFolder;
        void ExportFile()
        {
            Directory.CreateDirectory(aicFolder);

            string fileName = Path.Combine(aicFolder, this.TitleIdent);
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                this.collection.Write(fs);
            }

            Debug.Show(Localization.Get("ui_aicexport_success"), this.TitleIdent);
        }

        #endregion

        #region Loading Files

        static bool TryLoadCollection(string path, bool resource, out AICCollection result)
        {
            try
            {
                Stream stream;
                if (resource)
                {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    stream = asm.GetManifestResourceStream(path);
                }
                else
                {
                    stream = new FileStream(path, FileMode.Open);
                }

                result = new AICCollection(stream);
                return true;
            }
            catch (FormatException e)
            {
                Debug.Show(path + "\n" + e.Message);
            }
            result = null;
            return false;
        }

        /// <summary> Loads .aic files from the given path and setup directory. </summary>
        public static void LoadFiles(string cpath)
        {
            const string searchString = "UCP.AICs.";
            const string ucpFile = "UCP-Bugfix.aic";

            aicFolder = Path.Combine(cpath, "aic");

            // load resource files
            int beforeCount = Version.Changes.Count;
            Assembly asm = Assembly.GetExecutingAssembly();

            foreach (string str in asm.GetManifestResourceNames())
            {
                if (!str.StartsWith(searchString))
                    continue;

                if (!TryLoadCollection(str, true, out AICCollection collection))
                    continue;

                string name = str.Substring(searchString.Length);
                bool isUCPFile = name == ucpFile;

                AICChange change = new AICChange(name, collection, isUCPFile, true);
                if (isUCPFile)
                {
                    Version.Changes.Insert(beforeCount, change);
                }
                else
                {
                    Version.Changes.Add(change);
                }

            }


            // load files
            if (Directory.Exists(aicFolder))
                foreach (string filePath in Directory.EnumerateFiles(aicFolder, "*.aic"))
                {
                    if (!TryLoadCollection(filePath, false, out AICCollection collection))
                        continue;

                    AICChange change = new AICChange(Path.GetFileName(filePath), collection);
                    Version.Changes.Add(change);
                }
        }

        #endregion

        #region Binary Edit

        public static void DoEdit(ChangeArgs args)
        {
            // collect characters
            AICCollection result = new AICCollection();
            foreach (Change c in Version.Changes)
            {
                if (!c.IsChecked || !(c is AICChange))
                    continue;

                AICCollection collection = ((AICChange)c).Collection;
                foreach (AICharacter aic in collection.Values)
                {
                    // topmost aics have priority
                    if (!result.ContainsKey(aic.Index))
                        result.Add(aic.Index, aic);
                }
            }

            CreateEdit(result).Activate(args);
        }

        static ChangeHeader CreateEdit(AICCollection coll)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (AICharacter aic in coll.Values)
                {
                    // mov eax, index
                    bw.Write((byte)0xB8);
                    bw.Write((int)aic.Index * 0x2A4);

                    // imul eax, 2A4
                    /*bw.Write((byte)0x69);
                    bw.Write((byte)0xC0);
                    bw.Write(0x2A4);*/

                    // add eax, esi
                    bw.Write((byte)0x01);
                    bw.Write((byte)0xF0);

                    // edit AI's properties
                    for (int i = 0; i < AIPersonality.TotalFields; i++)
                    {
                        // mov [eax + prop], value
                        bw.Write((byte)0xC7);
                        bw.Write((byte)0x80);
                        bw.Write((int)(i * 4));
                        bw.Write((int)aic.Personality.GetByIndex(i));
                    }
                }
                data = ms.ToArray();
            }

            // 004D1928
            BinaryEdit be = new BinaryEdit("ai_prop")
            {
                new BinAddress("call", 0x1B+1, true),

                new BinSkip(0x1B),
                new BinHook(5)
                {
                    // ori code
                    0xE8, new BinRefTo("call"),

                    // edit ais
                    new BinBytes(data),
                }
            };

            return new DefaultHeader("ai_prop") { be };
        }

        #endregion

    }
}
