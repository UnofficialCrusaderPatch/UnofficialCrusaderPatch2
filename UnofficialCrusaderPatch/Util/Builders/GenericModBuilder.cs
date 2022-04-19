using System;
using System.Collections.Generic;
using UCP.Model;

namespace UCP.Util.Builders
{
    public class GenericModBuilder
    {
        static List<string> compatibilityList = new List<string> { "crusader", "extreme", "singleplayer", "multiplayer" };

        internal static ModUIConfig ConstructMod(ModBackendConfig mod, string language)
        {
            ModUIConfig transformedMod = new ModUIConfig();

            transformedMod.modIdentifier = mod.modIdentifier;
            transformedMod.modType = mod.modType;
            transformedMod.modDescription = mod.modDescription[language];

            try
            {
                transformedMod.detailedDescription = mod.detailedDescription[language];
            }
            catch (KeyNotFoundException)
            {
                transformedMod.detailedDescription = "";
            }
            catch (NullReferenceException)
            {
                transformedMod.detailedDescription = "";
            }

            List<ChangeUIConfig> changes = new List<ChangeUIConfig>();
            foreach (ChangeBackendConfig change in mod.changes)
            {
                if (!IsValidChange(change))
                {
                    throw new Exception("Invalid change configuration for mod: " + mod.modIdentifier);
                }
                ChangeUIConfig transformedChange = new ChangeUIConfig();
                transformedChange.identifier = change.identifier;
                transformedChange.selectionType = change.selectionType;

                try
                {
                    transformedChange.description = change.description[language];
                }
                catch (KeyNotFoundException)
                {
                    transformedChange.description = "";
                }

                try
                {
                    transformedChange.detailedDescription = change.detailedDescription[language];
                }
                catch (KeyNotFoundException)
                {
                    transformedChange.detailedDescription = "";
                }
                catch (NullReferenceException)
                {
                    transformedChange.detailedDescription = "";
                }

                transformedChange.selectionParameters = change.selectionParameters;
                if (change.selectionParameters.ContainsKey("options"))
                {
                    try
                    {
                        for (int i = 0; i < change.selectionParameters["options"].Count; i++)
                        {
                            try
                            {
                                transformedChange.selectionParameters["options"][i]["description"] = change.selectionParameters["options"][i]["description"][language];
                            }
                            catch (KeyNotFoundException) { }

                            try
                            {
                                transformedChange.selectionParameters["options"][i]["detailedDescription"] = change.selectionParameters["options"][i]["detailedDescription"][language];
                            }
                            catch (KeyNotFoundException) { }
                        }
                    }
                    catch (KeyNotFoundException) { }
                }

                changes.Add(transformedChange);
            }
            transformedMod.changes = changes;
            return transformedMod;
        }

        static bool IsValidChange(ChangeBackendConfig change)
        {
            try
            {
                var compatibility = change.compatibility;
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
                var identifier = change.identifier;
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception("identifier key is missing", e);
            }

            try
            {
                SelectionType selectionType = change.selectionType;
                Dictionary<string, dynamic> selectionParameters = change.selectionParameters;

                if (selectionType == SelectionType.CHECKBOX)
                {
                    if (selectionParameters.ContainsKey("options"))
                    {
                        return false;
                    }
                    try
                    {
                        var defaultValue = change.defaultValue;
                        if (!defaultValue.ToString().Equals("true") && !defaultValue.ToString().Equals("false"))
                        {
                            return false;
                        }
                    }
                    catch (KeyNotFoundException e)
                    {
                        throw new Exception("defaultValue key is missing" + e.Message, e);
                    }
                }

                if (selectionType == SelectionType.RADIO)
                {
                    if (selectionParameters["options"].Count == 0)
                    {
                        return false;
                    }

                    List<dynamic> options = selectionParameters["options"] as List<dynamic>;
                    try
                    {
                        string defaultValue = change.defaultValue.ToString();
                        if (!defaultValue.ToString().Equals("false"))
                        {
                            if (!options.Exists(x => x["value"].ToString().Equals(defaultValue as string)))
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

                if (selectionType == SelectionType.SLIDER)
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
                        var defaultValue = change.defaultValue;
                        if (!defaultValue.ToString().Equals("true") && !defaultValue.ToString().Equals("false"))
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
                throw new Exception(change.identifier + ": selectionType and/or selectionParameters key is missing: " + e.Message, e);
            }
            return true;
        }

    }
}
