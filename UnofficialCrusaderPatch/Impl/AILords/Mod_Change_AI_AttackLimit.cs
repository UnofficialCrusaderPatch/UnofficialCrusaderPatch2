using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * AI RECRUIT ADDITIONAL ATTACK TROOPS
     */
    public class Mod_Change_AI_AttackLimit : Mod
    {


        public Mod_Change_AI_AttackLimit() : base("ai_attacklimit")
        {
            this.changeList = new List<string>
            {
                "ai_attacklimit_slider"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 115EEE0 + (AI1 = 73E8) = stay home troops?
            // +8 attack troops

            // absolute limit at 0x4CDEF8 + 1 = 200
            return new Change("ai_attacklimit", ChangeType.AILords)
            {
                new ValuedSubChange("ai_attacklimit_slider")
                {
                    new BinaryEdit("ai_attacklimit")
                    {
                        new BinInt32Value()
                    },
                }
            };
        }
    }
}