using System;
using System.Web.Script.Serialization;

namespace UCPAIConversion
{
    /// <summary>
    /// Representation of an AI character through its defining attributes
    /// </summary>
    internal class AICharacter
    {
        [ScriptIgnore]
        public AICharacterName _Name { get; set; }

        [ScriptIgnore]
        public String _CustomName { get; set; }

        /// <summary>
        /// Name of the AI Character as defined in SHC
        /// </summary>
        public string Name
        {
            get => Enum.GetName(typeof(AICharacterName), _Name);
            set => _Name = (AICharacterName)Enum.Parse(typeof(AICharacterName), value);
        }

        /// <summary>
        /// Creator-assigned name of the AI Character as defined in the AIC
        /// </summary>
        public String CustomName
        {
            get => _CustomName;
            set => _CustomName = (value != null ? value.Substring(0, Math.Min(value.Length, 20)) : String.Empty);
        }

        /// <summary>
        /// List of parameters that define an AIs behaviour
        /// </summary>
        public AIPersonality Personality { get; set; }
    }
}
