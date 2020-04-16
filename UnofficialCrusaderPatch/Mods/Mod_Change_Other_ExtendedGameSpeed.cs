namespace UCP.Patching
{
    /**
     * EXTENDED MAXIMUM GAME SPEED
     */
    public class Mod_Change_Other_ExtendedGameSpeed : Mod
    {
        override public Change getChange()
        {
            return new Change("o_gamespeed", ChangeType.Other)
            {
                new DefaultHeader("o_gamespeed")
                {
                    // 4B4748
                    new BinaryEdit("o_gamespeed_up")
                    {
                        new BinBytes(0x3D, 0xE8, 0x03, 0x00, 0x00),      // cmp eax, 1000

                        new BinBytes(0x7D, 0x19), // jge to end

                        new BinBytes(0x8B, 0xF8),              // mov edi, eax

                        new BinBytes(0x3D, 0xC8, 0x00, 0x00, 0x00),     // cmp eax, 200
                        new BinHook("label1", 0x0F, 0x8C)                // jl hook
                        {
                            0x83, 0xF8, 0x5A, // cmp eax, 90
                            0x7C, 0x03,       // jl to end
                            0x83, 0xC7, 0x05, // add edi, 5
                        },
                        new BinBytes(0x83, 0xC7, 0x5F),        // add edi, 95
                        new BinLabel("label1"),
                        new BinBytes(0x83, 0xC7, 0x05),        // add edi, 5
                        new BinBytes(0xEB, 0x75),              // jmp to gamespeed_down set value
                        new BinBytes(0x90, 0x90, 0x90, 0x90),
                    },

                    // 004B47C2
                    new BinaryEdit("o_gamespeed_down")
                    {
                        new BinBytes(0x7E, 0x1B), // jle to end

                        new BinBytes(0x8B, 0xF8),              // mov edi, eax

                        new BinBytes(0x3D, 0xC8, 0x00, 0x00, 0x00),     // cmp eax, 200
                        new BinHook("label2", 0x0F, 0x8E)                // jle hook
                        {
                            0x83, 0xF8, 0x5A, // cmp eax, 90
                            0x7E, 0x03,       // jle to end
                            0x83, 0xEF, 0x05, // sub edi, 5
                        },
                        new BinBytes(0x83, 0xEF, 0x5F),        // sub edi, 95
                        new BinLabel("label2"),
                        new BinBytes(0x83, 0xEF, 0x05),        // sub edi, 5
                        new BinBytes(0x90, 0x90),
                    }
                }
            };
        }
    }
}