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
        private Button conflict;

        static Dictionary<String, String> errorMessages;
        static Dictionary<String, String> errorHints;

        static Dictionary<String, List<AICharacterName>> availableSelection;
        static Dictionary<AICharacterName, String> currentSelection;

        static List<string> internalAIC = new List<string>()
        {
            "vanilla.json", "UCP-Bugfix.json", "Kimberly-Balance-v1.0.json",
            "Krarilotus-aggressiveAI-v1.0.json", "Tatha 0.5.1.json",
            "Xander10alpha-v1.0.json"
        };

        static LinkedList<AICChange> selectedChanges = new LinkedList<AICChange>();

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
            string descr = GetLocalizedDescription(this.collection);
            descr = descr == String.Empty ? this.TitleIdent : descr;
            Localization.Add(this.TitleIdent + "_descr", descr);
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
            titleBox.Unchecked += TitleBox_Unchecked;

            grid = new Grid()
            {
                Background = new SolidColorBrush(Color.FromArgb(150, 200, 200, 200)),
                Width = 420,
                Margin = new Thickness(-18, 5, -1, -1),
                Focusable = false,
            };

            FillGrid(grid);

            tvi.Items.Add(grid);
            tvi.Items.Add(null); // spacing
            Grid panel = new Grid();
            panel.Children.Add(tvi);
            panel.Margin = new Thickness(-20, 0, 0, 0);

            if (this.GetTitle().EndsWith(".aic"))
            {
                Button infoButton = new Button()
                {
                    ToolTip = Localization.Get("ui_aicoldversion"),
                    Width = 17,
                    Height = 17,
                    Content = "!",
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
                this.conflict = new Button()
                {
                    ToolTip = Localization.Get("ui_aicconflict"),
                    Width = 17,
                    Height = 17,
                    Content = "!",
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 0, 45, 0),
                    Background = new SolidColorBrush(Color.FromRgb(255, 255, 0)),
                    Visibility = Visibility.Hidden
                };
                panel.Children.Add(conflict);
            }




            if (internalAIC.Contains(this.TitleIdent))
            {
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
            }
            this.uiElement = panel;
        }

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!(System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control))
            {
                DeselectOthers(this);
                selectedChanges.Clear();
                currentSelection.Clear();
                foreach (AICharacterName character in this.characters)
                {
                    currentSelection.Add(character, this.TitleIdent);
                }
                selectedChanges.AddFirst(this);
                //this.titleBox.IsChecked = true;
            }
            else
            {
                List<String> conflicts = new List<String>();
                foreach (AICharacterName character in this.characters)
                {
                    if (currentSelection.ContainsKey(character))
                    {
                        conflicts.Add(Enum.GetName(typeof(AICharacterName), character));
                    }
                    else
                    {
                        currentSelection.Add(character, this.TitleIdent);
                    }
                }
                if (conflicts.Count > 0)
                {
                    this.conflict.Visibility = Visibility.Visible;
                    this.conflict.ToolTip = String.Join(",\n", conflicts) + " " + Localization.Get("ui_aicconflict");
                }
                selectedChanges.AddLast(this);
            }
            base.TitleBox_Checked(sender, e);
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.conflict.Visibility = Visibility.Hidden;
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
            selectedChanges.Remove(this);

            foreach (AICChange change in selectedChanges)
            {
                change.conflict.Visibility = Visibility.Hidden;
                List<String> conflicts = new List<String>();
                foreach (AICharacterName character in change.characters)
                {
                    if (!currentSelection.ContainsKey(character))
                    {
                        currentSelection.Add(character, change.TitleIdent);
                    }
                    else
                    {
                        if (currentSelection[character] != change.TitleIdent && change.characters.Contains(character))
                        {
                            conflicts.Add(Enum.GetName(typeof(AICharacterName), character));
                            change.conflict.Visibility = Visibility.Visible;
                        }
                    }
                }
                change.conflict.ToolTip = String.Join(",\n", conflicts) + " " + Localization.Get("ui_aicconflict");
            }

            base.TitleBox_Unchecked(sender, e);
        }

        private void ConvertAIC()
        {
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "aic", this.TitleIdent);
            string newFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(this.TitleIdent) + ".json");

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
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "aic", "exports", this.TitleIdent);
            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            if (File.Exists(fileName))
            {
                File.Move(fileName, backupFileName);
            }
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "resources", "aic", "exports"));
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

                bool selected = Regex.Replace(@"\s+", "", changeSetting.Split('=')[1]).Contains("True");

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
                configuration.Add(aicChange.TitleIdent + "= { " + aicChange.TitleIdent + IsInternal(aicChange.TitleIdent).ToString() + "={" + (currentSelection.ContainsValue(aicChange.TitleIdent)).ToString() + "} }");
            }
            return configuration;
        }

        public static void Refresh(object sender, RoutedEventArgs args)
        {
            String[] prevSelection = new String[currentSelection.Values.Count];
            currentSelection.Values.CopyTo(prevSelection, 0);
            availableSelection.Clear();
            currentSelection.Clear();
            for (int i = 0; i < changes.Count; i++)
            {
                ((TreeView)((Grid)((Button)sender).Parent).Children[0]).Items.Remove(changes.ElementAt(i).UIElement);
                Localization.Remove(changes.ElementAt(i).TitleIdent + "_descr");
            }
            changes.Clear();
            LoadConfiguration();
            foreach (String selected in prevSelection)
            {
                foreach (AICChange aicChange in changes)
                {
                    if (aicChange.TitleIdent == selected)
                    {
                        foreach (AICharacter character in aicChange.collection.AICharacters)
                        {
                            currentSelection[character._Name] = aicChange.TitleIdent;
                        }
                    }
                }
            }
            Configuration.Save();
        }

        public static void DoChange(ChangeArgs args)
        {
            CreateEdit().Activate(args);
        }

        private static void DeselectOthers(AICChange selected)
        {
            foreach (var change in changes)
            {
                if (change != selected)
                {
                    change.titleBox.IsChecked = false;
                }
            }
        }

        private static bool IsInternal(string titleIdent)
        {
            return internalAIC.Contains(titleIdent);
        }

        private static void Load()
        {
            LoadAIC("UCP.AIC.Resources.UCP-Bugfix.json");
            LoadAIC("UCP.AIC.Resources.vanilla.json");
            LoadAIC("UCP.AIC.Resources.Kimberly-Balance-v1.0.json");
            LoadAIC("UCP.AIC.Resources.Krarilotus-aggressiveAI-v1.0.json");
            LoadAIC("UCP.AIC.Resources.Tatha 0.5.1.json");
            LoadAIC("UCP.AIC.Resources.Xander10alpha-v1.0.json");

            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "aic")))
            {
                foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "resources", "aic"), "*.aic", SearchOption.TopDirectoryOnly))
                {
                    LoadAIC(file);
                }

                List<string> exceptions = new List<string>();
                foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "resources", "aic"), "*.json", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        LoadAIC(file);
                    }
                    catch (Exception)
                    {
                        exceptions.Add(file);
                    }
                    
                }
                if (exceptions.Count > 0)
                {
                    Debug.Show("Error loading AIC files: " + String.Join(",", exceptions));
                }
            }
        }

        private static void LoadAIC(string fileName)
        {
            if (fileName.EndsWith(".aic"))
            {
                AICChange change = new AICChange(Path.GetFileName(fileName), true)
                {
                    new DefaultHeader(String.Empty, true, true)
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
                if (availableSelection.ContainsKey(aicName))
                {
                    throw new Exception("AIC with the same filename has already been loaded");
                }

                AICollection ch = serializer.Deserialize<AICollection>(text);;
                AICChange change = new AICChange(aicName, true)
                {
                    new DefaultHeader(aicName, true)
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
                File.AppendAllText("AICParsing.log", e.ToErrorString(fileName));
                throw e;
            }
            catch (Exception e)
            {
                File.AppendAllText("AICParsing.log", "\n" + aicName + ": " + e.Message + "\n");
                throw e;
            }
        }

        private static string GetLocalizedDescription(AICollection ch)
        {
            string currentLang = Localization.Translations.ToArray()[Localization.LanguageIndex].Ident;
            string descr = String.Empty;
            try
            {
                descr = ch.AICShortDescription[currentLang];
                if (descr == String.Empty)
                {
                    foreach (var lang in Localization.Translations)
                    {
                        try
                        {
                            descr = ch.AICShortDescription[lang.Ident];
                            if (descr != String.Empty)
                            {
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception)
            {
                foreach (var lang in Localization.Translations)
                {
                    try
                    {
                        descr = ch.AICShortDescription[lang.Ident];
                        if (descr != String.Empty)
                        {
                            break;
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            return descr;
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
