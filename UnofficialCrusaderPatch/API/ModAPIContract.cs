using System.Collections.Generic;
using System.Linq;
using UCP.AIC;
using UCP.AIV;
using UCP.Model;
using UCP.Model.Patching;
using UCP.Patching;
using UCP.Startup;

namespace UCP.API
{
    public static class ModAPIContract
    {
        #region AIC

        public static List<AICConfiguration> ListAICConfigurations()
        {
            return AICEnumerator.GetAICConfiguration();
        }

        public static void SetAICConfiguration(object aicConfiguration)
        {
            if (aicConfiguration == null)
            {
                return;
            }

            if (aicConfiguration is string)
            {
                AICEnumerator.SetAICConfiguration(aicConfiguration.ToString());
            } else if (aicConfiguration is object[])
            {
                AICEnumerator.SetAICConfiguration((object[])aicConfiguration);
            } else if (aicConfiguration is List<AICConfiguration>)
            {
                AICEnumerator.SetAICConfiguration((List<AICConfiguration>)aicConfiguration);
            }
        }


        #endregion

        #region AIV

        public static Dictionary<string, AIVConfiguration> ListAIVConfigurations()
        {
            return AIVEnumerator.GetAIVConfiguration();
        }

        public static void SetAIVConfiguration(string aivName)
        {
            if (aivName == null)
            {
                return;
            }
            AIVEnumerator.SetAIVConfiguration(aivName);
        }

        #endregion

        #region StartResource

        public static Dictionary<string, StartResourceConfiguration> ListStartResourceConfigurations()
        {
            return StartResourceEnumerator.GetStartResourceConfigurations();
        }

        public static void SetStartResourceConfiguration(string configName)
        {
            if (configName == null)
            {
                return;
            }
            StartResourceEnumerator.SetStartResourceConfiguration(configName);
        }

        #endregion

        #region StartTroop

        public static Dictionary<string, StartTroopConfiguration> ListStartTroopConfigurations()
        {
            return StartTroopEnumerator.GetStartTroopConfigurations();
        }

        public static void SetStartTroopConfiguration(string configName)
        {
            if (configName == null)
            {
                return;
            }
            StartTroopEnumerator.SetStartTroopConfiguration(configName);
        }

        #endregion

        #region GenericMod

        public static Dictionary<string, IEnumerable<string>> ListChanges()
        {
            return Mod.ModList;
        }

        public static void SetModValues(IEnumerable<ChangeConfiguration> modValueMap)
        {
            if (modValueMap == null)
            {
                return;
            }
            foreach (ChangeConfiguration changeConfig in modValueMap)
            {
                Mod currentMod = Mod.Items.ToList().SingleOrDefault(x => x.Identifier.Equals(changeConfig.Identifier));
                if (currentMod == null)
                {
                    continue;
                }
                currentMod.ApplyChanges(changeConfig.SubChanges);
            }

            List<string> changeIdentifiers = modValueMap.Select(config => config.Identifier).ToList();
            foreach (Mod currentMod in Mod.Items.Where(mod => !changeIdentifiers.Contains(mod.Identifier)))
            {
                currentMod.Disable();
            }
        }

        public static void SetModExtremeValues(IEnumerable<ChangeConfiguration> modValueMap)
        {
            if (modValueMap == null)
            {
                return;
            }
            foreach (ChangeConfiguration changeConfig in modValueMap)
            {
                Mod currentMod = Mod.Items.ToList().SingleOrDefault(x => x.Identifier.Equals(changeConfig.Identifier));
                if (currentMod == null)
                {
                    continue;
                }
                currentMod.ApplyExtremeChanges(changeConfig.SubChanges);
            }

            List<string> changeIdentifiers = modValueMap.Select(config => config.Identifier).ToList();
            foreach (Mod currentMod in Mod.Items.Where(mod => !changeIdentifiers.Contains(mod.Identifier)))
            {
                currentMod.Disable();
            }
        }

        #endregion

        #region Installation
        public static bool Install(UCPConfig config, bool overwrite=false, bool graphical=false)
        {
            ModAPIContract.SetAICConfiguration(config.AIC);
            ModAPIContract.SetAIVConfiguration(config.AIV);
            ModAPIContract.SetStartResourceConfiguration(config.StartResource);
            ModAPIContract.SetStartTroopConfiguration(config.StartTroop);
            ModAPIContract.SetModValues(config.GenericMods);
            ModAPIContract.SetModExtremeValues(config.GenericMods);


            AIVEnumerator.Install(config.Path, overwrite, graphical);
            Installer.Initialize(config.Path);

            if (Installer.crusaderArgs.HasValue)
            {
                SubChange header = AICEnumerator.CreateEdit();
                header?.Activate(Installer.crusaderArgs.Value);
                StartResourceEnumerator.DoChange(Installer.crusaderArgs.Value);
                StartTroopEnumerator.DoChange(Installer.crusaderArgs.Value);
                foreach (Mod mod in Mod.Items)
                {
                    mod.Install(Installer.crusaderArgs.Value);
                }
            }
            string errorMsg = Installer.WriteFinalize();

            Installer.InitializeExtreme(config.Path);
            if (Installer.extremeArgs.HasValue)
            {
                SubChange header = AICEnumerator.CreateEdit();
                header?.Activate(Installer.extremeArgs.Value);
                StartResourceEnumerator.DoChange(Installer.extremeArgs.Value);
                StartTroopEnumerator.DoChange(Installer.extremeArgs.Value);
                foreach (Mod mod in Mod.Items)
                {
                    mod.InitExtremeChange();
                    mod.InstallExtreme(Installer.extremeArgs.Value);
                }
            }
            errorMsg += Installer.WriteFinalizeExtreme();
            return true;
        }

        public static bool Uninstall(UCPConfig config, bool overwrite = false, bool graphical = false)
        {
            //AIVEnumerator.Restore(SHCPATH);
            Installer.RestoreOriginals(config.Path);
            return true;
        }

        #endregion
    }
}
