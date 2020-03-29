using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCPAIConversion
{
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
            } else
            {
                return "AIC Errors found in " + aicFile + ":\n" + String.Join("\n", ErrorList);
            }
        }
    }
}
