namespace UCP.Patching
{
    /**
     * REMOVE EXTREME BAR
     */
    public class Mod_Change_Other_RemoveExtremeBar : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_xtreme", ChangeType.Other, false)
            {
                new DefaultHeader("o_xtreme")
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