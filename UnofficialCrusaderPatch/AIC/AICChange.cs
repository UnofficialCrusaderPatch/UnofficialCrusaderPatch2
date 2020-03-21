using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UCP.Patching;
using UCPAIConversion;

namespace UCP.AIC
{
    class AICChange : Change
    {
        private AICollection collection;
        private List<AICharacterName> characters;

        static Dictionary<String, String> errorMessages;
        static Dictionary<String, String> errorHints;

        static Dictionary<String, List<AICharacterName>> availableSelection;
        static Dictionary<AICharacterName, String> currentSelection;

        static List<AICharacterName> emptySelection = new List<AICharacterName>();

        static List<string> internalAIC = new List<string>()
        {
            "vanilla.aic.json", "UCP-Bugfix.aic.json", "Kimberly-Balance-v1.0.aic.json",
            "Krarilotus-aggressive-AI-v1.0.aic.json", "Tatha 0.5.1.aic.json",
            "Xander10Alpha-v1.0.aic.json"
        };

        static List<AICChange> _changes = new List<AICChange>();

        public static List<AICChange> changes { get { return _changes; } }

        static AICChange () {
            currentSelection = new Dictionary<AICharacterName, string>();
            availableSelection = new Dictionary<string, List<AICharacterName>>();

            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.AIC.Resources.errors.json"), Encoding.UTF8);
            string errorText = reader.ReadToEnd();
            reader.Close();

            reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.AIC.Resources.descriptions.json"), Encoding.UTF8);
            string errorHintText = reader.ReadToEnd();
            reader.Close();

            
            JavaScriptSerializer errorSerializer = new JavaScriptSerializer();
            errorMessages = errorSerializer.Deserialize<Dictionary<String, String>>(errorText);
            errorHints = errorSerializer.Deserialize<Dictionary<String, String>>(errorHintText);
        }

        public AICChange(string titleIdent, bool enabledDefault = false)
            : base(titleIdent, ChangeType.AIC, enabledDefault, true)
        {
            this.NoLocalization = true;
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
                IsChecked = currentSelection.ContainsValue(this.TitleIdent),
                IsThreeState = true,
                IsEnabled = !this.GetTitle().EndsWith(".aic"),
                Background = internalAIC.Contains(this.TitleIdent) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque)
            };

            TreeViewItem tvi = new TreeViewItem()
            {
                IsExpanded = false,
                Focusable = false,
                Header = titleBox,
                MinHeight = 22,
            };

            titleBox.Checked += TitleBox_Checked;
            titleBox.Indeterminate += TitleBox_Indeterminate;
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
                    ToolTip = Localization.Get("ui_aicoldversion"),
                    Width = 17,
                    Height = 17,
                    Content = "\uFF1F",
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 45, 0),
                    Background = new SolidColorBrush(Color.FromRgb(246, 53, 53)),
                };
                infoButton.Click += (s, e) =>
                {

                    MessageBoxResult result = MessageBox.Show(Localization.Get("ui_aicofferconvert"), Localization.Get("ui_aicconvertprompt"), MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    switch (result)
                    {
                        case MessageBoxResult.Yes:
                            ConvertAIC();
                            break;
                        default:
                            break;
                    }
                };
                panel.Children.Add(infoButton);
            } 
            else
            {
                Button selectButton = new Button()
                {
                    ToolTip = Localization.Get("ui_aicselecttooltip"),
                    Width = 17,
                    Height = 17,
                    Content = "\u2713",
                    FontSize = 10,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 45, 0),
                    Background = new SolidColorBrush(Color.FromRgb(0, 255, 0)),

                };
                selectButton.Click += (s, e) =>
                {
                    Window aicWindow = new Window()
                    {
                        Width = 200,
                        Height = 400,
                        Title = "AIs",
                        Background = new SolidColorBrush(Color.FromRgb(220, 220, 220)),
                    };
                    Grid grid = new Grid();

                    int heightOffset = 0;
                    foreach (AICharacterName characterName in this.characters)
                    {
                        CheckBox checkBox = new CheckBox()
                        {
                            FontSize = 15,
                            Content = new TextBlock() {
                                Text = Enum.GetName(typeof(AICharacterName), characterName),
                                TextDecorations = !(currentSelection.ContainsKey(characterName) 
                                && !currentSelection[characterName].Equals(this.TitleIdent)) ? null : TextDecorations.Strikethrough,
                            },
                            Margin = new Thickness(0, 20 * (heightOffset++), 0, 0),
                            IsChecked = currentSelection.ContainsKey(characterName) && currentSelection[characterName] == this.TitleIdent,
                        };
                        if (currentSelection.ContainsKey(characterName)){
                            checkBox.ToolTip = Localization.Get("ui_aicalreadyselectedtooltip");
                        }
                        checkBox.Click += (s1, e1) =>
                        {
                            ((TextBlock)((CheckBox)s1).Content).TextDecorations = null;
                            if ((bool)((CheckBox)s1).IsChecked){
                                currentSelection[characterName] = this.TitleIdent;
                                return;
                            }
                            else if (currentSelection.ContainsKey(characterName))
                            {
                                currentSelection.Remove(characterName);
                            }
                        };
                        grid.Children.Add(checkBox);
                    }

                    Button acceptSelection = new Button()
                    {
                        Content = Localization.Get("ui_aicconfirm"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        Margin = new Thickness(0, 0, 0, 10),
                    };
                    acceptSelection.Click += (s2, e2) => aicWindow.Close();
                    grid.Children.Add(acceptSelection);
                    aicWindow.Content = grid;
                    bool? selection = aicWindow.ShowDialog();

                    int count = 0;
                    foreach (string entry in currentSelection.Values)
                    {
                        if (entry == this.TitleIdent)
                        {
                            count++;
                        }
                    }
                    if (count < 16)
                    {
                        this.titleBox.Indeterminate -= TitleBox_Indeterminate;
                        this.titleBox.IsChecked = null;
                        this.titleBox.Indeterminate += TitleBox_Indeterminate;
                    }
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
            };
            exportButton.Click += (s, e) => this.ExportFile();

            this.grid.Children.Add(exportButton);
            this.uiElement = panel;
        }

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!(System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control))
            {
                foreach (var change in changes)
                {
                    if (change != this)
                    {
                        List<AICharacterName> namesToRemove = new List<AICharacterName>();
                        foreach (KeyValuePair<AICharacterName, string> sel in currentSelection)
                        {
                            if (sel.Value == change.TitleIdent)
                            {
                                namesToRemove.Add(sel.Key);
                            }
                        }
                        foreach (AICharacterName name in namesToRemove)
                        {
                            currentSelection.Remove(name);
                        }
                        change.titleBox.IsChecked = false;
                    }
                }
            }
            foreach (AICharacterName character in this.characters)
            {
                currentSelection.Add(character, this.TitleIdent);
            }
            base.TitleBox_Checked(sender, e);
        }

        protected void TitleBox_Indeterminate(object sender, RoutedEventArgs e)
        {
            ((CheckBox)sender).IsChecked = false;
            TitleBox_Unchecked(sender, e);
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            List<AICharacterName> namesToRemove = new List<AICharacterName>();
            foreach (KeyValuePair<AICharacterName, string> sel in currentSelection)
            {
                if (sel.Value == this.TitleIdent)
                {
                    namesToRemove.Add(sel.Key);
                }
            }
            foreach (AICharacterName name in namesToRemove)
            {
                currentSelection.Remove(name);
            }
            base.TitleBox_Unchecked(sender, e);
        }

        private void ConvertAIC()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "aic", this.TitleIdent);
            string newFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(this.TitleIdent) + ".aic.json");

            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            try
            {
                bool result = AICHelper.Convert(fileName, newFileName);
                if (result)
                {
                    File.Move(fileName, backupFileName);
                    Debug.Show("AIC file successfully converted. Please click refresh to see updated AIC list");
                }
            }
            catch (Exception e)
            {
                File.WriteAllText("AICLoading.log", e.ToString());
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
            if (File.Exists(fileName))
            {
                File.Move(fileName, backupFileName);
            }
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "aic"));
            File.WriteAllText(fileName, Format(serializer.Serialize(collection)));

            Debug.Show(Localization.Get("ui_aicexport_success"), this.TitleIdent);
        }

        private String Format(String aicJson)
        {
            return aicJson.Replace(",\"", ",\n\t\"").Replace("{", "{\n\t").Replace("}", "\n}");
        }

        public static void LoadConfiguration(List<string> configuration = null)
        {
            Load();
            if (configuration == null)
            {
                return;
            }

            foreach (string change in configuration)
            {
                string[] changeLine = change.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries).Select(str => str.Trim()).ToArray();
                if (changeLine.Length < 2)
                {
                    continue;
                }

                string changeKey = changeLine[0];
                string changeSetting = changeLine[1];

                bool bundledAIC = changeSetting.Contains("aicFalse");
                bool selected = Regex.Replace(@"\s+", "", changeSetting).Contains("True");

                if (!changeKey.EndsWith(".aic"))
                {
                    foreach (AICChange aicChange in changes)
                    {
                        if (aicChange.TitleIdent == changeKey && selected == true)
                        {
                            //aicChange.titleBox.IsChecked = true;
                            foreach (AICharacter character in aicChange.collection.AICharacters)
                            {
                                currentSelection[character._Name] = aicChange.TitleIdent;
                            }
                        }
                    }
                }
            }
        }

        public static List<string> GetConfiguration()
        {
            List<string> configuration = new List<string>();
            foreach (AICChange aicChange in changes)
            {
                configuration.Add(aicChange.TitleIdent + "= { " + aicChange.TitleIdent + IsInternal(aicChange.TitleIdent).ToString() + "={" + (currentSelection.ContainsValue(aicChange.TitleIdent)).ToString() + "}");
            }
            return configuration;
        }

        public static void Refresh(object sender, RoutedEventArgs args)
        {
            for (int i = 0; i < changes.Count; i++)
            {
                ((TreeView)((Grid)((Button)sender).Parent).Children[0]).Items.Remove(changes.ElementAt(i).UIElement);
                availableSelection.Clear();
                currentSelection.Clear();
            }
            changes.Clear();
            Load();
        }

        public static void DoChange(ChangeArgs args)
        {
            CreateEdit().Activate(args);
        }

        private static bool IsInternal(string titleIdent)
        {
            return internalAIC.Contains(titleIdent);
        }

        private static void Load()
        {
            LoadAIC("UCP.AIC.Resources.UCP-Bugfix.aic.json");
            LoadAIC("UCP.AIC.Resources.vanilla.aic.json");
            LoadAIC("UCP.AIC.Resources.Kimberly-Balance-v1.0.aic.json");
            LoadAIC("UCP.AIC.Resources.Krarilotus-aggressiveAI-v1.0.aic.json");
            LoadAIC("UCP.AIC.Resources.Tatha 0.5.1.aic.json");
            LoadAIC("UCP.AIC.Resources.Xander10alpha-v1.0.aic.json");

            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "aic")))
            {
                foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "aic"), "*.aic", SearchOption.TopDirectoryOnly))
                {
                    LoadAIC(file);
                }

                foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "aic"), "*.aic.json", SearchOption.TopDirectoryOnly))
                {
                    LoadAIC(file);
                }
            }
        }

        private static void LoadAIC(string fileName)
        {
            if (fileName.EndsWith(".aic"))
            {
                AICChange change = new AICChange(Path.GetFileName(fileName), true)
                {
                    new DefaultHeader(Path.GetFileName(fileName), true, true)
                    {
                    }
                };
                changes.Add(change);
                return;
            }
            
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new ReadOnlyCollection<JavaScriptConverter>(new List<JavaScriptConverter>() { new AISerializer(errorMessages, errorHints) }));
            StreamReader reader;
            
            if (fileName.StartsWith("UCP.AIC.Resources"))
            {
                reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName), Encoding.UTF8);
            } else
            {
                reader = new StreamReader(new FileStream(fileName, FileMode.Open), Encoding.UTF8);
            }
            string text = reader.ReadToEnd();
            reader.Close();

            string aicName = Path.GetFileName(fileName).Replace("UCP.AIC.Resources.", "");
            try
            {
                AICollection ch = serializer.Deserialize<AICollection>(text);;

                AICChange change = new AICChange(aicName, true)
                {
                    new DefaultHeader(aicName, true, true)
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
                File.AppendAllText("Conversion.log", e.ToErrorString(fileName));
            }
        }

        private static ChangeHeader CreateEdit()
        {
            List<AICharacter> characterChanges = new List<AICharacter>();

            foreach (AICharacterName name in Enum.GetValues(typeof(AICharacterName)))
            {
                if (!currentSelection.ContainsKey(name))
                {
                    continue;
                }
                string changeLocation = currentSelection[name];
                AICChange changeSource = null;

                foreach (AICChange change in changes)
                {
                    if (change.TitleIdent == changeLocation)
                    {
                        changeSource = change;
                        break;
                    }
                }

                foreach (AICharacter character in changeSource.collection.AICharacters)
                {
                    if (character._Name == name)
                    {
                        characterChanges.Add(character);
                        break;
                    }
                }
            }

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (AICharacter aic in characterChanges)
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
                    for (int i = 0; i < Enum.GetNames(typeof(AIPersonalityFieldsEnum)).Length; i++)
                    {
                        string propertyName = Enum.GetName(typeof(AIPersonalityFieldsEnum), i);
                        PropertyInfo property = typeof(AIPersonality).GetProperty("_" + propertyName);
                        if (property == null)
                        {
                            property = typeof(AIPersonality).GetProperty(propertyName);
                        }
                        if (property == null) throw new Exception(propertyName);
                        object objValue = property.GetValue(aic.Personality, null);
                        int value = Convert.ToInt32(objValue);

                        // mov [eax + prop], value
                        bw.Write((byte)0xC7);
                        bw.Write((byte)0x80);
                        bw.Write((int)(i * 4));
                        bw.Write(value);
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
    }
}
