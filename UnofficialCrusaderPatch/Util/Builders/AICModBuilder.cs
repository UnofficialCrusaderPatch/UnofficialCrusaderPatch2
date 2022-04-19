using System;
using System.Collections.Generic;
using System.Linq;
using UCP.AIC;
using UCP.Model;
using UCP.Patching;

namespace UCP.Util.Builders
{
    public class AICModBuilder
    {
        internal static List<ModUIConfig> ConstructMods(string language)
        {
            List<ModUIConfig> mods = new List<ModUIConfig>();
            List<AICConfiguration> aicConfiguration = AICEnumerator.GetAICConfiguration();
            foreach (AICConfiguration aic in aicConfiguration)
            {
                ModUIConfig modConfig = new ModUIConfig();
                modConfig.modIdentifier = aic.Identifier;
                modConfig.modType = Enum.GetName(typeof(ChangeType), ChangeType.AIC);
                modConfig.modDescription = aic.Description[language];
                modConfig.changes = aic.CustomCharacterList.Select(aicName =>
                {
                    ChangeUIConfig changeConfig = new ChangeUIConfig();
                    changeConfig.identifier = aicName;
                    return changeConfig;
                });
                modConfig.modSelectionRule = "*";
                mods.Add(modConfig);
            }
            return mods;
        }
    }
}
