using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UCP.AICharacters;

namespace UCP.Patching
{
    static class AICChange
    {
        public static void Activate(ChangeArgs args)
        {
            AICCollection coll;
            using (FileStream fs = new FileStream("vanilla.aic", FileMode.Open))
                coll = new AICCollection(fs);

            CreateEdit(coll).Activate(args);
        }

        static ChangeHeader CreateEdit(AICCollection coll)
        {
            byte[] data;
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                foreach (AICharacter aic in coll.Values)
                {
                    // mov eax, index
                    bw.Write((byte)0xB8);
                    bw.Write((int)aic.Index * 0x2A4);

                    // imul eax, 2A4
                    /*bw.Write((byte)0x69);
                    bw.Write((byte)0xC0);
                    bw.Write(0x2A4);*/

                    // add eax, esi
                    bw.Write((byte)0x01);
                    bw.Write((byte)0xF0);

                    // edit AI's properties
                    for (int i = 0; i < AIPersonality.TotalFields; i++)
                    {
                        // mov [eax + prop], value
                        bw.Write((byte)0xC7);
                        bw.Write((byte)0x80);
                        bw.Write((int)(i * 4));
                        bw.Write((int)aic.Personality.GetByIndex(i));
                    }
                }
                data = ms.ToArray();
            }

            // 004D1928
            BinaryEdit be = new BinaryEdit("ai_prop")
            {
                new BinAddress("call", 0x1B+1, true),

                new BinSkip(0x1B),
                new BinHook(5)
                {
                    // ori code
                    0xE8, new BinRefTo("call"),

                    // edit ais
                    new BinBytes(data),
                }
            };

            return new DefaultHeader("ai_prop") { be };
        }
    }
}
