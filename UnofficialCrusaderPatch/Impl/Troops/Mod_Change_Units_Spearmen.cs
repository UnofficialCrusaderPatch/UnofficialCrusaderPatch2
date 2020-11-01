using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * CHANGE SPEARMEN STATS
     */
    public class Mod_Change_Units_Spearmen : Mod
    {
        public Mod_Change_Units_Spearmen() : base("u_spearmen")
        {
            this.changeList = new List<string>
            {
                "u_spearmen"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // Lanzenträger hp: 10000
            return new Change("u_spearmen", ChangeType.Troops)
            {
                new DefaultSubChange("u_spearmen")
                {
                    BinInt32.CreateEdit("u_spearbow", 2000), // B4EAA0 + 4 * 18   (vanilla = 3500)
                    BinInt32.CreateEdit("u_spearxbow", 9999), // B4EBE0 + 4 * 18   (vanilla = 15000)
                }
            };
        }
    }
}