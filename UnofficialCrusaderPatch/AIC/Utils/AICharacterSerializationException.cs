using System;
using System.Collections.Generic;

namespace UCPAIConversion
{
    /// <summary>
    /// Exception class for containing list of all exceptions found when parsing a single character
    /// </summary>
    internal class AICharacterSerializationException : FormatException
    {
        public String AssociatedAICharacter { get; set; }
        public List<String> Errors { get; set; }

        public AICharacterSerializationException()
        {
            Errors = new List<string>();
        }

        public override String ToString()
        {
            return "Errors found in " + AssociatedAICharacter + ":\n" + String.Join("\n", Errors);
        }
    }
}
