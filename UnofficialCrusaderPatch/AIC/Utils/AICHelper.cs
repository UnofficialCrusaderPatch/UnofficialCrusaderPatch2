using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UCPAIConversion
{
    class AICHelper
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

            List<string> descriptions = new List<string>();
            string[] headerLines = aicSrcFile.Substring(headerTitleEnd, end).Split(new string[] { "Descr" }, StringSplitOptions.None);

            List<string> descrLines = new List<string>();
            String language = "";
            for (int i = 0; i < headerLines.Length; i++)
            {
                string line = headerLines[i];
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

                if (description.EndsWith("{") || description.EndsWith("}") || description.EndsWith("="))
                {
                    description = description.Substring(0, description.Length - 1).Trim("\n\r\t ".ToCharArray());
                }
                description = Regex.Replace(description, "\t", "  ");

                descriptions.Add("\"" + language + "\":" + " \"" + description + "\"");
            }

            string headerJson = "\"AICShortDescription\": {\n  ";
            headerJson = headerJson + String.Join(",\n  ", descriptions);
            headerJson = headerJson + "\n  }";


            aicSrcFile = Regex.Replace(aicSrcFile, "/[*]([^*]|([*][^/]))*[*]+/", "");
            string[] characterSearch = aicSrcFile.Split(new string[] { "AICharacter" }, StringSplitOptions.None);
            string[] characters = new string[characterSearch.Length-1];
            Array.Copy(characterSearch, 1, characters, 0, characters.Length);

            string aicJSON = "{\n" + headerJson + ",\n\n" + "\"AICharacters\": [";
            foreach(string character in characters)
            {
                string[] personality = character.Split(new string[] { "Personality" }, StringSplitOptions.None);
                string[] characterID = personality[0].Split('\n');

                List<string> personalityIDs = new List<string>();

                //find the index key
                AICharacterName currentCharacterName = 0;
                bool indexFound = false;
                foreach (string identifier in characterID)
                {
                    string parsedIdentifier = Regex.Replace(identifier, @"\s+", String.Empty);
                    string[] idFields = parsedIdentifier.Split('=');
                    if (idFields[0].Trim() == "Index")
                    {
                        personalityIDs.Add('"' + "Name" + "\": " + Enum.GetName(typeof(AICharacterName), idFields[1]));
                        currentCharacterName = (AICharacterName)int.Parse(idFields[1]);
                        indexFound = true;
                    }
                }

                //convert the name key and all other keys
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
                                        idFields[1] = "Phillip";
                                    string indexName = currentCharacterName.ToString();
                                    if (indexName != idFields[1].Trim())
                                        //custom name is set
                                        personalityIDs.Add("\"CustomName\": \"" + int.Parse(idFields[1]).ToString() + '"');
                                    else
                                        //no custom name set
                                        personalityIDs.Add("\"CustomName\": \"\"");
                                }
                                else
                                {
                                    personalityIDs.Add('"' + idFields[0].Trim() + "\": " + int.Parse(idFields[1]).ToString());
                                }
                            }
                            catch (FormatException)
                            {
                                personalityIDs.Add('"' + idFields[0].Trim() + "\": \"" + idFields[1].Trim() + "\"");
                            }
                        }
                    }
                }

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
                                personalityFields.Add('"' + fieldName + "\": " + int.Parse(fieldData[1]).ToString());
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
                    foreach (string field in fields)
                    {
                        string parsedField = Regex.Replace(field, @"\s+", String.Empty);
                        string[] fieldData = parsedField.Split("=".ToCharArray());
                        if (fieldData.Length > 1)
                        {
                            string fieldName = UpdateFieldName(fieldData[0]);
                            try
                            {
                                personalityFields.Add('"' + fieldName + "\": " + int.Parse(fieldData[1]).ToString());
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

        private static string UpdateFieldName(string fieldName)
        {
            if (fieldName == "Unknown131")
            {
                return "AttUnitPatrolRecommandDelay";
            }
            else if (fieldName == "RangedBackupUnitGroupsCount")
            {
                return "AttUnitBackupGroupsCount";
            }
            else if (fieldName == "MinimumGoodsRequiredAfterTribute")
            {
                return "MinimumGoodsRequiredAfterTrade";
            }
            else if (fieldName == "InvestmentGoldThreshold")
            {
                return "RecruitGoldThreshold";
            }
            return fieldName;
        }
    }
}
