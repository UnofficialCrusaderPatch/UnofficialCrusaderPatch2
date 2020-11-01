using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * EXTENDED FIREPROOF DURATION FOR EXTENGUISHED BUILDINGS
     */
    public class Mod_Change_Other_FireCooldown : Mod
    {
        public Mod_Change_Other_FireCooldown() : base("o_firecooldown")
        {
            this.changeList = new List<string>
            {
                "o_firecooldown"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 0x00410A30 + 8 ushort default = 2000
            return new Change("o_firecooldown", ChangeType.Other)
            {
                new ValuedSubChange("o_firecooldown")
                {
                    new BinaryEdit("o_firecooldown")
                    {
                        new BinSkip(8),
                        new BinInt16Value()
                    },
                }
            };
        }
    }
}