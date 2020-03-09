using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows;
using UCP.Patching;
using System.Windows.Controls;
using System.Web.Script.Serialization;
using System.Collections.ObjectModel;
using AIConversion;
using System.Windows.Media;

namespace UCP.AIC
{
    class AICChange : Change
    {
        private AICollection collection;
        private List<AICharacterName> characters;
        private List<AICharacterName> selectedCharacters;

        static AICChange activeChange = null;
        static Dictionary<String, String> errorMessages;
        static Dictionary<String, String> errorHints;

        static Dictionary<String, List<AICharacterName>> availableSelection;
        static Dictionary<AICharacterName, String> currentSelection;
        //static List<AICharacterName> currentSelection;
        static List<AICharacterName> emptySelection = new List<AICharacterName>();

        static List<AICChange> _changes = new List<AICChange>()
        {
            AICChange.CreateDefault("vanilla.aic"),
            AICChange.CreateDefault("special.aic"),
        };
        

        public static List<AICChange> changes { get { return _changes; } }

        static AICChange () {
            currentSelection = new Dictionary<AICharacterName, string>();
            availableSelection = new Dictionary<string, List<AICharacterName>>();

            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.AIC.Resources.errors.json"), Encoding.UTF8);
            string errorText = reader.ReadToEnd();

            reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.AIC.Resources.descriptions.json"), Encoding.UTF8);
            string errorHintText = reader.ReadToEnd();

            
            JavaScriptSerializer errorSerializer = new JavaScriptSerializer();
            errorMessages = errorSerializer.Deserialize<Dictionary<String, String>>(errorText);
            errorHints = errorSerializer.Deserialize<Dictionary<String, String>>(errorHintText);
            Load();
        }

        public AICChange(string titleIdent, bool enabledDefault = false)
            : base(titleIdent, ChangeType.AIC, enabledDefault, true)
        {
            this.NoLocalization = true;
        }

        public static AICChange CreateDefault(string titleIdent, bool enabledDefault = false)
        {
            return new AICChange(titleIdent, enabledDefault)
            {
                new DefaultHeader(titleIdent, true, true)
                {
                }
            };
        }


        public override void InitUI()
        {
            this.titleBox = new CheckBox()
            {
                Content = new TextBlock()
                {
                    Text = this.GetTitle(),
                    TextDecorations = this.GetTitle().EndsWith(".aic") ? TextDecorations.Strikethrough : null,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, -1, 0, 0),
                    FontSize = 14,
                    Width = 400,
                },
                IsChecked = false,//headerList.Exists(h => h.IsEnabled),
                IsThreeState = true,
            };

            TreeViewItem tvi = new TreeViewItem()
            {
                IsExpanded = false,
                Focusable = false,
                Header = titleBox,
                MinHeight = 22,
            };

            titleBox.Checked += TitleBox_Checked;
            titleBox.Unchecked += TitleBox_Unchecked;

            grid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200)),
                Width = 420,
                Margin = new Thickness(-18, 5, 0, 0),
                Focusable = false,
            };

            FillGrid(grid);

            tvi.Items.Add(grid);
            tvi.Items.Add(null); // spacing
            Grid panel = new Grid();
            panel.Children.Add(tvi);

            if (this.GetTitle().EndsWith(".aic"))
            {
                Button infoButton = new Button()
                {
                    ToolTip = "Old version detected. Click to convert",
                    Width = 17,
                    Height = 17,
                    Content = "\uFF1F",
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 30, 0),
                    Background = new SolidColorBrush(Color.FromRgb(246, 53, 53)),
                };
                infoButton.Click += (s, e) =>
                {

                    MessageBoxResult result = MessageBox.Show("Convert aic file to newer version?", "AIC Conversion", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            Convert();
                            break;
                        default:
                            break;
                    }
                };
                panel.Children.Add(infoButton);
            } else
            {
                Button selectButton = new Button()
                {
                    ToolTip = "Select individual AIs",
                    Width = 17,
                    Height = 17,
                    Content = "\u2713",
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 30, 0),
                    Background = new SolidColorBrush(Color.FromRgb(0, 255, 0)),

                };
                panel.Children.Add(selectButton);
            }

            Button exportButton = new Button()
            {
                //Width = 40,
                Height = 20,
                Content = Localization.Get("ui_aicexport"),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 5, 5),
                ToolTip = Localization.Get("ui_aichint"),
                //Background = ButtonBrush,
            };
            exportButton.Click += (s, e) => this.ExportFile();

            this.grid.Children.Add(exportButton);
            this.uiElement = panel;
        }

        public static void Load()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new ReadOnlyCollection<JavaScriptConverter>(new List<JavaScriptConverter>() { new AISerializer(errorMessages, errorHints) }));
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.AIC.Resources.vanilla.aic.json"), Encoding.UTF8);
            string text = reader.ReadToEnd();
            try
            {
                AICollection ch = serializer.Deserialize<AICollection>(text);
                Console.WriteLine(ch.AIDescription.DescrEng);
                Console.WriteLine(ch.AIDescription.DescrRus);

                AICChange change = new AICChange("vanilla.aic.json", true)
                {
                    new DefaultHeader("vanilla.aic.json", true, true)
                    {
                    }
                };
                change.collection = ch;
                change.characters = ch.GetCharacters();
                availableSelection[change.TitleIdent] = ch.GetCharacters();
                changes.Add(change);
            }
            catch (AICSerializationException e)
            {
                Console.WriteLine(e.ToErrorString("AIConversion.vanilla.json"));
            }
        }

        public static void Refresh(object sender, RoutedEventArgs args)
        {
            // remove olds
            for (int i = Version.Changes.Count - 1; i >= 0; i--)
            {
                if (Version.Changes[i] is AICChange change)
                {
                    Version.Changes.RemoveAt(i);
                    ((TreeView)((Grid)((Button)sender).Parent).Children[0]).Items.Remove(change.UIElement);
                    availableSelection.Clear();
                    currentSelection.Clear();
                }
            }
            changes.Clear();
            Load();
        }

        public static ChangeHeader CreateEdit(AICollection coll)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (AICharacter aic in coll.AICharacters)
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
                    //for (int i = 0; i < AIPersonality.TotalFields; i++)
                    //{
                    //    // mov [eax + prop], value
                    //    bw.Write((byte)0xC7);
                    //    bw.Write((byte)0x80);
                    //    bw.Write((int)(i * 4));
                    //    bw.Write((int)aic.Personality.GetByIndex(i));
                    //}
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

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Checked(sender, e);
            if (!(System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control))
            {
                foreach (var c in Version.Changes)
                {
                    if (c != this && c is AICChange)
                    {
                        c.IsChecked = false;
                        foreach (KeyValuePair<AICharacterName, string> sel in currentSelection)
                        {
                            if (sel.Value == c.TitleIdent)
                            {
                                currentSelection.Remove(sel.Key);
                            }
                        }
                    }
                }
            }
            foreach (AICharacterName character in this.characters)
            {
                currentSelection.Add(character, this.TitleIdent);
            }
            Console.WriteLine(currentSelection);
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Unchecked(sender, e);

            if (activeChange == this)
                activeChange = null;

            foreach (KeyValuePair<AICharacterName, string> sel in currentSelection)
            {
                if (sel.Value == this.TitleIdent)
                {
                    currentSelection.Remove(sel.Key);
                }
            }
        }

        private void Convert()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "aic", this.TitleIdent);
            string newFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(this.TitleIdent) + ".aic.json");

            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            //File.Move(fileName, backupFileName);
            try
            {
                bool result = AICHelper.Convert(fileName, newFileName);
                if (result)
                {
                    Debug.Show("AIC file successfully converted. Please click refresh to see updated AIC list");
                }
            } catch (Exception e)
            {
                Debug.Show("Errors found in conversion. Please see convert manually and confirm valid JSON before reloading");
            }
        }

        private void ExportFile()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string fileName = Path.Combine(Environment.CurrentDirectory, "aic", this.TitleIdent);
            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            File.Move(fileName, backupFileName);
            File.WriteAllText(fileName, Format(serializer.Serialize(collection)));

            Debug.Show(Localization.Get("ui_aicexport_success"), this.TitleIdent);
        }

        private String Format(String aicJson)
        {
            return aicJson.Replace(",\"", ",\n\t\"").Replace("{", "{\n\t").Replace("}", "\n}");
        }
    }
}
