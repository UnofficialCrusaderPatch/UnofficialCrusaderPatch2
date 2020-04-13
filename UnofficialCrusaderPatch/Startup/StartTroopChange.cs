using System;
using System.Collections.Generic;
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

namespace UCP.Startup
{
    public class StartTroopChange : Change
    {
        enum LordType
        {
            Europ,
            Arab
        }

        string headerKey;
        const int lordStrengthBase = 0x64;
        //game offsets for codeblocks
        const int normalOffset = 0;
        const int crusaderOffset = 0x64;
        const int deathmatchOffset = 0xc8;
        const string troopBlockFile = "s_troops";
        const string lordStrengthBlockFile = "s_lordstrength";
        const string lordTypeBlockFile = "s_lordtype";

        string description;
        bool IsValid = true;
        
        static string selectedChange = String.Empty;
        public static StartTroopChange activeChange = null;

        static Dictionary<String, int[]> lordDots => new Dictionary<string, int[]>()
        {
            {"None", new int[]{ 0, 0, 0, 0, 0 } },
            {"Blue", new int[]{ 0, 1, 2, 3, 4, 5 } },
            {"Yellow", new int[]{ 0, 6, 7, 8, 9, 10 } }
        };

        public static List<Change> changes = new List<Change>();

        public StartTroopChange(string title, bool enabledDefault = false, bool isIntern = false)
            : base("s_" + title, ChangeType.StartTroops, enabledDefault, false)
        {
            this.NoLocalization = true;
        }

        public override void InitUI()
        {
            Localization.Add(this.TitleIdent + "_descr", this.description);
            base.InitUI();
            if (this.IsChecked)
            {
                activeChange = this;
            }
            ((TextBlock)this.titleBox.Content).Text = this.TitleIdent.Substring(2).Replace("UCP.", "");

            if (this.IsValid == false)
            {
                ((TextBlock)this.titleBox.Content).TextDecorations = TextDecorations.Strikethrough;
                this.titleBox.IsEnabled = false;
                this.titleBox.ToolTip = this.description;
                ((TextBlock)this.titleBox.Content).Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                this.titleBox.IsChecked = selectedChange.Equals(this.TitleIdent);
            }
            this.titleBox.Background = this.TitleIdent.Substring(2).StartsWith("UCP.") ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);

            if (this.TitleIdent.Substring(2).StartsWith("UCP."))
            {
                Button exportButton = new Button()
                {
                    //Width = 40,
                    Height = 20,
                    Content = Localization.Get("ui_aicexport"),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 5, 5),
                };
                exportButton.Click += (s, e) => this.ExportFile();
                grid.Height += 15;
                this.grid.Children.Add(exportButton);
            }
        }

        protected override void TitleBox_Checked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Checked(sender, e);

            if (activeChange != null)
            {
                activeChange.IsChecked = false;
            }
            selectedChange = this.TitleIdent;
            activeChange = this;
        }

        protected override void TitleBox_Unchecked(object sender, RoutedEventArgs e)
        {
            base.TitleBox_Unchecked(sender, e);

            if (activeChange == this)
            {
                selectedChange = String.Empty;
                activeChange = null;
            }
        }

        private void ExportFile()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string fileName = Path.Combine(Environment.CurrentDirectory, "resources", "troops", "exports", this.TitleIdent.Substring(2).Replace("UCP.", "")) + ".json";
            string backupFileName = fileName;
            while (File.Exists(backupFileName))
            {
                backupFileName = backupFileName + ".bak";
            }
            if (File.Exists(fileName))
            {
                File.Move(fileName, backupFileName);
            }
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "resources", "troops", "exports"));

            using (StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(this.TitleIdent.Substring(2).Replace("UCP.", "UCP.Startup.Resources.Troops.") + ".json"), Encoding.UTF8))
            {
                File.WriteAllText(fileName, reader.ReadToEnd(), Encoding.UTF8);
            }

            Debug.Show(Localization.Get("ui_aicexport_success"), this.TitleIdent.Substring(2).Replace("UCP.", "") + ".json");
        }

        public static void Refresh(object sender, RoutedEventArgs args)
        {
            changes.Clear();
            Load();

            Version.RemoveChanges(ChangeType.Troops);
            Version.Changes.AddRange(changes);
        }

        public static void LoadConfiguration(List<string> configuration = null)
        {
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
                if (selected == true)
                {
                    selectedChange = changeKey;
                }
            }
        }


        static void CreateNullChange(string file, string message)
        {
            StartTroopChange change = new StartTroopChange(Path.GetFileNameWithoutExtension(file).Replace("UCP.Startup.Resources.Troops.", "UCP."), false)
                        {
                            new DefaultHeader("s_" + file, false)
                            {
                                new BinaryEdit(troopBlockFile)
                                {
                                    new BinSkip(0x50),
                                },
                                new BinaryEdit(lordStrengthBlockFile)
                                {
                                    new BinSkip(0x80),
                                },
                                new BinaryEdit(lordTypeBlockFile)
                                {
                                    new BinSkip(0x16),
                                }
                            }
                        };
            change.description = message;
            change.IsValid = false;
            changes.Add(change);
        }

        internal static void DoChange(ChangeArgs args)
        {
            Change change = activeChange;
            if (!selectedChange.Equals(String.Empty))
            {
                if (activeChange == null)
                {
                    change = changes.Where(x => x.TitleIdent.Equals(selectedChange)).First();
                    foreach (var header in change)
                    {
                        header.Activate(args);
                    }
                    return;
                }
                foreach (var header in change)
                {
                    header.Activate(args);
                }
                return;
            }
        }

        public static void Load()

        {
            // load all premade Starttroop configurations that come with the UCP
            Load("UCP.Startup.Resources.Troops.vanilla.json");
            Load("UCP.Startup.Resources.Troops.UCP-StartingTroops-Patch.json");

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "troops")))
            {
                return;
            }

            foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "resources", "troops"), "*.json", SearchOption.TopDirectoryOnly))
            {
                Load(file);
            }
        }

        private static void Load(string fileName)
        {
            StreamReader reader;
            if (fileName.StartsWith("UCP.Startup"))
            {
                reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName), Encoding.UTF8);
            }
            else
            {
                reader = new StreamReader(new FileStream(fileName, FileMode.Open), Encoding.UTF8);
            }

            string starttroopsText = reader.ReadToEnd();
            reader.Close();

            string startTroopConfigName = Path.GetFileNameWithoutExtension(fileName);
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<String, Dictionary<String, Object>> startTroopConfig;
            try
            {
                startTroopConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(starttroopsText);
            }
            catch (Exception)
            {
                CreateNullChange(Path.GetFileNameWithoutExtension(startTroopConfigName), "Invalid JSON detected");
                return;
            }

            try 
            { 
                string description = GetLocalizedDescription(startTroopConfigName, startTroopConfig);
                StartTroopChange change = new StartTroopChange(startTroopConfigName.Replace("UCP.Startup.Resources.Troops.", "UCP."), false)
                {
                    CreateStartTroopHeader(startTroopConfigName.Replace("UCP.Startup.Resources.Troops.", "UCP."), startTroopConfig),
                };
                change.description = description;
                changes.Add(change);
            }
            //TODO error handling!
            catch (Exception e)
            {
                CreateNullChange(Path.GetFileNameWithoutExtension(startTroopConfigName).Replace(" ", ""), e.Message);
                File.AppendAllText("StartTroopsParsing.log", "\n" + startTroopConfigName + ": " + e.Message + "\n");
            }
        }

        static void AssignBytes(int type, int count, byte[] config)
        {
            byte[] countBits = BitConverter.GetBytes(count);
            for (var i = 0; i < 4; i++)
            {
                config[type * 4 + i] = countBits[i];
            }
        }

        //normal, crusader, deathmatch
        static byte[] ParseTroops(Dictionary<String, dynamic> gamemodeConfig)
        {
            byte[] gamemodeBytes = new byte[0x50];
            dynamic unitTypes;
            Dictionary<string, int> troopsCounter = new Dictionary<string, int>();
            if (gamemodeConfig.TryGetValue("Units", out unitTypes))
            {
                for (int typeIndex = 0; typeIndex < unitTypes.Count; ++typeIndex)
                {
                    int unitCount = 0;
                    try
                    {
                        unitCount = gamemodeConfig["Counts"][typeIndex];
                    }
                    catch { };
                    if (troopsCounter.ContainsKey(unitTypes[typeIndex])) troopsCounter[unitTypes[typeIndex]] += unitCount;
                    else troopsCounter.Add(unitTypes[typeIndex], unitCount);
                }
            }

            Type StartingTroopsType = typeof(StartingTroops);
            foreach (int troopsType in Enum.GetValues(StartingTroopsType))
            {
                try
                {
                    int unitCount = 0;
                    string enumName = Enum.GetName(StartingTroopsType, troopsType);
                    if (troopsCounter.ContainsKey(enumName))
                        unitCount += troopsCounter[enumName];
                    AssignBytes(troopsType, unitCount, gamemodeBytes);
                }
                catch (KeyNotFoundException) { }
            }

            return gamemodeBytes;

        }


        //AI's with Index from 1 to 16 , 17 = europ lord , 18 = arab lord
        static List<BinElement> ParsePlayer(Dictionary<String, Object> playerConfig)
        {
            List<BinElement> playerTroopChanges = new List<BinElement>();

            dynamic normal;
            if (playerConfig.TryGetValue("normal", out normal))
            {
                playerTroopChanges.Add(new BinBytes(ParseTroops(normal)));
            }
            else
            {
                playerTroopChanges.Add(new BinSkip(0x8C));
            }

            dynamic crusader;
            if (playerConfig.TryGetValue("crusader", out crusader))
            {
                playerTroopChanges.Add(new BinBytes(ParseTroops(crusader)));
            }
            else
            {
                playerTroopChanges.Add(new BinSkip(0x8C));
            }

            dynamic deathmatch;
            if (playerConfig.TryGetValue("deathmatch", out deathmatch))
            {
                playerTroopChanges.Add(new BinBytes(ParseTroops(deathmatch)));
            }
            else
            {
                playerTroopChanges.Add(new BinSkip(0x8C));
            }
            return playerTroopChanges;
        }

        static Dictionary<String, BinElement> ParseLordType(Dictionary<String, Object> gamemodeConfig)
        {
            Int32 lordMultiplier;
            List<String> exceptions = new List<string>();

            Dictionary<String, BinElement> resultConfig = new Dictionary<string, BinElement>()
                {
                    { "Strength", new BinSkip(0x4) },
                    { "Dots", new BinSkip(0x4) },
                    { "Type", new BinSkip(0x1) }
                };

            dynamic lordConfig;
            if (gamemodeConfig.TryGetValue("Lord", out lordConfig))
            {
                try
                {
                    int dotCount = Convert.ToInt32(lordConfig["DotCount"]);
                    String dotColour = lordConfig["DotColour"].ToString();
                    resultConfig["Dots"] = new BinInt32(lordDots[dotColour][dotCount]);
                }
                catch (KeyNotFoundException) { }
                catch (Exception)
                {
                    exceptions.Add("lord dots");
                }

                try
                {
                    lordMultiplier = Convert.ToInt32((double)lordConfig["StrengthMultiplier"] * lordStrengthBase);
                    resultConfig["Strength"] = new BinInt32(lordMultiplier);
                }
                catch (KeyNotFoundException) { }
                catch (Exception e)
                {
                    exceptions.Add("lord strength" + e.Message);
                }

                try
                {
                    resultConfig["Type"] = new BinBytes(new byte[] { (byte) Enum.Parse(typeof(LordType), lordConfig["Type"]) });
                }
                catch (KeyNotFoundException) { }
                catch (Exception)
                {
                    exceptions.Add("lord type");
                }

                if (exceptions.Count > 0)
                {
                    throw new Exception(String.Join(",", exceptions));
                }
            }
            return resultConfig;
        }

        static String GetLocalizedDescription(String file, Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            String description = file;
            string currentLang = Localization.Translations.ToArray()[Configuration.Language].Ident;
            try
            {
                description = resourceConfig["description"][currentLang].ToString();
            }
            catch (Exception)
            {
                foreach (var lang in Localization.Translations)
                {
                    try
                    {
                        description = resourceConfig["description"][lang.Ident].ToString();
                        break;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            if (!description.Equals(file))
            {
                description = description.Substring(0, Math.Min(description.Length, 1000));
            }

            return description;
        }


        static DefaultHeader CreateStartTroopHeader(String file, Dictionary<String, Dictionary<String, Object>> starttroopConfig)
        {
            List<BinElement> startTroopChanges = new List<BinElement>();
            List<BinElement> lordStrengthChanges = new List<BinElement>();
            List<BinElement> lordTypeChanges = new List<BinElement>();

            //look for each individual AI from Rat to Abbot
            for (int playerIndex = 1; playerIndex <= 18; playerIndex++)
            {
                string indexKey = playerIndex.ToString();
                Dictionary<String, Object> Player;
                if (starttroopConfig.TryGetValue(indexKey, out Player))
                {
                    startTroopChanges.AddRange(ParsePlayer(Player));

                    if (playerIndex < 17)
                    {
                        Dictionary<String, BinElement> lords;
                        try
                        {
                            lords = ParseLordType(Player);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                        lordStrengthChanges.Add(lords["Dots"]);
                        lordStrengthChanges.Add(lords["Strength"]);
                        lordTypeChanges.Add(lords["Type"]);
                    }
                    if (playerIndex == 16)
                    {
                        startTroopChanges.Add(new BinSkip(0xF0));
                    }
                }
            }

            BinaryEdit troopEdit = new BinaryEdit(troopBlockFile);
            troopEdit.AddRange(startTroopChanges);

            BinaryEdit lordStrengthEdit = new BinaryEdit(lordStrengthBlockFile);
            lordStrengthEdit.AddRange(lordStrengthChanges);

            BinaryEdit lordTypeEdit = new BinaryEdit(lordTypeBlockFile);
            lordTypeEdit.AddRange(lordTypeChanges);


            DefaultHeader header = new DefaultHeader("s_" + file, true)
            {
                troopEdit,
                lordStrengthEdit,
                lordTypeEdit
            };
            return header;
        }
    }
}
