using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    public sealed class Mod_Change_AI_AddAttack : Mod
    {

        public Mod_Change_AI_AddAttack() : base("ai_addattack")
        {
            this.changeList = new List<string>
            {
                "ai_addattack",
                "ai_addattack_alt"
            };
        }

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            Change change = new Change().withType(ChangeType.AILords).withIdentifier("ai_addattack");
            change.Add(
                // vanilla:
                // additional attack troops = factor * attack number

                new ValuedSubChange("ai_addattack")
                {
                    // 004CDEDC
                    new BinaryEdit("ai_addattack")
                    {
                        // if (ai gold < 10000)
                        new BinBytes(0x7E, 07), // jle to 8

                        new BinBytes(0xB9), // mov ecx, value * 7/5 (vanilla = 7)
                        new BinInt32Value(7.0 / 5.0),

                        new BinBytes(0xEB, 0x05), // jmp

                        new BinBytes(0xB9), // mov ecx, value (vanilla = 5)
                        new BinInt32Value(1),

                        new BinBytes(0xF7, 0xE9), // imul ecx

                        new BinNops(6),
                        new BinBytes(0x01, 0x86), // mov [addtroops], eax instead of ecx
                    },
                }
            );

            change.Add(
                // alternative:
                // additional attack troops = factor * initial attack troops * attack number

                new ValuedSubChange("ai_addattack_alt")
                {
                    // 004CDE7C
                    new BinaryEdit("ai_addattack_alt")
                    {
                        new BinAddress("attacknum", 0x2F), // [0115F71C]
                        new BinAddress("addtroops", 0x55), // [0115F698]

                        new BinBytes(0x83, 0xE8, 0x01), // sub eax,01   => ai_index
                        new BinBytes(0x69, 0xC0, 0xA4, 0x02, 0x00, 0x00), // imul eax, 2A4 { 676 }  => ai_offset
                        new BinBytes(0x8B, 0x84, 0x28, 0xF4, 0x01, 0x00, 0x00), // mov eax,[eax+ebp+1F4]   => initial attack troops

                        new BinBytes(0x8B, 0x8E), //mov ecx,[esi+0115F71C]   => attack number
                        new BinRefTo("attacknum", false),

                        new BinBytes(0xF7, 0xE9), // imul ecx   => attack number * initial attack troops

                        new BinBytes(0x69, 0xC0), // imul eax, value
                        new BinInt32Value(10),

                        new BinBytes(0xB9, 0x0A, 0x00, 0x00, 0x00), // mov ecx, 0A { 10 }
                        new BinBytes(0xF7, 0xF9), // idiv ecx

                        new BinBytes(0x83, 0xC0, 0x05), // add eax, 5   => because in vanilla, attackNum was already 1 for first attack

                        new BinBytes(0x89, 0x86), // mov [esi+0115F698],eax   =>  addtroops = result
                        new BinRefTo("addtroops", false),

                        new BinBytes(0xFF, 0x86), // inc [esi+0115F71C]  => attack number++
                        new BinRefTo("attacknum", false),

                        new BinBytes(0xEB, 0x46), // jmp over nops
                        new BinNops(0x46),
                    },
                }
            );
            return change;
        }
    }
}