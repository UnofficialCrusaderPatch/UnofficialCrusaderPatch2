using Newtonsoft.Json;
using System.Collections.Generic;

namespace UCP.Model
{
    public class AICConfiguration
    {
        public string Identifier { get; set; }
        public List<string> CharacterList { get; set; }

        [JsonIgnore]
        public List<string> CustomCharacterList { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> Description { get; set; }

        public AICConfiguration withIdentifier(string identifier)
        {
            this.Identifier = identifier;
            return this;
        }
        public AICConfiguration withCharacterList(List<string> characterList)
        {
            this.CharacterList = characterList;
            return this;
        }
        public AICConfiguration withCustomCharacterList(List<string> customCharacterList)
        {
            this.CustomCharacterList = customCharacterList;
            return this;
        }
        public AICConfiguration withDescription(Dictionary<string, string> description)
        {
            this.Description = description;
            return this;
        }
    }
}
