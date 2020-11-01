using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * MAKES MARKETPLACE FREE
     */
    public class Mod_Change_Other_FreeTradePost : Mod
    {
        public Mod_Change_Other_FreeTradePost() : base("o_freetrader")
        {
            this.changeList = new List<string>
            {
                "o_freetrader"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // trader post: runtime 01124EFC
            // 005C23D8
            return BinBytes.Change("o_freetrader", ChangeType.Other, true, 0x00);
        }
    }
}