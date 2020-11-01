using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * AI OVERCLOCK
     */
    public class Mod_Change_AI_Overclock : Mod
    {
        public Mod_Change_AI_Overclock() : base("ai_overclock")
        {
            this.changeList = new List<string>
            {
                "ai_overclock"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("ai_overclock", ChangeType.AILords)
            {
                new ValuedSubChange("ai_overclock")
                {
                    // 0045CC20+6
                    new BinaryEdit("ai_overclock")
                    {
                        new BinInt32Value(200) // default: 200
                    }
                }
            };
        }
    }
}