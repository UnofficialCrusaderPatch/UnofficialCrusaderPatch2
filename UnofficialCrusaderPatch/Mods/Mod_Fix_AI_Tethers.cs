namespace UCP.Patching
{
    /**
     * OX TETHER SPAM
     */
    public class Mod_Fix_AI_Tethers : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            // 004EFF9A => jne to jmp
            return BinBytes.Change("ai_tethers", ChangeType.Bugfix, true, 0x90, 0xE9);
        }
    }
}