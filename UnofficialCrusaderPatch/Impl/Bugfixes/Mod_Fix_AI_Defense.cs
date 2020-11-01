using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * REMANNING WALL DEFENSES
     */
    public class Mod_Fix_AI_Defense : Mod
    {
        public Mod_Fix_AI_Defense() : base("ai_defense")
        {
            this.changeList = new List<string>
            {
                "ai_defense"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("ai_defense", ChangeType.Bugfix)
            {
                // Crusader does count defensive units on walls and patrols together
                // this prevents the AI from reinforcing missing troops on walls, if
                // there are still defensive patrols above a certain threshold.
                // 
                // Solution: Implement another counter only for defensive units on walls

                new DefaultSubChange("ai_defense")
                {
                    // 4D26AF
                    new BinaryEdit("ai_defense_group")
                    {
                        // offset for the group index of units
                        // 1 == defense, 4 == def patrols
                        new BinAddress("groupVar", 0x1B)
                    },

                    // 579879
                    new BinaryEdit("ai_defense_reset")
                    {
                        new BinAddress("somevar", 1),

                        new BinAlloc("defNum", 9*4),
                        new BinHook(5)
                        {
                            0x31, 0xC0,  // xor eax,eax

                            0x89, 0x14, 0x85, new BinRefTo("defNum", false),  // mov [eax*4 + defNum],edx
                            0x40, //inc eax
                            0x83, 0xF8, 0x08, //cmp eax,08
                            0x7E, 0xF3,  // jle beneath xor
                            
                            // ori code
                            0xB8, new BinRefTo("somevar", false),     // mov eax, somevar
                        }
                    },

                    // 579A7C
                    new BinaryEdit("ai_defense_count")
                    {
                        new BinHook(6)
                        {
                            // get unit's group index
                            0x8B, 0xCD,                             // mov ecx,ebp
                            0x69, 0xC9, 0x90, 0x04, 0x00, 0x00,     // imul ecx,ecx, 490 
                            0x0F, 0xB6, 0x89, new BinRefTo("groupVar", false),              // movzx ecx,byte ptr [ecx+01388976]

                            // check if it's a wall defense unit
                            0x83, 0xF9, 0x01,                       // cmp ecx,01 
                            0x75, 0x09,                             // jne to ori code

                            // increase wall defense count for this AI
                            0x8D, 0x0C, 0xBD, new BinRefTo("defNum", false), // lea ecx,[edi*4 + ai_defNum]
                            0xFF, 0x01,                             // inc [ecx]

                            // ori code
                            0x69, 0xFF, 0xF4, 0x39, 0x00, 0x00,     // imul edi, edi, 39F4
                        }
                    },

                    // 004D3E6F
                    new BinaryEdit("ai_defense_check")
                    {
                        new BinHook(6)
                        {
                            // AI index
                            0x8B, 0x54, 0x24, 0x04, // mov edx,[esp+04]
                            0x8B, 0x14, 0x95, new BinRefTo("defNum", false), // mov edx,[edx*4 + defNum]
                        }
                    },

                    // 004D3486
                    new BinaryEdit("ai_defense_affinity")
                    {
                        // increase the general affinity to reman defenses
                        // this value was zero for some AIs like the rat
                        new BinHook(7)
                        {
                            // ori code
                            0x8B, 0xAC, 0x87, 0x28, 0x01, 0x00, 0x00, // mov ebp,[edi+eax*4+00000128]

                            // add a constant value of 20; snake's default f.e. is 30
                            0x83, 0xC5, 0x14 // add ebp, 14h
                        }
                    }
                }
            };
        }
    }
}