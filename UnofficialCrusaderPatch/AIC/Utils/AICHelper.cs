using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace UCPAIConversion
{
    /// <summary>
    /// Performs conversion of older .aic AIC files to the newer JSON format
    /// </summary>
    internal class AICHelper
    {
        public static bool Convert(string srcFile, string destFile)
        {
            StreamReader reader = new StreamReader(srcFile, Encoding.UTF8);
            string aicSrcFile = reader.ReadToEnd();
            reader.Close();

            aicSrcFile = Regex.Replace(aicSrcFile, "/[*]([^*]|([*][^/]))*[*]+/", String.Empty);

            Match header = Regex.Match(aicSrcFile, @"AIFileHeader[\n.\s]+{");
            int start = header.Index;
            int headerTitleEnd = header.Index + header.Length;
            int end = aicSrcFile.IndexOf("AICharacter", start);


            // Variables for parsing and storing descriptions
            List<string> descriptions = new List<string>();
            string[]     headerLines  = aicSrcFile.Substring(headerTitleEnd, end).Split(new[] { "Descr" }, StringSplitOptions.None);

            List<string> descrLines = new List<string>();

            // Parse AIFileHeader and store decriptions based on the defined language
            String language = "";
            for (int i = 0; i < headerLines.Length; i++)
            {
                string line = headerLines[i];

                // Ignore empty lines
                if (Regex.Replace(line.Trim(), @"[\n\r\t]+", String.Empty).Equals(String.Empty))
                {
                    continue;
                }

                if (line.StartsWith("Ger"))
                {
                    language = "German";
                } 
                else if (line.StartsWith("Eng"))
                {
                    language = "English";
                }
                else if (line.StartsWith("Rus"))
                {
                    language = "Russian";
                }
                else if (line.StartsWith("Pol"))
                {
                    language = "Polish";
                }
                
                if (i == headerLines.Length - 1)
                {
                    line = line.Substring(0, line.LastIndexOf("AICharacter")).Trim("\r\n\t =}".ToCharArray());
                }

                // Remove leading and ending punctuation and whitespace
                string description = line;
                if (!Regex.Replace(line.Trim(), @"[\n\r\t]+", String.Empty).Equals(String.Empty))
                {
                    description = description.Substring(Math.Min(description.Length, 3));
                    description = description.Trim("\n\r\t ".ToCharArray());
                    if (description.StartsWith("=") || description.StartsWith("{"))
                    {
                        description = description.Substring(1).Trim("\n\r\t ".ToCharArray());
                    }
                    if (description.EndsWith("{"))
                    {
                        description = description.Substring(0, description.Length - 1).Trim("\n\r\t ".ToCharArray());
                    }
                }
                description = description.Replace("\"", "\\\"");

                // Remove leading and ending punctuation and whitespace
                if (description.EndsWith("{") || description.EndsWith("}") || description.EndsWith("="))
                {
                    description = description.Substring(0, description.Length - 1).Trim("\n\r\t ".ToCharArray());
                }
                description = Regex.Replace(description, "\t", "  ");

                descriptions.Add("\"" + language + "\":" + " \"" + description + "\"");
            }

            string headerJson = "\"AICShortDescription\": {\n  ";
            headerJson += String.Join(",\n  ", descriptions);
            headerJson += "\n  }";

            // Remove all comments from the file
            aicSrcFile = Regex.Replace(aicSrcFile, "/[*]([^*]|([*][^/]))*[*]+/", "");
            string[] characterSearch = aicSrcFile.Split(new[] { "AICharacter" }, StringSplitOptions.None);
            string[] characters      = new string[characterSearch.Length -1];

            // Copy file text starting from the first AICharacter definition to be parsed
            Array.Copy(characterSearch, 1, characters, 0, characters.Length);

            string aicJSON = "{\n" + headerJson + ",\n\n" + "\"AICharacters\": [";
            foreach(string character in characters)
            {
                string[] personality = character.Split(new[] { "Personality" }, StringSplitOptions.None);
                string[] characterID = personality[0].Split('\n');

                List<string> personalityIDs = new List<string>();

                // Find the index key
                AICharacterName currentCharacterName = 0;
                bool indexFound = false;
                foreach (string identifier in characterID)
                {
                    string parsedIdentifier = Regex.Replace(identifier, @"\s+", String.Empty);
                    string[] idFields = parsedIdentifier.Split('=');
                    if (idFields[0].Trim() == "Index")
                    {
                        personalityIDs.Add('"' + "Name" + "\": \"" + Enum.GetName(typeof(AICharacterName), int.Parse(idFields[1])) + "\"");
                        currentCharacterName = (AICharacterName)int.Parse(idFields[1]);
                        indexFound = true;
                    }
                }

                // Convert the name key and all other keys
                if (indexFound)
                {
                    foreach (string identifier in characterID)
                    {
                        string parsedIdentifier = Regex.Replace(identifier, @"\s+", String.Empty);
                        string[] idFields = identifier.Split('=');
                        if (idFields.Length > 1)
                        {
                            try
                            {
                                if (idFields[0].Trim() == "Name")
                                {
                                    if (idFields[1].Trim() == "Philipp")
                                    {
                                        idFields[1] = "Phillip";
                                    }

                                    string indexName = currentCharacterName.ToString();

                                    // Save CustomName if set else output blank CustomName
                                    if (indexName != idFields[1].Trim())
                                    {
                                        personalityIDs.Add("\"CustomName\": \"" + idFields[1].Trim().Substring(0, Math.Min(idFields[1].Trim().Length, 20)) + '"');
                                    }
                                    else
                                    {
                                        personalityIDs.Add("\"CustomName\": \"\"");
                                    }
                                }
                                else
                                {
                                    // Attempt to save value of field as a numeric type
                                    personalityIDs.Add('"' + idFields[0].Trim() + "\": " + int.Parse(idFields[1]));
                                }
                            }
                            catch (FormatException) // Store value of field as string
                            {
                                personalityIDs.Add('"' + idFields[0].Trim() + "\": \"" + idFields[1].Trim() + "\"");
                            }
                        }
                    }
                }

                // Split the Personality definition by newlines and ignore lines without '=' as comments or invalid
                string[] characterData = personality[1].Split('\n');
                List<string> fields = new List<string>();
                foreach(string field in characterData)
                {
                    if (field.Contains("="))
                    {
                        fields.Add(field);
                    }
                }

                List<string> personalityFields = new List<string>();
                const int numPersonalityFields = 169;

                // If number of found fields is equal to the defined number of personality fields 
                // assume fields are in order
                if (fields.Count == numPersonalityFields)
                {
                    for (int index = 0; index < numPersonalityFields; index++)
                    {
                        string parsedField = Regex.Replace(fields[index], @"\s+", String.Empty);
                        string[] fieldData = parsedField.Split("=".ToCharArray());
                        if (fieldData.Length > 1)
                        {
                            string fieldName = Enum.GetName(typeof(AIPersonalityFieldsEnum), index);
                            try
                            {
                                personalityFields.Add('"' + fieldName + "\": " + int.Parse(fieldData[1]));
                            }
                            catch (FormatException)
                            {
                                personalityFields.Add('"' + fieldName + "\": \"" + fieldData[1] + "\"");
                            }
                        }
                    }
                }
                else
                {
                    // Iterate fields and store based on field name defined in file
                    foreach (string field in fields)
                    {
                        string parsedField = Regex.Replace(field, @"\s+", String.Empty);
                        string[] fieldData = parsedField.Split("=".ToCharArray());
                        if (fieldData.Length > 1)
                        {
                            string fieldName = UpdateFieldName(fieldData[0]);
                            try
                            {
                                personalityFields.Add('"' + fieldName + "\": " + int.Parse(fieldData[1]));
                            }
                            catch (FormatException)
                            {
                                personalityFields.Add('"' + fieldName + "\": \"" + fieldData[1] + "\"");
                            }
                        }
                    }
                }

                string characterJson = "\n\t\t" + String.Join(",\n\t\t", personalityIDs) + ",\n\t\t" + 
                    "\"Personality\" : {" + "\n\n\t\t\t" + String.Join(",\n\t\t\t", personalityFields) + "\n\t\t}\n\t}";

                aicJSON += "\n\t{" + characterJson + ",";
            }
            aicJSON = aicJSON.Substring(0, aicJSON.Length - 2) + "}]\n}";
            File.WriteAllText(destFile, aicJSON, Encoding.UTF8);
            return true;
        }

        /// <summary>
        /// Replaces outdated fieldnames with their updated counterpart to serve as key in JSON file
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private static string UpdateFieldName(string fieldName)
        {
            switch (fieldName)
            {
                case "Unknown131":
                    return "AttUnitPatrolRecommandDelay";
                case "RangedBackupUnitGroupsCount":
                case "RangedBackupGroupsCount":
                    return "AttUnitBackupGroupsCount";
                case "MinimumGoodsRequiredAfterTribute":
                    return "MinimumGoodsRequiredAfterTrade";
                case "InvestmentGoldThreshold":
                    return "RecruitGoldThreshold";
                case "Unknown161":
                    return "AttUnitSiegeDefGroupsCount";

                // v2.14 -> v2.15
                case "Unknown000":
                    return "WallDecoration";
                case "Unknown040":
                    return "AIRequestDelay";
                case "Unknown124":
                    return "RaidRetargetDelay";
                case "Unknown130":
                    return "AttAssaultDelay";
                case "AttUnit2":
                    return "AttUnitVanguard";
                case "AttUnit2Max":
                    return "AttUnitVanguardMax";
                default:
                    return fieldName;
            }
        }
    }
}
