namespace UCP.Patching
{
    /**
     * AI REBUILD STUFF
     */
    public class Mod_Fix_AI_Rebuild : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("ai_rebuild", ChangeType.Bugfix, true)
            {
                new DefaultHeader("ai_rebuild")
                {
                    // 004F94DA + 7
                    BinBytes.CreateEdit("ai_rebuildwalls", 0x01),

                    // 005161FB
                    new BinaryEdit("ai_rebuildtowers")
                    {
                        new BinAddress("end", 2, true),
                        new BinHook(6, "end", 0x0F, 0x85)
                        {
                            // quarry platform
                            0x66, 0x3D, 0x15, 0x00, // cmp ax, 15h
                            0x0F, 0x84, new BinRefTo("dem"),

                            // tower ruins
                            0x66, 0x3D, 0x56, 0x00, // cmp ax, 56h
                            0x0F, 0x84, new BinRefTo("dem"),
                            0x66, 0x3D, 0x57, 0x00, // cmp ax, 57h
                            0x0F, 0x84, new BinRefTo("dem"),
                            0x66, 0x3D, 0x58, 0x00, // cmp ax, 58h
                            0x0F, 0x84, new BinRefTo("dem"),
                            0x66, 0x3D, 0x59, 0x00, // cmp ax, 59h
                            0x0F, 0x84, new BinRefTo("dem"),
                            0x66, 0x3D, 0x4F, 0x00, // cmp ax, 4Fh
                            0x0F, 0x84, new BinRefTo("dem"),
                            
                            // engineer, tunnel & oil places
                            0x66, 0x3D, 0x35, 0x00, // cmp ax, 35h
                            0x0F, 0x84, new BinRefTo("dem"),
                            0x66, 0x3D, 0x3B, 0x00, // cmp ax, 3Bh
                            0x0F, 0x84, new BinRefTo("dem"),
                            0x66, 0x3D, 0x33, 0x00, // cmp ax, 33h
                            0x0F, 0x84, new BinRefTo("dem"),
                        },
                        new BinLabel("dem"),
                    }

                }
            };
        }
    }
}