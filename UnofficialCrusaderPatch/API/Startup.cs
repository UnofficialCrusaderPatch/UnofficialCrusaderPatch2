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

namespace UCP.API
{
    public class Startup
    {
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

            StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("UCP.Resources.modConfig.json"), Encoding.UTF8);
            string configText = reader.ReadToEnd();
            reader.Close();

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<object> config = serializer.Deserialize<List<object>>(configText);

            List<dynamic> modules = new List<dynamic>();
            foreach (var mod in config)
            {
                try
                {
                    dynamic transformedMod = ConstructMod(mod, language);
                    modules.Add(transformedMod);
                } catch (Exception e)
                {
                    throw new Exception("Mod that failed is" + ((dynamic) mod)["modIdentifier"].ToString() + e.Message);
                }
                
                
            }

            Dictionary<string, AIVConfiguration> aivConfiguration = AIVEnumerator.GetAIVConfiguration();
            foreach (KeyValuePair<string, AIVConfiguration> aivPair in aivConfiguration)
            {
                Dictionary<string, dynamic> modConfig = new Dictionary<string, dynamic>();
                modConfig["modIdentifier"] = aivPair.Key;
                modConfig["modType"] = "AIV";
                modConfig["modDescription"] = aivPair.Value.Description[language];
                modConfig["modSelectionRule"] = "*";
                //IEnumerable<object> castles = keyValuePair.Value.Description.Values.AsEnumerable();
                //keyValuePair.Value.Castles: { filename/id, description, image }
                // modConfig["modChanges"] = keyValuePair.Value.Castles
                modules.Add(modConfig);
            }

            List<AICConfiguration> aicConfiguration = AICEnumerator.GetAICConfiguration();
            foreach (AICConfiguration aic in aicConfiguration)
            {
                Dictionary<string, dynamic> modConfig = new Dictionary<string, dynamic>();
                modConfig["modIdentifier"] = aic.Identifier;
                modConfig["modType"] = "AIC";
                modConfig["modDescription"] = aic.Description[language];
                modConfig["modChanges"] = aic.CustomCharacterList;
                modConfig["modSelectionRule"] = "*";
                modules.Add(modConfig);
            }

            Dictionary<string, StartTroopConfiguration> startTroopConfiguration = StartTroopEnumerator.GetStartTroopConfigurations();
            foreach (KeyValuePair<string, StartTroopConfiguration> troop in startTroopConfiguration)
            {
                Dictionary<string, dynamic> modConfig = new Dictionary<string, dynamic>();
                modConfig["modIdentifier"] = troop.Key;
                modConfig["modType"] = "troop";
                modConfig["modDescription"] = troop.Value.Description[language];
                modConfig["modSelectionRule"] = "*";
                modules.Add(modConfig);
            }

            Dictionary<string, StartResourceConfiguration> startResourceConfiguration = StartResourceEnumerator.GetStartResourceConfigurations();
            foreach (KeyValuePair<string, StartResourceConfiguration> res in startResourceConfiguration)
            {
                Dictionary<string, dynamic> modConfig = new Dictionary<string, dynamic>();
                modConfig["modIdentifier"] = res.Key;
                modConfig["modType"] = "resource";
                modConfig["modDescription"] = res.Value.Description[language];
                modConfig["modSelectionRule"] = "*";
                modules.Add(modConfig);
            }
            return modules;
        }

        static object ConstructMod(dynamic mod, string language)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            dynamic transformedMod = serializer.Deserialize<object>(serializer.Serialize(mod));

            try
            {
                transformedMod["modDescription"] = mod["modDescription"][language];
            } catch (KeyNotFoundException) {}
            catch (Exception)
            {
                throw new Exception("Invalid modIdentifier present in mod");
            }

            try
            {
                transformedMod["detailedDescription"] = mod["detailedDescription"][language];
            }
            catch (KeyNotFoundException) { }
            catch (Exception)
            {
                transformedMod["detailedDescription"] = "";
            }

            List<dynamic> changes = new List<dynamic>();
            foreach (var change in mod["changes"])
            {
                if (!IsValidChange(change))
                {
                    throw new Exception("Invalid change configuration for mod: " + mod["modIdentifier"]);
                }
                dynamic changeCopy = serializer.Deserialize<object>(serializer.Serialize(change));

                try
                {
                    changeCopy["description"] = changeCopy["description"][language];
                }
                catch (KeyNotFoundException) {
                    changeCopy["description"] = "";
                }

                try
                {
                    changeCopy["detailedDescription"] = changeCopy["detailedDescription"][language];
                } catch (KeyNotFoundException) {
                    changeCopy["detailedDescription"] = "";
                }

                try
                {
                    for (int i = 0; i < changeCopy["selectionParameters"]["options"].Length; i++)
                    {
                        try
                        {
                            changeCopy["selectionParameters"]["options"][i]["description"] = changeCopy["selectionParameters"]["options"][i]["description"][language];
                        }
                        catch (KeyNotFoundException) { }

                        try
                        {
                            changeCopy["selectionParameters"]["options"][i]["detailedDescription"] = changeCopy["selectionParameters"]["options"][i]["detailedDescription"][language];
                        }
                        catch (KeyNotFoundException) { }
                    }
                } catch (KeyNotFoundException) { }

                changes.Add(changeCopy);
            }
            transformedMod["changes"] = changes;
            return transformedMod;
        }

        static List<string> compatibilityList = new List<string> { "crusader", "extreme", "singleplayer", "multiplayer" };

        static bool IsValidChange(dynamic change)
        {
            try
            {
                var compatibility = change["compatibility"];
                foreach (var env in compatibility)
                {
                    if (!compatibilityList.Contains(env))
                    {
                        return false;
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception("compatibility key is missing", e);
            }

            try
            {
                var identifier = change["identifier"];
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception("identifier key is missing", e);
            }

            try
            {
                string selectionType = change["selectionType"];
                Dictionary<string, object> selectionParameters = change["selectionParameters"];

                if (selectionType == "checkbox")
                {
                    if (selectionParameters.Count > 0)
                    {
                        return false;
                    }
                    try
                    {
                        var defaultValue = change["defaultValue"];
                        if (defaultValue != "true" && defaultValue != "false")
                        {
                            return false;
                        }
                    }
                    catch (KeyNotFoundException e)
                    {
                        throw new Exception("defaultValue key is missing" + e.Message, e);
                    }
                }

                if (selectionType == "radio")
                {
                    if (selectionParameters.Count != 1)
                    {
                        return false;
                    }
                    if (!selectionParameters.ContainsKey("options"))
                    {
                        return false;
                    }

                    object[] options = selectionParameters["options"] as dynamic;
                    try
                    {
                        var defaultValue = change["defaultValue"];
                        if (defaultValue != "false")
                        {
                            if (!options.Contains(defaultValue as string))
                            {
                                return false;
                            }
                        }
                    }
                    catch (KeyNotFoundException e)
                    {
                        throw new Exception("defaultValue key is missing", e);
                    }
                }

                if (selectionType == "slider")
                {
                    if (selectionParameters.Count != 5)
                    {
                        return false;
                    }
                    if (
                        !selectionParameters.ContainsKey("minimum") ||
                        !selectionParameters.ContainsKey("maximum") ||
                        !selectionParameters.ContainsKey("interval") ||
                        !selectionParameters.ContainsKey("default") ||
                        !selectionParameters.ContainsKey("suggested")
                        )
                    {
                        return false;
                    }
                    try
                    {
                        var defaultValue = change["defaultValue"];
                        if (defaultValue != "true" && defaultValue != "false")
                        {
                            return false;
                        }
                    }
                    catch (KeyNotFoundException e)
                    {
                        throw new Exception("defaultValue key is missing", e);
                    }
                }
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception(change["identifier"] + ": selectionType and/or selectionParameters key is missing: " + e.Message, e);
            }
            return true;
        }

        static string GetTranslations(string identifier)
        {
            return Localization.Get(identifier);
        }


        /*public static object GetUCPConfiguration()
        {
            Dictionary<string, AIVConfiguration> aivConfiguration = AIVEnumerator.GetAIVConfiguration();
            List<AICConfiguration> aicConfiguration = AICEnumerator.GetAICConfiguration();
            Dictionary<string, StartResourceConfiguration> startResourceConfiguration = StartResourceEnumerator.GetStartResourceConfigurations();
            Dictionary<string, StartTroopConfiguration> startTroopConfiguration = StartTroopEnumerator.GetStartTroopConfigurations();
            Dictionary<string, IEnumerable<string>> modConfiguration = Mod.ModList;

            List<object> ucpConfiguration = new List<object>();
            foreach (KeyValuePair<string, AIVConfiguration> keyValuePair in aivConfiguration)
            {
                dynamic modConfig = new object();
                modConfig.name = keyValuePair.Key;
                modConfig.type = "AIV";
                modConfig.description = keyValuePair.Value.Description;

                IEnumerable<object> castles = keyValuePair.Value.Description.Values.AsEnumerable(); 
                //keyValuePair.Value.Castles: { filename/id, description, image }
                // modConfig["modChanges"] = keyValuePair.Value.Castles
                // modConfig.selectionRules = "*"

                //modConfig["modChanges"] 
                //ucpConfiguration.Add()
            }

            foreach (AICConfiguration aic in aicConfiguration)
            {
                dynamic modConfig = new object();
                modConfig.name = aic.Identifier;
                modConfig.type = "AIC";
                modConfig.description = aic.Description;
                modConfig["modChanges"] = aic.CustomCharacterList;
            }

            foreach (KeyValuePair<string, StartTroopConfiguration> troop in startTroopConfiguration)
            {
                dynamic modConfig = new object();
                modConfig.name = troop.Key;
                modConfig.type = "troop";
                modConfig.description = troop.Value.Description;
                modConfig["modChanges"] = troop.Key;
            }

            foreach (KeyValuePair<string, StartResourceConfiguration> res in startResourceConfiguration)
            {
                dynamic modConfig = new object();
                modConfig.name = res.Key;
                modConfig.type = "resource";
                modConfig.description = res.Value.Description;
                modConfig["modChanges"] = res.Key;
            }

            foreach (KeyValuePair<string, IEnumerable<string>> mod in modConfiguration)
            {
                dynamic modConfig = new object();
                modConfig.name = mod.Key;
                modConfig.type = "generic";
                //modConfig.description = mod.Value.Description;
                //modConfig["modChanges"] = mod.Value.Changes;
                //Change = { id, description, selectiontype, selectionparameter, compatibility, defaultValue }
            }

            return null;
        }*/

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
            Startup.SetAICConfiguration(config.AIC);
            Startup.SetAIVConfiguration(config.AIV);
            Startup.SetStartResourceConfiguration(config.StartResource);
            Startup.SetStartTroopConfiguration(config.StartTroop);
            Startup.SetModValues(config.GenericMods);
            Startup.SetModExtremeValues(config.GenericMods);


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
