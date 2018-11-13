using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    // kalif eisen?
    // Abreißen deaktivieren
    // Scroll-Tempo in 1.41 reduzieren

    // europäische Bogenschützen mit leicht erhöhter Reichweite.

    class Version
    {
        public static string PatcherVersion = "2.05";

        // change version 0x424EF1 + 1
        public static readonly BinaryEdit MenuChange = BinRedirect.CreateEdit("menuversion", false,
                                                       Encoding.UTF8.GetBytes("V1.%d UCP" + PatcherVersion + '\0'));

        public static IEnumerable<Change> Changes { get { return changes; } }
        static List<Change> changes = new List<Change>()
        {
            /*
             * PLAYER 1 COLOR
             */

            new Change("o_playercolor", ChangeType.Other)
            {
                // 0044FC15
                new BinaryEdit("o_playercolor_ingame")
                {
                    new BinAddress("var", 2), // [ED3158]

                    new BinBytes(0xA1), // mov eax, [var]
                    new BinRefTo("var", false),

                    new BinBytes(0x3C, 0x01), // cmp al, 1
                    new BinBytes(0x75, 0x04), // jne to next cmp
                    new BinBytes(0xB0, 0x04), // mov al, 4
                    new BinBytes(0xEB, 0x06), // jmp to end
                    new BinBytes(0x3C, 0x04), // cmp al, 4
                    new BinBytes(0x75, 0x02), // jne to end
                    new BinBytes(0xB0, 0x01), // mov al, 1
                    // end

                    new BinBytes(0x3C, 0x02), // cmp al, 1
                    new BinBytes(0x75, 0x04), // jne to next cmp
                    new BinBytes(0xB0, 0x03), // mov al, 4
                    new BinBytes(0xEB, 0x06), // jmp to end
                    new BinBytes(0x3C, 0x03), // cmp al, 4
                    new BinBytes(0x75, 0x02), // jne to end
                    new BinBytes(0xB0, 0x02), // mov al, 1
                    // end

                    new BinBytes(0xA3), // mov [var], eax
                    new BinRefTo("var", false),

                    new BinNops(2),
                }
            },

            /*
             * GATES
             */ 

            new Change("o_responsivegates", ChangeType.Other)
            {
                // Gates closing distance to enemy = 200
                // 0x422ACC + 2
                BinInt32.CreateEdit("o_gatedistance", 140),

                // Gates closing time after enemy leaves = 1200
                // 0x422B35 + 7 (ushort)
                BinShort.CreateEdit("o_gatetime", 100),
            },


            /*
             *  EXTENDED GAME SPEED 
             */ 

            new Change("o_gamespeed", ChangeType.Other)
            {
                // 4B4748
                new BinaryEdit("o_gamespeed_up")
                {
                    new BinBytes(0x3D, 0xE8, 0x03, 0x00, 0x00),      // cmp eax, 1000

                    new BinBytes(0x7D, 0x19), // jge to end

                    new BinBytes(0x8B, 0xF8),              // mov edi, eax

                    new BinBytes(0x3D, 0xC8, 0x00, 0x00, 0x00),     // cmp eax, 200
                    new BinHook("label", 0x0F, 0x8C)                // jl hook
                    {
                        0x83, 0xF8, 0x5A, // cmp eax, 90
                        0x7C, 0x03,       // jl to end
                        0x83, 0xC7, 0x05, // add edi, 5
                    },
                    new BinBytes(0x83, 0xC7, 0x5F),        // add edi, 95
                    new BinLabel("label"),
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
                    new BinHook("label", 0x0F, 0x8E)                // jle hook
                    {
                        0x83, 0xF8, 0x5A, // cmp eax, 90
                        0x7E, 0x03,       // jle to end
                        0x83, 0xEF, 0x05, // sub edi, 5
                    },
                    new BinBytes(0x83, 0xEF, 0x5F),        // sub edi, 95
                    new BinLabel("label"),
                    new BinBytes(0x83, 0xEF, 0x05),        // sub edi, 5
                    new BinBytes(0x90, 0x90),
                }
            },

            /*
             *  AI RECRUIT ADDITIONAL ATTACK TROOPS 
             */
             
            /*new SliderChange("ai_addattack", ChangeType.AILords, false, 0, 100, 1, 5)
            {
                // 004CDEDC
                new BinaryEdit("ai_addattack")
                {
                    // if (ai gold < 10000)
                    new BinBytes(0x7E, 07),      // jle to 8

                    new BinBytes(0xB9),     // mov ecx, value * 7/5 (vanilla = 7)
                    new BinProduct(7.0/5.0),     

                    new BinBytes(0xEB, 0x05), // jmp
                    
                    new BinBytes(0xB9),     // mov ecx, value (vanilla = 5)
                    new BinProduct(1),

                    new BinBytes(0xF7, 0xE9), // imul ecx
                    
                    new BinNops(6),
                    new BinBytes(0x01, 0x86), // mov [addtroops], eax instead of ecx
                },
            },*/

            /*
             * AI RECRUIT INTERVALS
             */

            // recruit interval: 023FC8E8 + AI_OFFSET * 4 + 164

            // start of game offsets?
            // rat offset: 0xA9  => 1, 1, 1
            // snake offset: 0x152 => 1, 0, 1
            // pig offset: 0x1FB => 1, 1, 4
            // wolf offset: 0x2A4 => 4, 1, 4
            // saladin offset: 0x34D => 1, 1, 1
            // kalif offset: 0x3F6 => 0, 1, 0
            // sultan offset: 0x49F  => 8, 8, 4
            // richard offset: 0x548  => 1, 1, 1
            // frederick offset: 0x5F1  => 4, 1, 4
            // philipp offset: 0x69A  => 4, 4, 4
            // wazir offset: 0x743  => 1, 1, 1
            // emir offset: 0x7EC  => 0, 1, 0
            // nizar offset: 0x895  => 4, 8, 1
            // sheriff offset: 0x93E  => 4, 1, 4
            // marshal offset: 0x9E7  => 1, 1, 4
            // abbot offset: 0xA90  => 1, 1, 1

            // +4, normal2
            // +8, turned up?

            // sets the recruitment interval to 1 for all AIs
            // 004D3B41 mov eax, 1
            BinBytes.Change("ai_recruitinterval", ChangeType.AILords, false, 0xB8, 0x01, 0, 0, 0, 0x90, 0x90),

            // disable sleeping phase for AI recruitment during attacks
            // this is no good, because the AI sends newly recruited troops instantly forth
            // while an attack is still going on, ending in streams of single soldiers
            // 004D3BF6 jne 2E, skips some comparisons
            //BinBytes.Change("ai_recruitsleep", ChangeType.Balancing, false, 0x75, 0x2E),


            /*
             * AI RECRUITMENT ATTACK LIMITS
             */ 

            // attack start troops: 023FC8E8 + AI_OFFSET * 4 + 1F4
            // rat => 20
            // snake => 30
            // pig => 10
            // wolf => 40
            // saladin => 50
            // kalif => 15
            // sultan => 10
            // richard => 20
            // frederick => 30
            // philipp => 10
            // wazir => 40
            // emir => 30
            // nizar => 40
            // sheriff => 50
            // marshal => 10
            // abbot => 50

            // 115EEE0 + (AI1 = 73E8) = stay home troops?
            // +8 attack troops
            
            // absolute limit at 0x4CDEF8 + 1 = 200
            BinInt32.Change("ai_attacklimit", ChangeType.AILords, 1000),


            /* 
             * STAT CHANGES
             */

            // Armbrust dmg table: 0xB4ED20
            // Bogen dmg table: 0xB4EAA0
            // Sling dmg table: 0xB4EBE0

            // Schutz von Leiternträgern gegen Fernkämpfer
            new Change("u_laddermen", ChangeType.Troops)
            {
                BinInt32.CreateEdit("u_ladderarmor_bow", 420), // B4EAA0 + 4 * 1D   (vanilla = 1000)
                BinInt32.CreateEdit("u_ladderarmor_sling", 1000), // B4EBE0 + 4 * 1D   (vanilla = 2500)
                BinInt32.CreateEdit("u_ladderarmor_xbow", 1000), // B4ED20 + 4 * 1D   (vanilla = 2500)
            },         
            
            // Armbrustschaden gegen Arab. Schwertkämpfer, original: 8000
            // 0xB4EE4C = 0x4B*4 + 0xB4ED20
            BinInt32.Change("u_arabxbow", ChangeType.Troops, 3500),
            
            // Arab. Schwertkämpfer Angriffsanimation, ca. halbiert
            // 0xB59CD0
            BinBytes.Change("u_arabwall", ChangeType.Troops,
                0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
                0x10, 0x11, 0x12, 0x13, 0x14, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x00),
            

            // Lanzenträger hp: 10000
            new Change("u_spearmen", ChangeType.Troops)
            {
                BinInt32.CreateEdit("u_spearbow", 2000), // B4EAA0 + 4 * 18   (vanilla = 3500)
                BinInt32.CreateEdit("u_spearxbow", 9999), // B4EBE0 + 4 * 18   (vanilla = 15000)
            },                  

            /*
             * AI BUY FOOD
             */

            // Wazir runtime buytable 023FE5F4 +84, apples, cheese, bread, wheat
            // Emir 023FE898
            // Nizar 023FEB3C
            

            /*
            * WEAPON & ARMOR AI BUYING - found from routine at 0x4CD62C
            */

            // ai1_buytable 0x01165C78
            new Change("ai_buy", ChangeType.Bugfix)
            {
                // mov [EAX+84], EDI = 10
                BinBytes.CreateEdit("ai_foodbuy_wazir", 0x89, 0xB8), // 004C951C
                
                // mov [EAX+9C], 2
                BinHook.CreateEdit("ai_wepbuy_marshal", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4CA5AE, runtime: 0x23FF084 + 0x9C
                BinHook.CreateEdit("ai_wepbuy_frederick", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4C8DEA, runtime: 0x23FE0AC + 0x9C
                BinHook.CreateEdit("ai_wepbuy_emir", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4C99AB, runtime: 023FE898 + 0x9C
                BinHook.CreateEdit("ai_wepbuy_abbot", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4CA95B, runtime: 023FF328 + 0x9C
            },




            /*
            * FIXED AIV CASTLES - https://github.com/Evrey/SHC_AIV
            */

            AIVEdit.Change("evreyfixed", ChangeType.Bugfix),
            AIVEdit.Change("evreyimproved", ChangeType.AILords, false),
        };
    }
}
