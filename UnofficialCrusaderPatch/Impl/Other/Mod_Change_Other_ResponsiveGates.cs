using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * GATES ARE FASTER TO OPEN/CLOSE
     */
    public class Mod_Change_Other_ResponsiveGates : Mod
    {
        public Mod_Change_Other_ResponsiveGates() : base("o_responsivegates")
        {
            this.changeList = new List<string>
            {
                "o_responsivegates"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_responsivegates", ChangeType.Other)
            {
                new DefaultSubChange("o_responsivegates")
                {
                    // Gates closing distance to enemy = 200
                    // 0x422ACC + 2
                    BinInt32.CreateEdit("o_gatedistance", 140),

                    // Gates closing time after enemy leaves = 1200
                    // 0x422B35 + 7 (ushort)
                    BinShort.CreateEdit("o_gatetime", 100),
                }
            };
        }
    }
}