using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AIConversion
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

        [ScriptIgnore]
        public AICharacterName _Name { get; set; }

        public string Name
        {
            get => Enum.GetName(typeof(AICharacterName), _Name);
            set
            {
                _Name = (AICharacterName)Enum.Parse(typeof(AICharacterName), value);
            }
        }
        public AIPersonality Personality { get; set; }
    }
}
