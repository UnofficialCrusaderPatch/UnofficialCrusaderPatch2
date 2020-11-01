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
    class StartResourceEnumerator
    {
        const int normalOffset = 0;
        const int crusaderOffset = 0x64;
        const int deathmatchOffset = 0xc8;

        const int goldArrayLength = 0x28;
        const int aiGoldOffset = 0x10;
        const int fairnessLevels = 5;

        public static Change activeChange = null;

        public static List<Change> _changes = new List<Change>();
        static Dictionary<string, StartResourceConfiguration> startResourceConfig;


        private static object resourceBlockFile = "s_resource";
        private static object goldBlockFile = "s_gold";
        private static byte[] resourceBlock;
        private static byte[] goldBlock;

        internal static Dictionary<string, StartResourceConfiguration> GetStartResourceConfigurations()
        {
            return startResourceConfig;
        }

        internal static void SetStartResourceConfiguration(string configName)
        {
            activeChange = _changes.Where(x => x.Identifier.Equals(configName)).ToList().First();
        }

        static StartResourceEnumerator()
        {
            startResourceConfig = new Dictionary<string, StartResourceConfiguration>();
            Assembly asm = Assembly.GetExecutingAssembly();

            // check if code block file is there
            string file = string.Format("UCP.Resources.CodeBlocks.{0}.block", resourceBlockFile);
            if (!asm.GetManifestResourceNames().Contains(file))
                throw new Exception("MISSING BLOCK FILE " + file);

            // read code block file
            using (Stream stream = asm.GetManifestResourceStream(file))
                resourceBlock = new CodeBlox.CodeBlock(stream).Elements.ToArray().Select(x => x.Value).ToArray();

            // check if code block file is there
            file = string.Format("UCP.Resources.CodeBlocks.{0}.block", goldBlockFile);
            if (!asm.GetManifestResourceNames().Contains(file))
                throw new Exception("MISSING BLOCK FILE " + file);

            // read code block file
            using (Stream stream = asm.GetManifestResourceStream(file))
                goldBlock = new CodeBlox.CodeBlock(stream).Elements.ToArray().Select(x => x.Value).ToArray();

            try
            {
                Load();
            }
            catch (ArgumentException e)
            {
                startResourceConfig = null;
            }
        }


        /// <summary>
        /// Load built-in vanilla starting resource file and user-provided JSON starting gooods files located in resources\goods subfolder
        /// </summary>
        public static void Load()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            StreamReader vanilla = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.Resources.StartResource.vanilla.json"), Encoding.UTF8);
            string vanillaText = vanilla.ReadToEnd();
            vanilla.Close();
            Dictionary<String, Dictionary<String, Object>> vanillaConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(vanillaText);
            if (vanillaConfig != null)
            {
                StartResourceChange change = new StartResourceChange("vanilla")
                        {
                            CreateResourceHeader("vanilla", vanillaConfig),
                        };
                Dictionary<string, string> languageConfig = new Dictionary<string, string>();
                foreach(KeyValuePair<string, object> description in vanillaConfig["description"])
                {
                    languageConfig.Add(description.Key, description.Value.ToString());
                }
                startResourceConfig.Add(change.Identifier, new StartResourceConfiguration().withDescription(languageConfig));
                _changes.Add(change);
            }


            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "resources", "goods")))
            {
                return;
            }

            foreach (string file in Directory.EnumerateFiles(Path.Combine(Environment.CurrentDirectory, "resources", "goods"), "*.json", SearchOption.TopDirectoryOnly))
            {
                StreamReader reader = new StreamReader(new FileStream(file, FileMode.Open), Encoding.UTF8);
                string resourceText = reader.ReadToEnd();
                reader.Close();

                Dictionary<String, Dictionary<String, Object>> resourceConfig;
                try
                {
                    resourceConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(resourceText);
                }
                catch (Exception e)
                {
                    File.AppendAllText("StartResourceParsing.log", "\n" + Path.GetFileNameWithoutExtension(file) + ": " + e.Message + "\n");
                    continue;
                }
                StartResourceChange change = new StartResourceChange(Path.GetFileNameWithoutExtension(file))
                        {
                            CreateResourceHeader("res_" + Path.GetFileNameWithoutExtension(file), resourceConfig),
                        };
                Dictionary<string, string> languageConfig = new Dictionary<string, string>();
                if (vanillaConfig.ContainsKey("description"))
                {
                    foreach (KeyValuePair<string, object> description in vanillaConfig["description"])
                    {
                        languageConfig.Add(description.Key, description.Value.ToString());
                    }
                }
                startResourceConfig.Add(change.Identifier, new StartResourceConfiguration().withDescription(languageConfig));
                _changes.Add(change);

            }
        }

        static byte[] ParseResources(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            Type StartingResourceType = typeof(StartingResource);
            byte[] config = new byte[0x124];

            List<String> exceptions = new List<String>();
            foreach (int field in Enum.GetValues(StartingResourceType))
            {
                try
                {
                    AssignResourceBytes(normalOffset, field, resourceConfig["normal"], config);
                }
                catch (KeyNotFoundException)
                {
                    for (int i = 0; i < crusaderOffset; i++)
                    {
                        config[i] = resourceBlock[i];
                    }
                }
                catch (Exception)
                {
                    exceptions.Add("Invalid normal value for " + Enum.GetName(StartingResourceType, field));
                }

                try
                {
                    AssignResourceBytes(crusaderOffset, field, resourceConfig["crusader"], config);
                }
                catch (KeyNotFoundException)
                {
                    for (int i = crusaderOffset; i < deathmatchOffset; i++)
                    {
                        config[i] = resourceBlock[i];
                    }
                }
                catch (Exception)
                {
                    exceptions.Add("Invalid crusader value for " + Enum.GetName(StartingResourceType, field));
                }

                try
                {
                    AssignResourceBytes(deathmatchOffset, field, resourceConfig["deathmatch"], config);
                }
                catch (KeyNotFoundException)
                {
                    for (int i = deathmatchOffset; i < resourceBlock.Length; i++)
                    {
                        config[i] = resourceBlock[i];
                    }
                }
                catch (Exception)
                {
                    exceptions.Add("Invalid deathmatch value for " + Enum.GetName(StartingResourceType, field));
                }
            }

            if (exceptions.Count > 0)
            {
                throw new Exception(String.Join("\n", exceptions));
            }

            return config;
        }

        static byte[] ParseGold(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            byte[] config = new byte[0x78];

            List<String> exceptions = new List<String>();
            try
            {
                dynamic normalGold = resourceConfig["normal"]["gold"];
                AssignGoldBytes("normal", normalGold, 0, config);
            }
            catch (KeyNotFoundException)
            {
                for (int lvl = 0; lvl < fairnessLevels; lvl++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4] = goldBlock[i + (lvl * 2) * 4];
                        config[i + (lvl * 2 + 1)] = goldBlock[i + (lvl * 2 + 1) * 4];
                    }
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e.ToString());
            }


            try
            {
                dynamic crusaderGold = resourceConfig["crusader"]["gold"];
                AssignGoldBytes("crusader", crusaderGold, goldArrayLength, config);
            }
            catch (KeyNotFoundException)
            {
                for (int lvl = 0; lvl < fairnessLevels; lvl++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4 + goldArrayLength] = goldBlock[i + (lvl * 2) * 4 + goldArrayLength];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength] = goldBlock[i + (lvl * 2 + 1) * 4 + goldArrayLength];
                    }
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e.ToString());
            }


            try
            {
                dynamic deathGold = resourceConfig["deathmatch"]["gold"];
                AssignGoldBytes("deathmatch", deathGold, goldArrayLength * 2, config);
            }
            catch (KeyNotFoundException)
            {
                for (int lvl = 0; lvl < fairnessLevels; lvl++)
                {
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4 + goldArrayLength * 2] = goldBlock[i + (lvl * 2) * 4 + goldArrayLength * 2];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength * 2] = goldBlock[i + (lvl * 2 + 1) * 4 + goldArrayLength * 2];
                    }
                }
            }
            catch (Exception e)
            {
                exceptions.Add(e.ToString());
            }

            if (exceptions.Count > 0)
            {
                throw new Exception(String.Join("\n", exceptions));
            }
            return config;
        }

        static void AssignGoldBytes(String mode, dynamic goldDictionary, Int32 offset, byte[] config)
        {
            bool humanGoldException = false;
            bool aiGoldException = false;

            for (int lvl = 0; lvl < fairnessLevels; lvl++)
            {
                try
                {
                    byte[] humanGold = BitConverter.GetBytes(goldDictionary["human"][lvl]);
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4 + offset] = humanGold[i];
                    }
                }
                catch (Exception)
                {
                    humanGoldException = true;
                }

                try
                {
                    byte[] aiGold = BitConverter.GetBytes(goldDictionary["ai"][lvl]);
                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2 + 1) * 4 + offset] = aiGold[i];
                    }
                }
                catch (Exception)
                {
                    aiGoldException = true;
                }
            }

            List<String> exceptions = new List<string>();
            if (humanGoldException == true)
            {
                exceptions.Add(mode + " mode human gold values incorrect");
            }
            if (aiGoldException == true)
            {
                exceptions.Add(mode + " mode AI gold values incorrect");
            }
            if (exceptions.Count > 0)
            {
                throw new Exception(String.Join("\n", exceptions));
            }
        }

        static void AssignResourceBytes(int offset, int field, Dictionary<String, Object> resourceConfig, byte[] config)
        {
            Type StartingResourceType = typeof(StartingResource);
            if (!resourceConfig.ContainsKey(Enum.GetName(StartingResourceType, field)))
            {
                return;
            }

            byte[] fieldValue = BitConverter.GetBytes(Convert.ToInt32(resourceConfig[Enum.GetName(StartingResourceType, field)]));
            for (var i = 0; i < 4; i++)
            {
                config[field * 4 + offset + i] = fieldValue[i];
            }
        }

        internal static void DoChange(ChangeArgs args)
        {
            if (activeChange != null)
            {
                foreach (var header in activeChange)
                {
                    header.Activate(args);
                }
                return;
            }
        }

        static DefaultSubChange CreateResourceHeader(String file, Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            try
            {
                byte[] resources = ParseResources(resourceConfig);
                byte[] gold = ParseGold(resourceConfig);

                return new DefaultSubChange(file)
                {
                    BinBytes.CreateEdit("s_resource", resources),
                    BinBytes.CreateEdit("s_gold", gold)
                };
            }
            catch (Exception e)
            {
                throw new Exception("Errors found in " + file + ":\n" + e.Message);
            }
        }
    }
}
