namespace UCP.Patching
{
    /**
     * CHANGE LADDERMEN STATS
     */
    public class Mod_Change_Units_Laddermen : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            //BinInt32.Change("laddermadness", ChangeType.Troops, 1),

            
            // Armbrust dmg table: 0xB4ED20
            // Bogen dmg table: 0xB4EAA0
            // Sling dmg table: 0xB4EBE0

            // Schutz von Leiternträgern gegen Fernkämpfer
            return new Change("u_laddermen", ChangeType.Troops)
            {
                new DefaultHeader("u_laddermen")
                {
                    BinInt32.CreateEdit("u_ladderarmor_bow", 420), // B4EAA0 + 4 * 1D   (vanilla = 1000)
                    BinInt32.CreateEdit("u_ladderarmor_sling", 1000), // B4EBE0 + 4 * 1D   (vanilla = 2500)
                    BinInt32.CreateEdit("u_ladderarmor_xbow", 1000), // B4ED20 + 4 * 1D   (vanilla = 2500)

                    // 0052EC37 + 2
                    BinBytes.CreateEdit("u_laddergold", 0xF7), // 1D - 9 = 14h            (vanilla: 1D - 19 = 4)
                    
                    new BinaryEdit("ui_fix_laddermen_cost_display_in_engineers_guild") // F5C91
                    {
                        new BinBytes(0xBB, 0x14),
                    }
                }
            };
        }
    }
}