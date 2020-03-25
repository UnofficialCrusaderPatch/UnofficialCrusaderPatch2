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
            string headerSubstring = aicSrcFile.Substring(headerTitleEnd, end);
            if (headerSubstring.IndexOf('=') > -1)
            {
                string[] headerLines = headerSubstring.Split('\n');
                foreach(string line in headerLines)
                {
                    string[] descr = line.Split('=');
                    if (descr.Length > 1)
                    {
                        string descrKey = Regex.Replace(descr[0], @"\s +", "").Trim();
                        string descrValue = descr[1].Trim();
                        descriptions.Add('"' + descrKey + "\": " + '"' + descrValue + "\",");
                    }
                }
            }
            else
            {
                string[] headerLines = aicSrcFile.Substring(headerTitleEnd, end).Split(new string[] { "Descr" }, StringSplitOptions.None);
                List<string> descrLines = new List<string>();
                foreach(string line in headerLines)
                {
                    string trimmedLine = Regex.Replace(line.Trim(), @"[\n\r\t]+", String.Empty);
                    if (trimmedLine != String.Empty)
                    {
                        string description = trimmedLine.Substring(4);
                        description = description.Replace("\"", "\\\"");
                        int indexEnd = description.IndexOf("}");
                        description = description.Substring(0, indexEnd);
                        descriptions.Add("\"Descr" + trimmedLine.Substring(0, 3) + "\": \"" + description + "\",");
                    }
                }
            }
            string headerJson = "\"AIDescription\": {\n\t\t";
            headerJson = headerJson + String.Join("\n\t\t", descriptions);

            if (headerJson.Length > 1)
            {
                headerJson = headerJson.Substring(0, headerJson.Length - 1);
            }
            headerJson = headerJson + "\n\t}";

            string[] characterSearch = aicSrcFile.Split(new string[] { "AICharacter" }, StringSplitOptions.None);
            string[] characters = new string[characterSearch.Length-1];
            Array.Copy(characterSearch, 1, characters, 0, characters.Length);

            string aicJSON = "{\n" + headerJson + ",\n\n" + "\"AICharacters\": [";
            foreach(string character in characters)
            {
                string[] personality = character.Split(new string[] { "Personality" }, StringSplitOptions.None);
                string[] characterID = personality[0].Split('\n');

                List<string> personalityIDs = new List<string>();
                personalityIDs.Add("\"Description\": \"AICharacter\"");

                foreach(string identifier in characterID)
                {
                    string parsedIdentifier = Regex.Replace(identifier, @"\s+", String.Empty);
                    string[] idFields = identifier.Split('=');
                    if (idFields.Length > 1)
                    {
                        try
                        {
                            personalityIDs.Add('"' + idFields[0].Trim() + "\": " + int.Parse(idFields[1]).ToString());
                        }
                        catch (FormatException)
                        {
                            personalityIDs.Add('"' + idFields[0].Trim() + "\": \"" + idFields[1].Trim() + "\"");
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
            return fieldName;
        }
    }
}
