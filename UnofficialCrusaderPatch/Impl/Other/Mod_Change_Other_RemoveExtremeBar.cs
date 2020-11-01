using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * REMOVE EXTREME BAR
     */
    public class Mod_Change_Other_RemoveExtremeBar : Mod
    {
        public Mod_Change_Other_RemoveExtremeBar() : base("o_xtreme")
        {
            this.changeList = new List<string>
            {
                "o_xtreme"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_xtreme", ChangeType.Other)
            {
                new DefaultSubChange("o_xtreme")
                {
                    // 0057CAC5 disable manabar rendering
                    BinNops.CreateEdit("o_xtreme_bar1", 10),
                    
                    // 4DA3E0 disable manabar clicks
                    BinBytes.CreateEdit("o_xtreme_bar2", 0xC3),
                }
            };
        }
    }
}