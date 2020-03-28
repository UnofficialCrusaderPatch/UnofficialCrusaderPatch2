using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCPAIConversion
{
    class AICharacterSerializationException : FormatException
    {
        public String AssociatedAICharacter { get; set; }
        public List<String> Errors { get; set; }

        public AICharacterSerializationException()
        {
            this.Errors = new List<string>();
        }

        override public String ToString()
        {
            return "Errors found in " + AssociatedAICharacter + ":\n" + String.Join("\n", Errors);
        }
    }
}
