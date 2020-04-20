namespace UCP.Patching
{
    /**
     * ALWAYS ATTACK NEAREST NEIGHBOR
     */
    public class Mod_Change_AI_AttackTarget : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            // 004D47B2
            return new Change("ai_attacktarget", ChangeType.AILords, false)
            {
                new DefaultHeader("ai_attacktarget_nearest", true)
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x11, 0x90)
                },

                new DefaultHeader("ai_attacktarget_richest", false)
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x3F, 0x90)
                },

                new DefaultHeader("ai_attacktarget_weakest", false)
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x52, 0x90)
                },
            };
        }
    }
}