using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.AICharacters
{
    /// <summary>
    /// The topmost character class which contains the AI's personality, its Crusader-intern index and more in the future
    /// </summary>
    public class AICharacter
    {
        public int Index;
        public string Name;
        public AIPersonality Personality;
    }
}
