namespace UCP.Patching
{
    /**
     * IMPROVE WOOD BUYING
     */
    public class Mod_Fix_AI_BuyWood : Mod
    {
        override public Change getChange()
        {
            // 00457DF4
            return new Change("ai_buywood", ChangeType.Bugfix, true)
            {
                new DefaultHeader("ai_buywood")
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