using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * AI NO SLEEP
     */
    public class Mod_Change_AI_NoSleep : Mod
    {
        public Mod_Change_AI_NoSleep() : base("ai_nosleep")
        {
            this.changeList = new List<string>
            {
                "ai_nosleep"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 004CBCD5
            return new Change("ai_nosleep", ChangeType.AILords)
            {
                new DefaultSubChange("ai_nosleep")
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