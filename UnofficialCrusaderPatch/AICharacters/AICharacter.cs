using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.AICharacters
{
    /// <summary>
    /// The topmost character class which contains the AI's personality, its Crusader-intern index and more in the future
    /// </summary>
    public class AICharacter
    {
        public const string SectionTitle = "Character";

        public AICIndex Index;
        public AIPersonality Personality;

        public void Write(LineWriter lw)
        {
            lw.OpenSec(SectionTitle);

            // write properties
            lw.WriteEnum("Index", this.Index);
            lw.WriteLine();

            // write the personality
            this.Personality.Write(lw);

            lw.CloseSec();
            lw.WriteLine();
        }

        public static AICharacter Read(LineReader lr)
        {
            if (!lr.OpenSec(SectionTitle))
                return null;

            AICharacter result = new AICharacter();
            result.Index = lr.ReadEnum<AICIndex>();
            result.Personality = AIPersonality.Read(lr);

            lr.CloseSec();
            return result;
        }
    }
}
