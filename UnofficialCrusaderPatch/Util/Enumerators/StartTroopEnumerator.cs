using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using UCP.Model;
using UCP.Patching;

namespace UCP.Startup
{
    class StartTroopEnumerator
    {
        enum LordType
        {
            Europ,
            Arab
        }

        const int lordStrengthBase = 0x64;

        // Game offsets for codeblocks
        const int normalOffset = 0;
        const int crusaderOffset = 0x64;
        const int deathmatchOffset = 0xc8;
        const string troopBlockFile = "s_troops";
        const string lordStrengthBlockFile = "s_lordstrength";
        const string lordTypeBlockFile = "s_lordtype";

        static string selectedChange = String.Empty;
        public static Change activeChange = null;

        static Dictionary<String, int[]> lordDots => new Dictionary<string, int[]>()
        {
            {"None", new int[]{ 0, 0, 0, 0, 0 } },
            {"Blue", new int[]{ 0, 1, 2, 3, 4, 5 } },
            {"Yellow", new int[]{ 0, 6, 7, 8, 9, 10 } }
        };

        public static List<Change> _changes = new List<Change>();
        static Dictionary<string, StartTroopConfiguration> startTroopConfig;

        static StartTroopEnumerator() {
            startTroopConfig = new Dictionary<string, StartTroopConfiguration>();
            try
            {
                Load();
            }
            catch (ArgumentException)
            {
                startTroopConfig = null;
            }
            
        }

        internal static Dictionary<string, StartTroopConfiguration> GetStartTroopConfigurations()
        {
            _changes.Clear();
            startTroopConfig.Clear();
            Load();
            ResetStartTroopConfiguration();
            return startTroopConfig;
        }

        internal static void SetStartTroopConfiguration(string configName)
        {
            activeChange = _changes.Where(x => x.Identifier.Equals(configName)).ToList().SingleOrDefault();
        }

        internal static void ResetStartTroopConfiguration()
        {
            activeChange = null;
        }

        internal static void DoChange(ChangeArgs args)
        {
            Change change = activeChange;
            if (!selectedChange.Equals(String.Empty))
            {
                if (activeChange == null)
                {
                    change = _changes.Where(x => x.Identifier.Equals(selectedChange)).First();
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

        /// <summary>
        /// Load built-in starting troops files and user-provided JSON starting troops files located in resources\troops subfolder
        /// </summary>
        public static void Load()
        {
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
            StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open), Encoding.UTF8);

            string starttroopsText = reader.ReadToEnd();
            reader.Close();

            string troopConfigName = Path.GetFileNameWithoutExtension(fileName);
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            Dictionary<String, Dictionary<String, Object>> troopConfig;
            try
            {
                troopConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(starttroopsText);
            }
            catch (Exception e)
            {
                File.AppendAllText("StartTroopsParsing.log", "\n" + troopConfigName + ": " + e.Message + "\n");
                return;
            }

            StartTroopChange change = new StartTroopChange(troopConfigName)
            {
                CreateSubChange(troopConfigName, troopConfig),
            };
            Dictionary<string, string> languageConfig = new Dictionary<string, string>();
            if (troopConfig.ContainsKey("description"))
            {
                foreach (KeyValuePair<string, object> description in troopConfig["description"])
                {
                    languageConfig.Add(description.Key, description.Value.ToString());
                }
            }
            startTroopConfig.Add(change.Identifier, new StartTroopConfiguration().withDescription(languageConfig));
            _changes.Add(change);
        }

        static void AssignBytes(int type, int count, byte[] config)
        {
            byte[] countBits = BitConverter.GetBytes(count);
            for (var i = 0; i < 4; i++)
            {
                config[type * 4 + i] = countBits[i];
            }
        }

        /// <summary>
        /// Parse the starting troop definition of the configuration
        /// </summary>
        /// <param name="gamemodeConfig"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Parse the player starting troop configurations
        /// </summary>
        /// <param name="gamemodeConfig"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Parse the player lord strength and dot configurations
        /// </summary>
        /// <param name="gamemodeConfig"></param>
        /// <returns></returns>
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
                    resultConfig["Type"] = new BinBytes(new byte[] { (byte)Enum.Parse(typeof(LordType), lordConfig["Type"]) });
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


        static DefaultSubChange CreateSubChange(String file, Dictionary<String, Dictionary<String, Object>> starttroopConfig)
        {
            List<BinElement> startTroopChanges = new List<BinElement>();
            List<BinElement> lordStrengthChanges = new List<BinElement>();
            List<BinElement> lordTypeChanges = new List<BinElement>();

            //look for each individual AI from Rat to Abbot
            //AI's with Index from 1 to 16 , 17 = europ lord , 18 = arab lord
            for (int playerIndex = 1; playerIndex <= 18; playerIndex++)
            {
                string indexKey = playerIndex.ToString();
                Dictionary<String, Object> Player;
                if (starttroopConfig.TryGetValue(indexKey, out Player))
                {
                    startTroopChanges.AddRange(ParsePlayer(Player));

                    // Only attempt to parse lord strength and dot configurations for AI characters
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


            DefaultSubChange header = new DefaultSubChange(file)
            {
                troopEdit,
                lordStrengthEdit,
                lordTypeEdit
            };
            return header;
        }
    }
}
