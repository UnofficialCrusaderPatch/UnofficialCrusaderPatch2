using System;
using System.Collections.Generic;
using UCP.Model;
using UCP.Patching;
using UCP.Startup;

namespace UCP.Util.Builders
{
    public class StartTroopModBuilder
    {
        internal static List<ModUIConfig> ConstructMods(string language)
        {
            List<ModUIConfig> mods = new List<ModUIConfig>();
            Dictionary<string, StartTroopConfiguration> startTroopConfiguration = StartTroopEnumerator.GetStartTroopConfigurations();
            foreach (KeyValuePair<string, StartTroopConfiguration> troop in startTroopConfiguration)
            {
                ModUIConfig modConfig = new ModUIConfig();
                modConfig.modIdentifier = troop.Key;
                modConfig.modType = Enum.GetName(typeof(ChangeType), ChangeType.StartTroops);
                modConfig.modDescription = troop.Value.Description[language];
                modConfig.modSelectionRule = "*";
                mods.Add(modConfig);
            }
            return mods;
        }
    }
}
