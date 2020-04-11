using System;
using System.Web.Script.Serialization;

namespace UCPAIConversion
{
    class AICharacter
    {

        private Int32 _index;

        public String Description { get; set; }
        
        public Int32 Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (value < 1 || value > 16)
                {
                    throw new ArgumentException();
                }
                _index = value;
            }
        }

        public String Name { get; set; }

        public AIPersonality Personality { get; set; }
    }
}
