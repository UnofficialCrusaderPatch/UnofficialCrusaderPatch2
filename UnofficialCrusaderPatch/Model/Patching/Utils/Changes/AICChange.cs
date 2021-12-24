using System;
using System.Collections.Generic;
using UCP.Model;
using UCP.Patching;
using UCPAIConversion;

namespace UCP.AIC
{
    class AICChange : Change
    {
        internal AICollection collection;
        internal List<AICharacterName> characters;
        internal List<String> customCharacterNames;
        internal AICChange(string identifier)
            : base(identifier, ChangeType.AIC)
        {
        }
    }
}
