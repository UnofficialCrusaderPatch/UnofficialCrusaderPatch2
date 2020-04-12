using System;
using System.Web.Script.Serialization;

namespace UCPAIConversion
{
    class AICharacter
    {
        [ScriptIgnore]
        public AICharacterName _Name { get; set; }

        [ScriptIgnore]
        public String _CustomName { get; set; }

        public string Name
        {
            get => Enum.GetName(typeof(AICharacterName), _Name);
            set
            {
                _Name = (AICharacterName)Enum.Parse(typeof(AICharacterName), value);
            }
        }

        public String CustomName
        {
            get => _CustomName;
            set
            {
                _CustomName = (value != null ? value.Substring(0, Math.Min(value.Length, 20)) : String.Empty);
            }
        }

        public AIPersonality Personality { get; set; }
    }
}
