using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using UCP.AIC;
using UCP.AIV;
using UCP.Model;
using UCP.Model.Patching;
using UCP.Patching;
using UCP.Startup;
using UCP.Util.Builders;

namespace UCP.API
{
    public class Startup
    {
        const string MOD_CONFIG_PATH = "UCP.Resources.modConfig.json";

        #region Configuration
        /**
         * Func<Task, Task<Object>> delegate to expose method to external callers (ie NodeJS)
         */
        public async Task<object> Invoke (object input)
        {
            return GetUCPConfiguration(input.ToString());
        }

        public static object GetUCPConfiguration(string language)
        {
            Localization.Load(language);
            List<ModBackendConfig> config = ReadStoredConfig();
            return BuildModUIConfigList(config, language);
        }

        private static List<ModBackendConfig> ReadStoredConfig()
        {
            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(MOD_CONFIG_PATH), Encoding.UTF8);
            string configText = reader.ReadToEnd();
            reader.Close();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Deserialize<List<ModBackendConfig>>(configText);
        }

        // Builds the list of mod UI configs for each mod type using the ModBuilders
        private static List<ModUIConfig> BuildModUIConfigList(List<ModBackendConfig> config, string language)
        {
            List<ModUIConfig> modules = new List<ModUIConfig>();

            List<ModUIConfig> genericMods = GenericModBuilder.ConstructMods(config, language);
            modules.AddRange(genericMods);

            List<ModUIConfig> aivMods = AIVModBuilder.ConstructMods(language);
            modules.AddRange(aivMods);

            List<ModUIConfig> aicMods = AICModBuilder.ConstructMods(language);
            modules.AddRange(aicMods);

            List<ModUIConfig> startTroopMods = StartTroopModBuilder.ConstructMods(language);
            modules.AddRange(startTroopMods);

            List<ModUIConfig> startResourceMods = StartResourceModBuilder.ConstructMods(language);
            modules.AddRange(startResourceMods);

            return modules;
        }


        static string GetTranslations(string identifier)
        {
            return Localization.Get(identifier);
        }

        #endregion


        #region AIC

        public static List<AICConfiguration> ListAICConfigurations()
        {
            return AICEnumerator.GetAICConfiguration();
        }

        public static void SetAICConfiguration(object aicConfiguration)
        {
            if (aicConfiguration == null)
            {
                AICEnumerator.ResetAICConfiguration();
            }

            if (aicConfiguration is string)
            {
                AICEnumerator.SetAICConfiguration(aicConfiguration.ToString());
            }
            else if (aicConfiguration is object[])
            {
                AICEnumerator.SetAICConfiguration((object[])aicConfiguration);
            }
            else if (aicConfiguration is List<AICConfiguration>)
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
                AIVEnumerator.ResetAIVConfiguration();
            }
            else
            {
                AIVEnumerator.SetAIVConfiguration(aivName);
            }
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
                StartResourceEnumerator.ResetStartResourceConfiguration();
            }
            else
            {
                StartResourceEnumerator.SetStartResourceConfiguration(configName);
            }
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
                StartTroopEnumerator.ResetStartTroopConfiguration();
            }
            else
            {
                StartTroopEnumerator.SetStartTroopConfiguration(configName);
            }
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
        public static bool Install(UCPConfig config, bool overwrite = false, bool graphical = false)
        {
            SetAICConfiguration(config.AIC);
            SetAIVConfiguration(config.AIV);
            SetStartResourceConfiguration(config.StartResource);
            SetStartTroopConfiguration(config.StartTroop);
            SetModValues(config.GenericMods);
            SetModExtremeValues(config.GenericMods);


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

            /* Clear configuration until install called again */
            foreach (Mod currentMod in Mod.Items)
            {
                currentMod.Disable();
            }
            AICEnumerator.ResetAICConfiguration();
            AIVEnumerator.ResetAIVConfiguration();
            StartResourceEnumerator.ResetStartResourceConfiguration();
            StartTroopEnumerator.ResetStartTroopConfiguration();

            return true;
        }

        public static bool Uninstall(UCPConfig config)
        {
            AIVEnumerator.Uninstall(config.Path);
            Installer.RestoreOriginals(config.Path);
            return true;
        }

        #endregion
    }
}
