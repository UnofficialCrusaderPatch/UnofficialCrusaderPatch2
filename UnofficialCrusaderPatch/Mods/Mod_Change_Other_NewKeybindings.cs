using System.Text;

namespace UCP.Patching
{
    /**
     * ADDS NEW KEYBINDINGS
     */
    public class Mod_Change_Other_NewKeybindings : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_keys", ChangeType.Other, false)
            {
                new DefaultHeader("o_keys")
                {
                    // 495800
                    new BinaryEdit("o_keys_savefunc")
                    {
                        new BinAddress("self", 17),
                        new BinAddress("c1", 22),
                        new BinAddress("func", 27, true),
                        new BinAddress("savefunc", 50, true),

                        // 0x20 == save, 0x1F == load
                        new BinAlloc("DoSave", null)
                        {
                            0x8B, 0x44, 0x24, 0x04, // mov eax, [esp+4]
                            0xA3, new BinRefTo("c1", false), // mov [c1], eax
                            0xB9, new BinRefTo("self", false), // mov ecx, self
                            0x6A, 0x0E, // push E
                            0xE8, new BinRefTo("func"), // call func
                            0xE9, new BinRefTo("savefunc"), // jmp to save
                        }
                    },    

                    // 004697C0
                    new BinaryEdit("o_keys_savename")
                    {
                        new BinAlloc("namebool", 1),
                        new BinAlloc("name", Encoding.ASCII.GetBytes("Quicksave\0")),
                        new BinHook(9)
                        {
                            0x80, 0x3D, new BinRefTo("namebool", false), 0x00, // cmp byte ptr [namebool], 0
                            0x74, 0x06, // je to ori code
                            0xB8, new BinRefTo("name", false), // mov eax, quicksave
                            0xC3, // ret
                            // ori code:
                            new BinBytes(0x83, 0x79, 0x04, 0x00, 0x75, 0x03, 0x33, 0xC0, 0xC3)
                        }
                    },          

                    // 004B3B5C S key
                    new BinaryEdit("o_keys_s")
                    {
                        new BinAddress("ctrl", 0x106), // 4B3C60
                        
                        0x39, 0x1D, new BinRefTo("ctrl", false),  // cmp [ctrlpressed], ebx = 0
                        0x0F, 0x84, 0xFA, 0xF3, 0xFF, 0xFF,       // jmp to move if equal

                        0xC6, 0x05, new BinRefTo("namebool", false), 0x01,

                        0x6A, 0x20, // push 0x20
                        0xE8, new BinRefTo("DoSave"), // call save func
                        

                        0xC6, 0x05, new BinRefTo("namebool", false), 0x00,

                        0x58, // pop eax
                        0xEB, 0x53 // jmp to default/end 4B3BD3
                    },

                       

                    // 0046C2E0
                    new BinaryEdit("o_keys_loadname")
                    {
                        new BinAddress("someoffset", 25),
                        new BinHook(9)
                        {
                            0x80, 0x3D, new BinRefTo("namebool", false), 0x00, // cmp byte ptr [namebool], 0
                            0x74, 0x08, // je to ori code
                            0xB8, new BinRefTo("name", false), // mov eax, quicksave
                            0xC2, 0x04, 0x00, // ret

                            // ori code:
                            new BinBytes(0x8B, 0x44, 0x24, 0x04, 0x3D, 0xF4, 0x01, 0x00, 0x00)
                        }
                    },  
                    
                    // 004B3DAE L key
                    new BinaryEdit("o_keys_l")
                    {
                        new BinAddress("somevar", 0x02),
                        new BinAddress("default", 0x20, true),
                        new BinHook(6)
                        {
                            0x39, 0x1D, new BinRefTo("ctrl", false),  // cmp [ctrlpressed], ebx = 0
                            0x74, 0x1B, // je to ori code

                            0xC6, 0x05, new BinRefTo("namebool", false), 0x01,

                            0x6A, 0x1F, // push 0x1F
                            0xE8, new BinRefTo("DoSave"), // call save func
                            
                            0xC6, 0x05, new BinRefTo("namebool", false), 0x00,

                            0x58, // pop eax

                            0xE9, new BinRefTo("default"), // jump awayy
                            
                            // ori code
                            0x39, 0x1D, new BinRefTo("somevar", false)
                        }
                    },

                    // WASD
                    // Arrow Keys: 4b4ee4 + 1D => 9, A, B, C
                    // WASD Keys: 4b4ee4 + 39, 4F, 3C, 4B
                    new BinaryEdit("o_keys_down")
                    {
                        // 4b4ee4 + 39
                        new BinBytes(0x09),
                        new BinSkip(0x02),
                        new BinBytes(0x0B),
                        //new BinSkip(0x0E),
                        //new BinBytes(0x0C),
                        new BinSkip(0x03 + 0x0E + 1),
                        new BinBytes(0x0A),
                    },

                    // WASD
                    // 004B4C9F
                    new BinaryEdit("o_keys_up")
                    {
                        new BinHook(6, null, 0xE9)
                        {
                            0x83, 0xC0, 0xDB, // add eax, -25

                            // 1C left => 0
                            // 32 top => 1
                            // 1F right => 2
                            // 2E down => 3

                            0x83, 0xF8, 0x1C, // cmp eax, 1C
                            0x75, 0x04,       // jne to next
                            0x31, 0xC0,       // xor eax, eax
                            0xEB, 0x1C,       // jmp to end
                            
                            0x83, 0xF8, 0x32, // cmp eax, 32
                            0x75, 0x05,       // jne to next
                            0x8D, 0x40, 0xCF, // lea eax, [eax-31]
                            0xEB, 0x12,       // jmp to end
                            
                            0x83, 0xF8, 0x1F, // cmp eax, 1F
                            0x75, 0x05,       // jne to next
                            0x8D, 0x40, 0xE3, // lea eax, [eax-1D]
                            0xEB, 0x08,       // jmp to end

                            0x83, 0xF8, 0x2E, // cmp eax, 2E
                            0x75, 0x03,       // jne to end 
                            0x8D, 0x40, 0xD5, // lea eax, [eax-2B]

                            // end
                            0x83, 0xF8, 0x03 // cmp eax, 3
                        }
                    },

                    new BinaryEdit("o_keys_menu")
                    {
                        new BinAddress("callright", 6, true),
                        new BinAddress("callleft", 0x93, true),

                        new BinSkip(5),
                        new BinHook(5)
                        {
                            0x83, 0xFE, 0x44,
                            0x74, 0x05,
                            0xE8, new BinRefTo("callright")
                        },

                        new BinSkip(0x88),
                        new BinHook(5)
                        {
                            0x83, 0xFE, 0x41,
                            0x74, 0x05,
                            0xE8, new BinRefTo("callleft")
                        }
                    }
                }
            };
        }
    }
}