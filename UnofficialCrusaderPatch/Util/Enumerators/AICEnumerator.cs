using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Script.Serialization;
using UCP.Model;
using UCP.Patching;
using UCPAIConversion;

namespace UCP.AIC
{
    class AICEnumerator
    {
        static Dictionary<String, String> errorMessages;
        static Dictionary<String, String> errorHints;

        static List<AICConfiguration> availableSelection { get; }
        static Dictionary<AICharacterName, String> activeSelection;


        static Dictionary<string, AICChange> _changes { get; }

        #region Initialization
        static AICEnumerator()
        {
            activeSelection = new Dictionary<AICharacterName, string>();
            availableSelection = new List<AICConfiguration>();
            _changes = new Dictionary<string, AICChange>();

            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.Resources.AIC.errors.json"), Encoding.UTF8);
            string errorText = reader.ReadToEnd();
            reader.Close();

            reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.Resources.AIC.descriptions.json"), Encoding.UTF8);
            string errorHintText = reader.ReadToEnd();
            reader.Close();

            JavaScriptSerializer errorSerializer = new JavaScriptSerializer();
            errorMessages = errorSerializer.Deserialize<Dictionary<String, String>>(errorText);
            errorHints = errorSerializer.Deserialize<Dictionary<String, String>>(errorHintText);

            try
            {
                Load();
            } catch (ArgumentException)
            {
                availableSelection = null;
                activeSelection = null;
            }
            
        }

        private static void Load()
        {
            if (Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "aic")))
            {
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
                    File.AppendAllText("AICParsing.log", "\n" + "Error loading AIC files: " + String.Join(",", exceptions) + "\n");
                }
            }
        }

        #endregion

        internal static List<AICConfiguration> GetAICConfiguration()
        {
            availableSelection.Clear();
            Load();
            return availableSelection;
        }

        internal static void SetAICConfiguration(string aicConfiguration)
        {
            activeSelection.Clear();
            List<string> aiCharacterNames = new List<string>();

            if (availableSelection.Exists(x => x.Identifier.Equals(aicConfiguration)))
            {
                aiCharacterNames.AddRange(availableSelection.Single(x => x.Identifier.Equals(aicConfiguration)).CharacterList);
                foreach (string name in aiCharacterNames)
                {
                    activeSelection[(AICharacterName)Enum.Parse(typeof(AICharacterName), name)] = aicConfiguration.ToString();
                }
            }
        }

        internal static void SetAICConfiguration(List<string> aicConfigurationList)
        {
            activeSelection.Clear();
            List<string> aiCharacterNames = new List<string>();

            foreach(string aicConfiguration in aicConfigurationList.AsEnumerable().Reverse())
            {
                if (availableSelection.Exists(x => x.Identifier.Equals(aicConfiguration)))
                {
                    aiCharacterNames.AddRange(availableSelection.Single(x => x.Identifier.Equals(aicConfiguration)).CharacterList);
                    foreach (string name in aiCharacterNames)
                    {
                        activeSelection[(AICharacterName)Enum.Parse(typeof(AICharacterName), name)] = aicConfiguration.ToString();
                    }
                }
            }
        }

        internal static void SetAICConfiguration(object[] aicConfiguration)
        {
            activeSelection.Clear();
            List<string> aiCharacterNames = new List<string>();

            foreach (Dictionary<string, object> aicOption in aicConfiguration)
            {
                if (!aicOption.ContainsKey("Name") || !aicOption.ContainsKey("Characters"))
                {
                    return;
                }
                string aicName = aicOption["Name"].ToString();
                if (availableSelection.Exists(x => x.Identifier.Equals(aicName)))
                {
                    foreach (string aiCharacterName in (object[])aicOption["Characters"])
                    {
                        if (availableSelection.Single(x => x.Identifier.Equals(aicName)).CharacterList.Contains(aiCharacterName) && !aiCharacterNames.Contains(aiCharacterName))
                        {
                            aiCharacterNames.Add(aiCharacterName);
                            activeSelection[(AICharacterName)Enum.Parse(typeof(AICharacterName), aiCharacterName)] = aicName;
                        }
                    }
                }
            }
        }

        internal static void SetAICConfiguration(List<AICConfiguration> aicConfiguration)
        {
            activeSelection.Clear();
            List<string> aiCharacterNames = new List<string>();

            foreach (AICConfiguration aicOption in aicConfiguration)
            {
                string aicName = aicOption.Identifier;
                if (availableSelection.Exists(x => x.Identifier.Equals(aicName)))
                {
                    foreach (string aiCharacterName in aicOption.CharacterList)
                    {
                        if (availableSelection.Single(x => x.Identifier.Equals(aicName)).CharacterList.Contains(aiCharacterName) && !aiCharacterNames.Contains(aiCharacterName))
                        {
                            aiCharacterNames.Add(aiCharacterName);
                            activeSelection[(AICharacterName)Enum.Parse(typeof(AICharacterName), aiCharacterName)] = aicName;
                        }
                    }
                }
            }
        }

        internal static void ResetAICConfiguration()
        {
            activeSelection.Clear();
        }

        internal static SubChange CreateEdit()
        {
            if (activeSelection.Count == 0)
            {
                return null;
            }
            
            List<AICharacter> characterChanges = new List<AICharacter>();

            foreach (AICharacterName name in Enum.GetValues(typeof(AICharacterName)))
            {
                if (!activeSelection.ContainsKey(name))
                {
                    continue;
                }
                string changeLocation = activeSelection[name];
                AICChange changeSource;
                if (!_changes.TryGetValue(changeLocation, out changeSource))
                {
                    break;
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

            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (AICharacter aic in characterChanges)
                {
                    // mov eax, index
                    bw.Write((byte)0xB8);
                    bw.Write((int)aic._Name * 0x2A4);

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
            return new DefaultSubChange("ai_prop") { be };
        }

        private static void LoadAIC(string fileName)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new ReadOnlyCollection<JavaScriptConverter>(new List<JavaScriptConverter>() { new AISerializer(errorMessages, errorHints) }));
            StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open), Encoding.UTF8);

            string text = reader.ReadToEnd();
            reader.Close();

            string aicName = Path.GetFileNameWithoutExtension(fileName).Replace("UCP.Resources.AIC.", "");
            try
            {
                if (availableSelection.Exists(x => x.Identifier.Equals(aicName)))
                {
                    throw new Exception("AIC with the same filename has already been loaded");
                }

                AICollection ch = serializer.Deserialize<AICollection>(text); ;
                AICChange change = new AICChange(aicName)
                {
                    new DefaultSubChange(aicName)
                    {
                    }
                };
                change.collection = ch;
                change.characters = ch.GetCharacters();
                change.customCharacterNames = ch.GetCustomCharacterNames();
                _changes[aicName] = change;
                availableSelection.Add(new AICConfiguration().withIdentifier(aicName)
                    .withCharacterList(ch.GetCharacters().Select(x => x.ToString()).ToList())
                    .withCustomCharacterList(ch.GetCustomCharacterNames().Select(x => x.ToString()).ToList())
                    .withDescription(ch.AICShortDescription)
                    );
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
    }
}
