using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCPAIConversion
{
    /// <summary>
    /// Stores the collection of AI Characters defined in an AIC file
    /// </summary>
    class AICollection
    {
        /// <summary>
        /// Dictionary of AIC descriptions (key = language, value = description)
        /// </summary>
        public Dictionary<String, String> AICShortDescription { get; set; }
        public List<AICharacter> AICharacters { get; set; }

        /// <summary>
        /// Convenience method to return ordered list of AI Character names as defined in SHC
        /// </summary>
        /// <returns></returns>
        public List<AICharacterName> GetCharacters()
        {
            return AICharacters.Select(character => character._Name).ToList();
        }

        /// <summary>
        /// Convenience method to return ordered list of the AI CustomNames
        /// </summary>
        /// <returns></returns>
        public List<String> GetCustomCharacterNames()
        {
            return AICharacters.Select(character => character.CustomName.Substring(0, Math.Min(character.CustomName.Length, 20))).ToList();
        }
    }
}
