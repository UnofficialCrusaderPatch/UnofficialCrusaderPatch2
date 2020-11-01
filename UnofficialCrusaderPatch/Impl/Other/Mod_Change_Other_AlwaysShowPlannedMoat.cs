using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * PLANNED MOATS ARE ALWAYS VISIBLE
     */
    public class Mod_Change_Other_AlwaysShowPlannedMoat : Mod
    {
        public Mod_Change_Other_AlwaysShowPlannedMoat() : base("o_moatvisibility")
        {
            this.changeList = new List<string>
            {
                "o_moatvisibility"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_moatvisibility", ChangeType.Other)
            {
                new DefaultSubChange("o_moatvisibility")
                {
                    // 4EC86C
                    new BinaryEdit("o_moatvisibility")
                    {
                        new BinSkip(0x24),
                        new BinBytes(0x15) // mov [ ], edx = 1 instead of ebp = 0
                    }
                }
            };
        }
    }
}