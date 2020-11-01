using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * OX TETHER SPAM
     */
    public class Mod_Fix_AI_Tethers : Mod
    {
        public Mod_Fix_AI_Tethers() : base("ai_tethers")
        {
            this.changeList = new List<string>
            {
                "ai_tethers"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 004EFF9A => jne to jmp
            return BinBytes.Change("ai_tethers", ChangeType.Bugfix, true, 0x90, 0xE9);
        }
    }
}