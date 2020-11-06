using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * IMPROVE WOOD BUYING
     */
    public class Mod_Fix_AI_BuyWood : Mod
    {
        public Mod_Fix_AI_BuyWood() : base("ai_buywood")
        {
            this.changeList = new List<string>
            {
                "ai_buywood"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 00457DF4
            return new Change("ai_buywood", ChangeType.Bugfix)
            {
                new DefaultSubChange("ai_buywood")
                {
                    new BinaryEdit("ai_buywood")
                    {
                        new BinAddress("offset", 2),
                        new BinHook(6)
                        {
                            0x83, 0xC3, 0x02, // add ebx, 2
                            0x3B, 0x9E, new BinRefTo("offset", false), // ori code, cmp ebx, [esi+offset]
                        }
                    }
                }
            };
        }
    }
}