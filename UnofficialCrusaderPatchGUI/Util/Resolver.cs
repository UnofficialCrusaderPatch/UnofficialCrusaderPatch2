using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCP.Model;
using static UCP.Util.Constants;

namespace UCP.Util
{
    internal static class Resolver
    {
        public static readonly JsonSerializer Writer = new JsonSerializer()
        {
            Formatting = Formatting.Indented
        };
        static readonly SHA256 hashSHA256 = SHA256.Create();

        internal static bool isValidSHCPath(string path)
        {
            return Directory.Exists(path) && Directory.GetFiles(path)
                .AsEnumerable().Any(x =>
                    Path.GetFileName(x).Equals(
                        "Stronghold Crusader.exe") ||
                    Path.GetFileName(x).Equals(
                        "Stronghold_Crusader_Extreme.exe"));
        }

        internal static string GenerateHash()
        {
            return BitConverter.ToString(hashSHA256.ComputeHash(Encoding.ASCII.GetBytes(Environment.MachineName)));
        }

        internal static bool VerifyHash(string hash)
        {
            return hash.Equals(BitConverter.ToString(hashSHA256.ComputeHash(Encoding.ASCII.GetBytes(Environment.MachineName))));
        }

        internal static Dictionary<string, object> GetExistingOrWriteEmptyPreference()
        {
            Dictionary<string, object> preferences = ReadPreferences();
            if (preferences == null)
            {
                preferences = GetEmptyPreference();
                Resolver.WritePreferences(preferences);
            }
            return preferences;
        }

        internal static Dictionary<string, object> ReadPreferences()
        {
            if (File.Exists(PATH_PREFS))
            {
                using (StreamReader file = File.OpenText(PATH_PREFS))
                {
                    return (Dictionary<string, object>)Writer.Deserialize(file, typeof(Dictionary<string, object>));
                }
            }
            return null;
        }

        internal static void WritePreferences(Dictionary<string, object> config)
        {
            using(StreamWriter file = File.CreateText(PATH_PREFS))
            {
                Writer.Serialize(file, config);
            }
        }

        internal static Dictionary<string, object> GetEmptyPreference()
        {
            return new Dictionary<string, object>
                {
                    {HASH, Resolver.GenerateHash()},
                    {PATH, null},
                    {MOST_RECENT_CONFIG, null},
                    {KNOWN_PATHS, new JArray()},
                    {PREFER_PATH, false},
                    {LANGUAGE, ENGLISH},
                    {AUTOSAVE, true}
                };
        }

        internal static UCPConfig GetUCPConfigFromUncovertedCfg(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string firstLine = reader.ReadLine();
                if (firstLine == null)
                {
                    return null;
                }

                UCPConfig config = new UCPConfig();
                config.Path = firstLine.Split('=')[1];
                List<string> aicList = new List<string>();

                List<ChangeConfiguration> changeConfigurations = new List<ChangeConfiguration>();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("u_") || line.StartsWith("ai_") || line.StartsWith("o_"))
                    {
                        if (line.Contains("True"))
                        {
                            changeConfigurations.Add(ParseGenericModConfiguration(line));
                        }
                    }
                    else if (line.StartsWith("res_"))
                    {
                        if (line.Contains("{True}"))
                        {
                            config.StartResource = line.Split('=')[0].Replace("res_", "").Replace("UCP.", "");
                        }
                    }
                    else if (line.StartsWith("s_"))
                    {
                        if (line.Contains("{True}"))
                        {
                            config.StartTroop = line.Split('=')[0].Replace("s_", "").Replace("UCP.", "");
                        }
                    }
                    else if (line.StartsWith("aiv_"))
                    {
                        if (line.Contains("{True}"))
                        {
                            config.AIV = line.Split('=')[0].Replace("aiv_", "").Replace("UCP.", "");
                        }
                    }
                    else if (line.StartsWith("aic_"))
                    {
                        if (line.Contains("{True}"))
                        {
                            aicList.Add(line.Split('=')[0].Replace("aic_", "").Replace("UCP.", ""));
                        }
                    }
                }
                config.AIC = aicList;
                config.GenericMods = changeConfigurations;
                return config;
            }
        }

        private static ChangeConfiguration ParseGenericModConfiguration(string line)
        {
            ChangeConfiguration currentConfig = new ChangeConfiguration();
            int index = line.IndexOf('=');
            currentConfig.Identifier = line.Substring(0, index);

            string rawSubChangeList = line.Substring(index + 1).Trim();
            rawSubChangeList = rawSubChangeList.Replace("{", "").Replace("}", "").Trim();

            bool enabled = false;
            Dictionary<string, double?> subChange = new Dictionary<string, double?>();
            foreach (string rawSubChange in rawSubChangeList.Split(' '))
            {
                string identifier = rawSubChange.Split('=')[0];
                string[] value = rawSubChange.Split('=')[1].Split(';');

                if (bool.Parse(value[0]))
                {
                    enabled = true;
                    if (value.Length == 2)
                    {
                        subChange.Add(identifier, double.Parse(value[1]));
                    }
                    else
                    {
                        subChange.Add(identifier, null);
                    }
                }
            }
            if (enabled)
            {
                currentConfig.SubChanges = subChange;
            }
            return currentConfig;
        }
    }
}
