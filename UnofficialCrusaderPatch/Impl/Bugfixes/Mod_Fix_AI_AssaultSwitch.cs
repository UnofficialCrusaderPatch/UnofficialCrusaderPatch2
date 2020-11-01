using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * NO ASSAULT SWITCHES
     */
    public class Mod_Fix_AI_AssaultSwitch : Mod
    {
        public Mod_Fix_AI_AssaultSwitch() : base("ai_assaultswitch")
        {
            this.changeList = new List<string>
            {
                "ai_assaultswitch"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("ai_assaultswitch", ChangeType.Bugfix)
            {
                new DefaultSubChange("ai_assaultswitch")
                {
                    //4D3B41
                    new BinaryEdit("ai_recruitinterval")
                    {
                        // 4d3c1a
                        new BinAddress("order", 0xD9 + 2)
                    },

                    // 004D477B
                    new BinaryEdit("ai_assaultswitch")
                    {
                        new BinAddress("target", 0x15E),
                        new BinHook(8)
                        {
                            new BinBytes(0x83, 0xBB), // cmp [ebx+order], 3
                            new BinRefTo("order", false),
                            new BinBytes(0x03, 0x7C, 0x12, 0x39, 0xBB), // jl, cmp [ebx+target], edi
                            new BinRefTo("target", false),
                            new BinBytes(0x75, 0x0A),

                            new BinBytes(0x5F, 0x5E, 0x5D, 0x5B, 0x83, 0xC4, 0x20, 0xC2, 0x04, 0x00), // ret

                            new BinBytes(0x8B, 0x44, 0x24, 0x1C, 0x8B, 0x4C, 0x24, 0x20) // ori code
                        }
                    }
                }
            };
        }
    }
}