﻿using System;
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
using UCP.AIC.Utils;
using UCP.Patching;
using UCP.Startup;
using UCPAIConversion;

using static UCP.Patching.BinElements.Register;
using static UCP.Patching.BinElements.OpCodes;
using static UCP.Patching.BinElements.OpCodes.Condition.Values;

namespace UCP.AIC
{
    class AICChange : Change
    {
        private AICollection collection;
        private List<AICharacterName> characters;
        private List<String> customCharacterNames;
        private Button conflict;

        static Dictionary<String, String> errorMessages;
        static Dictionary<String, String> errorHints;

        static Dictionary<String, List<AICharacterName>> availableSelection;
        static Dictionary<AICharacterName, String> currentSelection;

        static List<string> internalAIC = new List<string>()
        {
            "UCP.vanilla", "UCP.UCP-AI-Patch", "UCP.Kimberly-Balance-v1.2",
            "UCP.Krarilotus-aggressiveAI-v1.2", "UCP.Tatha 0.5.1",
            "UCP.Xander10alpha-Release-v1.3"
        };

        static LinkedList<AICChange> selectedChanges = new LinkedList<AICChange>();

        static List<AICChange> _changes = new List<AICChange>();

        public static List<AICChange> changes { get { return _changes; } }


        /// <summary>
        /// Read and store UCP-defined errors messages and field description hints
        /// </summary>
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
            : base("aic_" + titleIdent, ChangeType.AIC, enabledDefault, true)
        {
            this.NoLocalization = true;
        }

        /// <summary>
        /// Initialize the UI of the Change including titleBox (checkbox), titleBox.Content (AIC title), saving localized description.
        /// The titleBox object for built-in AICs is coloured white (unlike beige for user-loaded)
        /// Built-in AICs have an export option available
        /// </summary>
        public override void InitUI()
        {
            string descr = GetLocalizedDescription(this.collection);
            descr = descr == String.Empty ? this.TitleIdent : descr;
            Localization.Add(this.TitleIdent + "_descr", descr);
            this.titleBox = new CheckBox()
            {
                Content = new TextBlock() // Define title of AIC
                {
                    Text = internalAIC.Contains(this.TitleIdent.Substring(4)) ? this.TitleIdent.Substring(4).Replace("UCP.", "") : this.TitleIdent.Substring(4),
                    TextDecorations = this.GetTitle().EndsWith(".aic") ? TextDecorations.Strikethrough : null,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, -1, 0, 0),
                    FontSize = 14,
                    Width = 400,
                },
                IsChecked = currentSelection.ContainsValue(this.TitleIdent),
                IsEnabled = !this.GetTitle().EndsWith(".aic"),
                Background = internalAIC.Contains(this.TitleIdent.Substring(4)) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque)
            };

            TreeViewItem tvi = new TreeViewItem()
            {
                IsExpanded = false,
                Focusable = false,
                Header = titleBox,
                MinHeight = 22,
            };

            // Add custom handlers for tracking selected AIC files and AI Character definitions to use
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

            // Render warning with option to convert outdated .aic files to the newer JSON format
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

                    MessageBoxResult result = MessageBox.Show(Localization.Get("ui_aicofferconvert"), Localization.Get("ui_aicconvertprompt"), MessageBoxButton.YesNo, MessageBoxImage.Question);
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
                // Create warning button for indicating AIC conflicts - only visible when conflicts are present
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



            // Add export button for built-in AICs
            if (internalAIC.Contains(this.TitleIdent.Substring(4)))
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
                grid.Height += 15;
                this.grid.Children.Add(exportButton);
            }
            this.uiElement = panel;
        }

        /// <summary>
        /// If the user presses Ctrl+Click then append to the current selection otherwise exclusively select the checked AIC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                // Update list of conflicts based on additional user selection
                List<String> conflicts = new List<String>();
                int count = -1;
                foreach (AICharacterName character in this.characters)
                {
                    count++;
                    if (currentSelection.ContainsKey(character))
                    {
                        String customName = this.customCharacterNames.ElementAt(count) ;
                        String name = Enum.GetName(typeof(AICharacterName), this.collection.GetCharacters().ElementAt(count));
                        conflicts.Add(name + ((customName.Equals(String.Empty) || customName.Equals(name)) ? String.Empty : " (" + customName + ")"));
                    }
                    else
                    {
                        currentSelection.Add(character, this.TitleIdent);
                    }
                }
                if (conflicts.Count > 0)
                {
                    this.conflict.Visibility = Visibility.Visible;
                    this.conflict.ToolTip = String.Join(",\n", conflicts) + "\n" + Localization.Get("ui_aicconflict");
                }
                selectedChanges.AddLast(this);
            }
            Configuration.Save();
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

            // Update list of conflicts on user deselection of a change
            foreach (AICChange change in selectedChanges)
            {
                change.conflict.Visibility = Visibility.Hidden;
                List<String> conflicts = new List<String>();
                int count = -1;
                foreach (AICharacterName character in change.characters)
                {
                    count++;
                    if (!currentSelection.ContainsKey(character))
                    {
                        currentSelection.Add(character, change.TitleIdent);
                    }
                    else
                    {
                        if (currentSelection[character] != change.TitleIdent && change.characters.Contains(character))
                        {
                            String customName = change.customCharacterNames.ElementAt(count);
                            String name = Enum.GetName(typeof(AICharacterName), change.collection.GetCharacters().ElementAt(count));
                            conflicts.Add(name + ((customName.Equals(String.Empty) || customName.Equals(name)) ? String.Empty : " (" + customName + ")"));
                            change.conflict.Visibility = Visibility.Visible;
                        }
                    }
                }
                change.conflict.ToolTip = String.Join(",\n", conflicts) + "\n" + Localization.Get("ui_aicconflict");
            }
            Configuration.Save();
            base.TitleBox_Unchecked(sender, e);
        }

        /// <summary>
        /// Converts the associated .aic file to the newer JSON format
        /// </summary>
        private void ConvertAIC()
        {
            string workaroundFilename =  this.TitleIdent.Substring(4);
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "aic", workaroundFilename);
            string newFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(workaroundFilename) + ".json");

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

        /// <summary>
        /// Exports the current AIC definition to the resources\aic\exports folder
        /// </summary>
        private void ExportFile()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "aic", "exports", this.TitleIdent.Substring(4).Replace("UCP.", "")) + ".json";
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
            File.WriteAllText(fileName, new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(this.TitleIdent.Substring(4).Replace("UCP.", "UCP.AIC.Resources.") + ".json"), Encoding.UTF8).ReadToEnd());

            Debug.Show(Localization.Get("ui_aicexport_success"), this.TitleIdent.Substring(4).Replace("UCP.", "") + ".json");
        }

        /// <summary>
        /// Convenience method for exporting custom AICs
        /// </summary>
        /// <param name="aicJson"></param>
        /// <returns></returns>
        private String Format(String aicJson)
        {
            return Regex.Unescape(aicJson.Replace(",\"", ",\n\t\"").Replace("{", "{\n\t").Replace("}", "\n}").Replace("\\r\\n", "\r\n"));
        }

        /// <summary>
        /// Loads configuration of saved selections of AIC changes.
        /// In the event of conflicts the first listed AIC is given priority.
        /// The first listed AIC is the AIC that was selected first during the user's last session.
        /// </summary>
        /// <param name="configuration"></param>
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
                                currentSelection[(AICharacterName)Enum.Parse(typeof(AICharacterName), character.Name.ToString())] = aicChange.TitleIdent;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns AIC selection to store in configuration file.
        /// Only AIC files with AI Characters that will be used are set to selected.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetConfiguration()
        {
            List<string> configuration = new List<string>();
            foreach (AICChange aicChange in selectedChanges.Reverse())
            {
                configuration.Add(aicChange.TitleIdent + "= { " + aicChange.TitleIdent + IsInternal(aicChange.TitleIdent).ToString() + "={" + (currentSelection.ContainsValue(aicChange.TitleIdent)).ToString() + "} }");
            }
            foreach (AICChange aicChange in changes)
            {
                if (selectedChanges.Contains(aicChange))
                {
                    continue;
                }
                configuration.Add(aicChange.TitleIdent + "= { " + aicChange.TitleIdent + IsInternal(aicChange.TitleIdent).ToString() + "={" + (currentSelection.ContainsValue(aicChange.TitleIdent)).ToString() + "} }");
            }
            return configuration;
        }

        /// <summary>
        /// Clears and reloads the set of AIC definitions.
        /// Reapplies selections if the specified AIC is still present.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void Refresh(object sender, RoutedEventArgs args)
        {
            String[] prevSelection = new String[currentSelection.Values.Count];
            currentSelection.Values.CopyTo(prevSelection, 0);
            availableSelection.Clear();
            currentSelection.Clear();
            for (int i = 0; i < changes.Count; i++)
            {
                ((TreeView)((Grid)((Button)sender).Parent).Children[0]).Items.Remove(changes.ElementAt(i).UIElement);
                Localization.Remove(changes.ElementAt(i) + "_descr");
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
                            currentSelection[(AICharacterName)Enum.Parse(typeof(AICharacterName), character.Name.ToString())] = aicChange.TitleIdent;
                        }
                    }
                }
            }
            Configuration.Save();
        }

        public static void DoChange(ChangeArgs args, bool isExtreme)
        {
            CreateEdit(isExtreme).Activate(args);
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
            /*LoadAIC("UCP.AIC.Resources.UCP-AI-Patch.json");*/
            LoadAIC("UCP.AIC.Resources.vanilla.json");
            /*LoadAIC("UCP.AIC.Resources.Kimberly-Balance-v1.2.json");
            LoadAIC("UCP.AIC.Resources.Krarilotus-aggressiveAI-v1.2.json");
            LoadAIC("UCP.AIC.Resources.Tatha 0.5.1.json");
            LoadAIC("UCP.AIC.Resources.Xander10alpha-Release-v1.3.json");*/

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

            string aicName = Path.GetFileNameWithoutExtension(fileName).Replace("UCP.AIC.Resources.", "UCP.");
            try
            {
                if (availableSelection.ContainsKey(aicName))
                {
                    throw new Exception("AIC with the same filename has already been loaded");
                }

                AICollection ch = serializer.Deserialize<AICollection>(text);;
                AICChange change = new AICChange(aicName, true)
                {
                    new DefaultHeader("aic_" + aicName, true)
                    {
                    }
                };
                change.collection = ch;
                change.characters = ch.GetCharacters();
                change.customCharacterNames = ch.GetCustomCharacterNames();
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

        /// <summary>
        /// Retrieve localized description based on selected language preference.
        /// If a description is not found then return first non-empty description.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
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

        private static ChangeHeader CreateEdit(bool isExtreme)
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
                    if ((AICharacterName)Enum.Parse(typeof(AICharacterName), character.Name.ToString()) == name)
                    {
                        characterChanges.Add(character);
                        break;
                    }
                }
            }

            BinaryEdit startResourceEdit = null;
            byte[] originalCode = null;
            if (isExtreme)
            {
                originalCode = new byte[] { 0x8B, 0x8C, 0x24, 0x24, 0x11, 0x00, 0x00 }; /* original code */
                startResourceEdit = new BinaryEdit("ai_startresource");
            }
            else
            {
                originalCode = new byte[] { 0x8B, 0x8C, 0x24, 0x8C, 0x0D, 0x00, 0x00 }; /* original code */
                startResourceEdit = new BinaryEdit("ai_startresource_shc");
            }   
            uint requiredBytes = 0;

            List<BinElement> editBytes = new List<BinElement>()
            {
                originalCode,
                PUSH(FLAGS),
                PUSH(ALL),
                XOR(new byte[] { 0x31, 0xC0 }), /* xor eax, eax */
            };

            foreach (AICharacter aic in characterChanges.Where(aic => aic.StartResources != null))
            {
                byte aicIndex = (byte)aic._Name;
                if (aic.StartResources.normal != null)
                {
                    List<BinElement> normalBytes = BuildResourceSelection(requiredBytes, aicIndex, 1, isExtreme);
                    editBytes.AddRange(normalBytes);
                    requiredBytes += 0x5C;
                }

                if (aic.StartResources.crusader != null)
                {
                    List<BinElement> crusaderBytes = BuildResourceSelection(requiredBytes, aicIndex, 2, isExtreme);
                    editBytes.AddRange(crusaderBytes);
                    requiredBytes += 0x5C;
                }

                if (aic.StartResources.deathmatch != null)
                {
                    List<BinElement> deathmatchBytes = BuildResourceSelection(requiredBytes, aicIndex, 3, isExtreme);
                    editBytes.AddRange(deathmatchBytes);
                    requiredBytes += 0x5C;
                }
            }

            editBytes.Add(POP(ALL));
            editBytes.Add(POP(FLAGS));
            
            BinHook startResourceHook = new BinHook(0x7);
            

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (AICharacter aic in characterChanges)
                {
                    // mov eax, index
                    bw.Write((byte)0xB8);
                    bw.Write((int)aic._Name * 0x2A4);

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
            BinAlloc resourceLocation = BuildResourceAllocation(characterChanges);
            startResourceEdit.Add(resourceLocation);
            foreach (BinElement elem in editBytes) startResourceHook.Add(elem);
            startResourceEdit.Add(startResourceHook);
            return new DefaultHeader("ai_prop") { startResourceEdit, be };
        }

        private static BinAlloc BuildResourceAllocation(List<AICharacter> characterChanges)
        {
            List<byte> allocatedBytes = new List<byte>();
            foreach (AICharacter aic in characterChanges)
            {
                if (aic.StartResources != null)
                {
                    AIStartResourceCollection resourceCollection = aic.StartResources;
                    if (resourceCollection.normal != null)
                    {
                        allocatedBytes.AddRange(GetDefinedResourceValues(resourceCollection.normal));
                    }
                    if (resourceCollection.crusader != null)
                    {
                        allocatedBytes.AddRange(GetDefinedResourceValues(resourceCollection.crusader));
                    }
                    if (resourceCollection.deathmatch != null)
                    {
                        allocatedBytes.AddRange(GetDefinedResourceValues(resourceCollection.deathmatch));
                    }
                }
            }
            
            return new BinAlloc("resource_location_array", allocatedBytes.ToArray());
        }

        private static IEnumerable<byte> GetDefinedResourceValues(Dictionary<StartingResource, int> resourceDictionary)
        {
            List<byte> allocatedBytes = new List<byte>();
            for (int i = 0; i < 23; i++)
            {
                if (Enum.IsDefined(typeof(StartingResource), i))
                {
                    StartingResource resourceName = (StartingResource)i;
                    if (resourceDictionary.TryGetValue(resourceName, out int resourceValue))
                    {
                        allocatedBytes.AddRange(BitConverter.GetBytes(resourceValue));
                    }
                    else
                    {
                        allocatedBytes.AddRange(BitConverter.GetBytes(0));
                    }
                }
                else
                {
                    allocatedBytes.AddRange(BitConverter.GetBytes(0));
                }
            }
            return allocatedBytes;
        }

        private static List<BinElement> BuildResourceSelection(uint arrayOffset, byte aicIndex, byte gameMode, bool isExtreme)
        {
            byte[] gameModeAddress = isExtreme ? new byte[] { 0xA4, 0x7A, 0x4B, 0x02 } : new byte[] { 0xA4, 0x45, 0xA2, 0x01 };
            byte[] playerAIIndex = isExtreme ? new byte[] { 0x2C, 0x47, 0x1F, 0x01 } : new byte[] { 0xEC, 0x1A, 0x16, 0x01 };
            byte[] playerResourceAllocation = isExtreme ? new byte[] { 0xA0, 0x28, 0x1F, 0x01 } : new byte[] { 0x60, 0xFC, 0x15, 0x01 };

            List<BinElement> editBytes = new List<BinElement>()
                    {
                        0xB8, BitConverter.GetBytes(arrayOffset),                         /* mov eax, current_offset_in_resource_array */
                        0x80, 0x3D, gameModeAddress, gameMode,                            /* cmp byte ptr [024B7AA4],01 */
                        JMP(NOTEQUALS, 0x47),                                             /* if not normal game mode skip */
                        0x31, 0xD2,                                                       /* xor edx, edx - set player check */
                        0x80, 0xBA, playerAIIndex, (byte)(aicIndex + 1),                  /* cmp byte ptr [edx+011F472C], aicIndex+1 */
                        JMP(NOTEQUALS, 0x2E),
                        XOR(new byte[] { 0x31, 0xC9 }),                                   /* xor ecx, ecx */
                        0x8B, 0x9C, 0x08, new BinRefTo("resource_location_array", false), /* mov ebx, [eax+ecx+resource_location_array] */
                        0x89, 0x9C, 0x11, playerResourceAllocation,                         /* mov [eax+edx+011F28A0],ebx */
                        ADD(ECX, 0x4),
                        CMP(ECX, 0xC),
                        JMP(EQUALS, 0x11),
                        CMP(ECX, 0x18),
                        JMP(EQUALS, 0xC),
                        CMP(ECX, 0x34),
                        JMP(EQUALS, 0x7),
                        CMP(ECX, 0x5C),
                        JMP(new byte[]{ 0x7C, 0xDB }),                                    /* jl to mov ebx, [eax+resource_location_array] */
                        JMP(GREATER, 0x5),
                        ADD(ECX, 0x4),
                        JMP(new byte[]{ 0xEB, 0xD4 }),                                    /* jmp to mov ebx, [eax+resource_location_array] */
                        ADD(new byte[]{ 0x81, 0xC2, 0xF4, 0x39, 0x00, 0x00 }),            /* add edx, 0x39f4 */
                        CMP(new byte[]{ 0x81, 0xFA, 0xA0, 0xCF, 0x01, 0x00 }),            /* cmp edx, 0x1cfa0 */
                        JMP(new byte[]{ 0x7C, 0xBB }),                                    /* jl to cmp byte ptr [edx+011F472C], aicIndex+1 */
                    };
            return editBytes;
        }
    }
}
