namespace UCP.Patching
{
    /**
     * ECONOMY DEMOLISHING
     */
    public class Mod_Change_AI_Demolish : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 004D0280
            return new Change("ai_demolish", ChangeType.AILords, false, false)
            {
                new DefaultHeader("ai_demolish_walls", true)
                {
                    // 004D03F4  => jmp to end
                    BinBytes.CreateEdit("ai_demolish_walls", 0xE9, 0x15, 0x01, 0x00, 0x00, 0x90, 0x90)
                },

                new DefaultHeader("ai_demolish_trapped", false)
                {
                    // 004F1988  => jne to jmp
                    BinBytes.CreateEdit("ai_demolish_trapped", 0xEB)
                },

                new DefaultHeader("ai_demolish_eco", false)
                {
                    // 004D0280  => retn 8
                    BinBytes.CreateEdit("ai_demolish_eco", 0xC2, 0x08, 0x00, 0x90, 0x90, 0x90)
                },
            };
        }
    }
}