using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCPAIConversion
{
    class AICollection
    {
        public AIHeader AIDescription { get; set; }
        public List<AICharacter> AICharacters { get; set; }

        public List<AICharacterName> GetCharacters()
        {
            return AICharacters.Select(character => character._Name).ToList();
        }
    }
}
