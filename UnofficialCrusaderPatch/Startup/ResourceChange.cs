using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using UCP.Startup;

namespace UCP.Patching
{
    public class ResourceChange : Change
    {
        string headerKey;
        const int normalOffset = 0;
        const int crusaderOffset = 0x64;
        const int deathmatchOffset = 0xc8;

        const int goldArrayLength = 0x28;
        const int aiGoldOffset = 0x10; 
        const int fairnessLevels = 5;

        public static List<Change> changes = new List<Change>();

        public ResourceChange(string title, bool enabledDefault = false, bool isIntern = false)
            : base(title, ChangeType.Other, enabledDefault, false)
        {
            this.NoLocalization = true;
            //this.headerKey = descrIdent + "_descr";
            //Localization.Add(headerKey, descr);
        }

        static ResourceChange()
        {
            Load();
        }

        public static void Load()
        {
            String fileName = "resource.json";
            StreamReader reader = new StreamReader(new FileStream(fileName, FileMode.Open), Encoding.UTF8);
            string resourceText = reader.ReadToEnd();
            reader.Close();


            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Dictionary<String, Dictionary<String, Object>> resourceConfig = serializer.Deserialize<Dictionary<String, Dictionary<String, Object>>>(resourceText);

            changes.Add(
            new ResourceChange(fileName, true)
            {
                CreateResourceHeader(resourceConfig),
            });
        }

        static byte[] ParseResources(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            Type StartingResourceType = typeof(StartingResource);
            byte[] config = new byte[0x124];
            foreach (int field in Enum.GetValues(StartingResourceType))
            {
                try
                {
                    AssignBytes("normal", normalOffset, field, resourceConfig, config);
                    AssignBytes("crusader", crusaderOffset, field, resourceConfig, config);
                    AssignBytes("deathmatch", deathmatchOffset, field, resourceConfig, config);
                }
                catch (KeyNotFoundException) { }
            }
            return config;
        }

        static byte[] ParseGold(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            dynamic normalGold = resourceConfig["normal"]["gold"];
            dynamic crusaderGold = resourceConfig["crusader"]["gold"];
            dynamic deathGold = resourceConfig["deathmatch"]["gold"];

            byte[] config = new byte[0x78];
            for (int lvl = 0; lvl < fairnessLevels; lvl++)
            {
                try
                {   
                    byte[] normalHumanGold = BitConverter.GetBytes(normalGold["human"][lvl]);
                    byte[] normalAIGold = BitConverter.GetBytes(normalGold["ai"][lvl]);

                    
                    byte[] crusaderHumanGold = BitConverter.GetBytes(crusaderGold["human"][lvl]);
                    byte[] crusaderAIGold = BitConverter.GetBytes(crusaderGold["ai"][lvl]);

                    
                    byte[] deathHumanGold = BitConverter.GetBytes(deathGold["human"][lvl]);
                    byte[] deathAIGold = BitConverter.GetBytes(deathGold["ai"][lvl]);

                    for (var i = 0; i < 4; i++)
                    {
                        config[i + (lvl * 2) * 4] = normalHumanGold[i];
                        config[i + (lvl * 2 + 1) * 4] = normalAIGold[i];

                        config[i + (lvl * 2) * 4 + goldArrayLength] = crusaderHumanGold[i];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength] = crusaderAIGold[i];

                        config[i + (lvl * 2) * 4 + goldArrayLength * 2] = deathHumanGold[i];
                        config[i + (lvl * 2 + 1) * 4 + goldArrayLength * 2] = deathAIGold[i];
                    }
                }
                catch (KeyNotFoundException) { }
            }
            return config;
        }

        static void AssignBytes(String mode, int offset, int field, Dictionary<String, Dictionary<String, Object>> resourceConfig, byte[] config)
        {
            Type StartingResourceType = typeof(StartingResource);
            byte[] fieldValue = BitConverter.GetBytes(Convert.ToInt32(resourceConfig[mode][Enum.GetName(StartingResourceType, field)]));
            for (var i = 0; i < 4; i++)
            {
                config[field * 4 + offset + i] = fieldValue[i];
            }
        }


        #region Binary Edit

        static DefaultHeader CreateResourceHeader(Dictionary<String, Dictionary<String, Object>> resourceConfig)
        {
            byte[] resources = ParseResources(resourceConfig);
            byte[] gold = ParseGold(resourceConfig);
            return new DefaultHeader("s_resource", true, true)
                {
                    BinBytes.CreateEdit("s_resource", resources),
                    BinBytes.CreateEdit("s_gold", gold)
                };
        }

        #endregion

    }
}
