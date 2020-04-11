using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCPAIConversion
{
    class AICollection
    {
        public Dictionary<String, String> AICShortDescription { get; set; }
        public List<AICharacter> AICharacters { get; set; }

        public List<AICharacterName> GetCharacters()
        {
            return AICharacters.Select(character => (AICharacterName)Enum.Parse(typeof(AICharacterName), character.Index.ToString())).ToList();
        }

        public List<String> GetCustomCharacterNames()
        {
            return AICharacters.Select(character => character.Name.Substring(0, Math.Min(character.Name.Length, 20))).ToList();
        }

        public List<Int32> GetIndices()
        {
            return AICharacters.Select(character => character.Index).ToList();
        }
    }
}
