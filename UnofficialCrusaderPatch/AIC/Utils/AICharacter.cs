using System;
using System.Web.Script.Serialization;

namespace UCPAIConversion
{
    class AICharacter
    {

        private Int32 _index;

        public String Description { get; set; }

        public AICharacterName Name { get; set; }

        public String CustomName { get; set; }

        public AIPersonality Personality { get; set; }
    }
}
