using System;
using System.Collections.Generic;
using UCP.AIV;
using UCP.Model;
using UCP.Patching;

namespace UCP.Util.Builders
{
    public class AIVModBuilder
    {
        internal static List<ModUIConfig> ConstructMods(string language)
        {
            List<ModUIConfig> mods = new List<ModUIConfig>();
            Dictionary<string, AIVConfiguration> aivConfiguration = AIVEnumerator.GetAIVConfiguration();
            foreach (KeyValuePair<string, AIVConfiguration> aivPair in aivConfiguration)
            {
                ModUIConfig modConfig = new ModUIConfig();
                /*Dictionary<string, dynamic> modConfig = new Dictionary<string, dynamic>();*/
                modConfig.modIdentifier = aivPair.Key;
                modConfig.modType = Enum.GetName(typeof(ChangeType), ChangeType.AIV);
                modConfig.modDescription = aivPair.Value.Description[language];
                modConfig.modSelectionRule = "*";
                //IEnumerable<object> castles = keyValuePair.Value.Description.Values.AsEnumerable();
                //keyValuePair.Value.Castles: { filename/id, description, image }
                // modConfig["modChanges"] = keyValuePair.Value.Castles
                mods.Add(modConfig);
            }
            return mods;
        }
    }
}
