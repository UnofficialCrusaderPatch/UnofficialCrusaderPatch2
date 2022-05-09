using System;
using System.Collections.Generic;
using UCP.Model;
using UCP.Patching;
using UCP.Startup;

namespace UCP.Util.Builders
{
    public class StartResourceModBuilder
    {
        internal static List<ModUIConfig> ConstructMods(string language)
        {
            List<ModUIConfig> mods = new List<ModUIConfig>();
            Dictionary<string, StartResourceConfiguration> startResourceConfiguration = StartResourceEnumerator.GetStartResourceConfigurations();
            foreach (KeyValuePair<string, StartResourceConfiguration> res in startResourceConfiguration)
            {
                ModUIConfig modConfig = new ModUIConfig();
                modConfig.modIdentifier = res.Key;
                modConfig.modType = Enum.GetName(typeof(ChangeType), ChangeType.StartResource);
                modConfig.modDescription = res.Value.Description[language];
                modConfig.modSelectionRule = "*";
                mods.Add(modConfig);
            }
            return mods;
        }
    }
}
