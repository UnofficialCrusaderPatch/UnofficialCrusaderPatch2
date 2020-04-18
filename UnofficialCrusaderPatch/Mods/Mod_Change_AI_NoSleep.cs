namespace UCP.Patching
{
    /**
     * AI NO SLEEP
     */
    public class Mod_Change_AI_NoSleep : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            // 004CBCD5
            return new Change("ai_nosleep", ChangeType.AILords, false)
            {
                new DefaultHeader("ai_nosleep")
                {
                    new BinaryEdit("ai_nosleep")
                    {
                        new BinBytes(0x30, 0xD2, 0x90), // xor dl, dl
                        new BinSkip(2),
                        new BinBytes(0x30, 0xC9, 0x90), // xor cl, cl
                        new BinSkip(10),
                        new BinBytes(0x30, 0xD2, 0x90), // xor dl, dl
                        new BinSkip(8),
                        new BinBytes(0x30, 0xC9, 0x90), // xor cl, cl
                        new BinSkip(8),
                        new BinBytes(0x30, 0xD2, 0x90), // xor dl, dl
                        new BinSkip(10),
                        new BinBytes(0x30, 0xC9, 0x90), // xor cl, cl
                        new BinSkip(10),
                        new BinBytes(0x30, 0xD2, 0x90), // xor dl, dl
                        new BinSkip(10),
                        new BinBytes(0x30, 0xC9, 0x90), // xor cl, cl
                        new BinSkip(10),
                        new BinBytes(0x30, 0xD2, 0x90), // xor dl, dl
                        new BinSkip(10),
                        new BinBytes(0x30, 0xC9, 0x90), // xor cl, cl
                        new BinSkip(10),
                        new BinBytes(0x30, 0xD2, 0x90), // xor dl, dl
                    }
                }
            };
        }
    }
}