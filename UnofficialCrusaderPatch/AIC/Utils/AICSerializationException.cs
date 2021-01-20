using System;
using System.Collections.Generic;

namespace UCPAIConversion
{
    /// <summary>
    /// Exception class for containing list of all exceptions found when parsing an AIC
    /// Comprises of a list of AICharacterSerializationException instances
    /// </summary>
    class AICSerializationException : FormatException
    {
        public List<AICharacterSerializationException> ErrorList { get; set; }

        public AICSerializationException()
        {
            this.ErrorList = new List<AICharacterSerializationException>();
        }

        public String ToErrorString(string aicFile)
        {
            if (aicFile == null)
            {
                return "AIC Errors found:\n" + String.Join("\n", ErrorList);
            }
            else
            {
                return "AIC Errors found in " + aicFile + ":\n" + String.Join("\n", ErrorList);
            }
        }
    }
}
