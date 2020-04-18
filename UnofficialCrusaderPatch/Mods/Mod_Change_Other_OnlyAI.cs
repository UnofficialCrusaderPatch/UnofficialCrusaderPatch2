namespace UCP.Patching
{
    /**
     * ONLY AI / SPECTATOR MODE
     */
    public class Mod_Change_Other_OnlyAI : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_onlyai", ChangeType.Other, false)
            {
                new DefaultHeader("o_onlyai")
                {   
                    // reset player list
                    // 0048F919
                    new BinaryEdit("o_onlyai_reset")
                    {
                        new BinAddress("selfindex", 2),
                        new BinAddress("selfai", 8),

                        new BinHook(12)
                        {
                            0x31, 0xC0, // xor eax, eax
                            0xA3, new BinRefTo("selfindex", false),

                            0x83, 0xE8, 0x01, // sub eax, 1
                            0xA3, new BinRefTo("selfai", false),
                        }
                    },

                    // game start
                    // 0048F96C => je to jmp to almost end
                    BinBytes.CreateEdit("o_onlyai", 0xE9, 0x09, 0x01, 0x00, 0x00, 0x90),
                    
                    // loading
                    // 004956FB
                    new BinaryEdit("o_onlyai_load1")
                    {
                        //  => mov [selfindex], eax   to   mov [selfindex], ebx = 0
                        new BinBytes(0x90, 0x90, 0x90, 0x89, 0x1D),
                        new BinSkip(5),
                        new BinBytes(0x3C) // mov ..., ebp  => mov ..., edi
                    },

                    // missing in 1.3
                    // after loading, hide buildings menu
                    // 0046B3FA => mov ecx, [selfindex]   to   xor ecx, ecx
                    //BinBytes.CreateEdit("o_onlyai_load2", 0x31, 0xC9, 0x90, 0x90, 0x90, 0x90),

                    // happy face :)
                    // 0x4334A6
                    BinBytes.CreateEdit("o_onlyai_face", 0xB9, 0xD3, 0x13, 0x00, 0x00, 0x90),

                    // show assassins
                    // 004EA265
                    BinBytes.CreateEdit("o_onlyai_assassins", 0xEB), // change je to jmp
                }
            };
        }
    }
}