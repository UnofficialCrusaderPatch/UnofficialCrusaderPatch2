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
            return AICharacters.Select(character => character.Name).ToList();
        }

        public List<String> GetCustomCharacterNames()
        {
            return AICharacters.Select(character => character.CustomName.Substring(0, Math.Min(character.CustomName.Length, 20))).ToList();
        }
    }
}
