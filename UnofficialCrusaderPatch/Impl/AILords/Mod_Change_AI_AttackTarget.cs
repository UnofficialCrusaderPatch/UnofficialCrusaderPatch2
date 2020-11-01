using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * ALWAYS ATTACK NEAREST NEIGHBOR
     */
    public class Mod_Change_AI_AttackTarget : Mod
    {
        public Mod_Change_AI_AttackTarget() : base("ai_attacktarget")
        {
            this.changeList = new List<string>
            {
                "ai_attacktarget_nearest",
                "ai_attacktarget_richest",
                "ai_attacktarget_weakest"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 004D47B2
            return new Change("ai_attacktarget", ChangeType.AILords)
            {
                new DefaultSubChange("ai_attacktarget_nearest")
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x11, 0x90)
                },

                new DefaultSubChange("ai_attacktarget_richest")
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x3F, 0x90)
                },

                new DefaultSubChange("ai_attacktarget_weakest")
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x52, 0x90)
                },
            };
        }
    }
}