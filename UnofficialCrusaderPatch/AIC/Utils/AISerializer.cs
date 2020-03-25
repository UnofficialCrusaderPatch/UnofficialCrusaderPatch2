using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace UCPAIConversion
{
    class AISerializer : JavaScriptConverter
    {
        private readonly Dictionary<string, string> errorMessages;
        private readonly Dictionary<string, string> errorHints;

        private AICSerializationException AICSerializationExceptionList = new AICSerializationException();

        public AISerializer (Dictionary<String, String> errorMessages, Dictionary<String, String> errorHints)
        {
            this.errorMessages = errorMessages;
            this.errorHints = errorHints;
        }

        private String GetErrorMessage(String field, String character)
        {
            String msg = "Error parsing field " + field + ".";
            if (this.errorMessages.ContainsKey(field))
            {
                msg += " Reason: " + this.errorMessages[field];
            }
            if (this.errorHints.ContainsKey(field))
            {
                msg += "\nHint: " + this.errorHints[field];
            }
            return msg;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(AICollection) })); }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            AIHeader header = new AIHeader(); ;
            List<AICharacter> AICharacters = new List<AICharacter>();
            AICollection collection = new AICollection();

            Type AIHeaderType = typeof(AIHeader);
            Type AICharacterType = typeof(AICharacter);
            Type AIPersonalityType = typeof(AIPersonality);

            foreach (KeyValuePair<string, object> entry in dictionary)
            {
                AICharacterSerializationException SerializationErrors = new AICharacterSerializationException();
                SerializationErrors.Errors = new List<string>();

                if (entry.Key == "AIDescription")
                {
                    foreach (KeyValuePair<string, object> field in (Dictionary<string, object>)entry.Value)
                    {
                        try
                        {
                            SetProperty(AIHeaderType, header, field.Key, field.Value);
                        }
                        catch (ArgumentException)
                        {
                            SerializationErrors.Errors.Add(GetErrorMessage(field.Key, null));
                        }
                    }

                } else if (entry.Key == "AICharacters")
                {
                    AICharacter currentCharacter;
                    AIPersonality currentPersonality;
                    foreach (var character in (System.Collections.ArrayList)entry.Value)
                    {
                        currentCharacter = new AICharacter();
                        SerializationErrors = new AICharacterSerializationException();

                        foreach (KeyValuePair<string, object> definition in (Dictionary<string, object>)character)
                        {
                            if (definition.Key != "Personality")
                            {
                                try
                                {
                                    SetProperty(AICharacterType, currentCharacter, definition.Key, definition.Value);
                                }
                                catch (Exception e)
                                {
                                    if (e is TargetInvocationException || e is ArgumentException)
                                    {
                                        SerializationErrors.Errors.Add(GetErrorMessage(definition.Key, currentCharacter._Name.ToString()));
                                    }
                                }

                            } else if (definition.Key == "Personality")
                            {
                                currentPersonality = new AIPersonality();
                                foreach (KeyValuePair<string, object> personalityValue in (Dictionary<string, object>)definition.Value)
                                {
                                    if (personalityValue.Key.ToLowerInvariant().Contains("description") ||
                                        personalityValue.Key.ToLowerInvariant().Contains("comment"))
                                    {
                                        continue;
                                    }
                                    try
                                    {
                                        SetProperty(AIPersonalityType, currentPersonality, personalityValue.Key, personalityValue.Value);
                                    }
                                    catch (ArgumentException)
                                    {
                                        SerializationErrors.Errors.Add(GetErrorMessage(personalityValue.Key, currentCharacter._Name.ToString()));
                                    }
                                    catch (NullReferenceException)
                                    {
                                        SerializationErrors.Errors.Add(GetErrorMessage(personalityValue.Key, currentCharacter._Name.ToString()));
                                    }
                                }
                                currentCharacter.Personality = currentPersonality;
                            }
                        }
                        AICharacters.Add(currentCharacter);
                        if (SerializationErrors.Errors.Count > 0)
                        {
                            SerializationErrors.AssociatedAICharacter = currentCharacter._Name.ToString();
                            AICSerializationExceptionList.ErrorList.Add(SerializationErrors);
                        }
                    }
                }
            }
            if (AICSerializationExceptionList.ErrorList.Count > 0)
            {                
                throw AICSerializationExceptionList;
            }
            collection.AIDescription = header;
            collection.AICharacters = AICharacters;
            return collection;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private void SetProperty(Type targetType, object target, String expectedFieldName, object expectedFieldValue)
        {
            if (expectedFieldName == "Unknown131")
            {
                expectedFieldName = "AttUnitPatrolRecommandDelay";
            }
            PropertyInfo parameter = targetType.GetProperty(expectedFieldName);
            if (parameter.PropertyType == typeof(bool))
            {
                parameter.SetValue(target, Convert.ToBoolean(expectedFieldValue), null);
            }
            else
            {
                parameter.SetValue(target, expectedFieldValue, null);
            }
        }
    }
}
