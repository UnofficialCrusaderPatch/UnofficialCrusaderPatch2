namespace UCP.Patching
{
    /**
     * IMPROVED ATTACKS
     */
    public class Mod_Change_AI_AttackWave : Mod
    {
        override public Change getChange()
        {
            return new Change("ai_attackwave", ChangeType.AILords)
            {
                new DefaultHeader("ai_attackwave")
                {
                    // 0x524633
                    new BinaryEdit("ai_attackwave")
                    {
                        new BinAddress("walls", 1, true),
                        new BinAddress("buildings", 0x1C7, true),
                        new BinAddress("towers", 0x20F, true),

                        new BinAlloc("var_type", 4),

                        new BinHook("back", 0xE9)
                        {
                            0x8B, 0x1D, // mov ebx,[var]
                            new BinRefTo("var_type", false),

                            0x83, 0xFB, 0x04,      // cmp ebx,04
                            0x7D, 0x07,            // jge to next cmp
                            0xE8,                  // call find wall
                            new BinRefTo("walls"),
                            0xEB, 0x11,            // jmp to inc

                            0x83, 0xFB, 0x06,      // cmp ebx,06
                            0x7D, 0x07,            // jge to last call
                            0xE8,                  // call find fortifications
                            new BinRefTo("towers"),
                            0xEB, 0x05,            // jmp to inc
                            
                            0xE8,                  // call find building
                            new BinRefTo("buildings"),

                            0x43,                  // inc ebx
                            0x83, 0xFB, 0x07,      // cmp ebx,7
                            0x7C, 0x02,            // jge to mov
                            0x31, 0xDB,            // xor ebx,ebx

                            0x89, 0x1D, // mov [var], ebx
                            new BinRefTo("var_type", false)
                        },
                        new BinLabel("back")
                    },

                    // 0051EF25
                    new BinaryEdit("ai_attackwave_wallcount")
                    {
                        new BinBytes(0xEB, 0x10), // skip check if wallpart is taken
                        new BinSkip(0x21),
                        new BinNops(0xD) // skip check if wallpart is broken
                    },

                    // 4D31CD
                    new BinaryEdit("ai_attackwave_lord")
                    {
                        new BinBytes(0x90, 0x90), // when a breach happens, send most troops to enemy lord
                    },
                },
            };
        }
    }
}