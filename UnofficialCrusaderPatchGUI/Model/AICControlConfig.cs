using System;
using System.Collections.Generic;

namespace UCP.Model
{
    class AICControlConfig
    {
        internal string Identifier { get; set; }
        internal Func<bool> Enabled { get; set; }
        internal Func<List<string>> CharacterList { get; set; }
        
        public AICControlConfig withIdentifier(string identifier)
        {
            this.Identifier = identifier;
            return this;
        }

        public AICControlConfig withEnabled(Func<bool> enabled)
        {
            this.Enabled = enabled;
            return this;
        }

        public AICControlConfig withCharacterList(Func<List<string>> characterList)
        {
            this.CharacterList = characterList;
            return this;
        }
    }
}
