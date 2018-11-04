using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    // Wazir & Nizar Korn & Mehl kaufen lassen
    // kalif eisen?
    // Abreißen deaktivieren
    // ki rekrutierungen anpassen
    // Scroll-Tempo in 1.41 reduzieren
    //

    // Leiterträger, die deutlich mehr Pfeile und Bolzen aushalten
    // europäische Bogenschützen mit leicht erhöhter Reichweite.

    class Version
    {
        public static string PatcherVersion = "2.02";

        // change version 0x424EF1 + 1
        public static readonly BinaryEdit MenuChange = BinRedirect.CreateEdit("menuversion", false,
                                                       Encoding.UTF8.GetBytes("V1.%d UCP" + PatcherVersion + '\0'));

        public static IEnumerable<Change> Changes { get { return changes; } }
        static List<Change> changes = new List<Change>()
        {
            /*
             *  EXTENDED GAME SPEED 
             */ 

            new BinaryChange("c_gamespeed", ChangeType.Bugfix)
            {
                // 4B4748
                new BinaryEdit("c_gamespeed_up")
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
                new BinaryEdit("c_gamespeed_down")
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
             * AI RECRUIT INTERVALS
             */

            // recruit interval: 023FC8E8 + AI_OFFSET * 4 + 164

            // start of game offsets?
            // rat offset: 0xA9  => 1
            // sultan offset: 0x49F  => 8
            // richard offset: 0x548  => 1
            // wolf offset: 0x2A4  => 4
            // frederick offset: 0x5F1  => 4

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

            // 115EEE0 + (AI1 = 73E8) = stay home troops?
            // +8 attack troops
            
            // absolute limit at 0x4CDEF8 + 1 = 200
            BinInt32.Change("ai_attacklimit", ChangeType.AILords, 1000),
            



            /* 
             * STAT CHANGES
             */

            // Armbrust dmg table: 0xB4ED20
            // Bogen dmg table: 0xB4EAA0
            // Lanzenträger hp: 10000
            
            // Armbrustschaden gegen Arab. Schwertkämpfer, original: 8000
            // 0xB4EE4C = 0x4B*4 + 0xB4ED20
            BinInt32.Change("c_arabxbow", ChangeType.Troops, 3500),
            
            // Arab. Schwertkämpfer Angriffsanimation, ca. halbiert
            // 0xB59CD0
            BinBytes.Change("c_arabwall", ChangeType.Troops,                
                0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
                0x10, 0x11, 0x12, 0x13, 0x14, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x00),

            // Armbrustschaden gegen Lanzenträger, original: 15000
            // 0xB4ED80 = 0x18*4 + 0xB4ED20
            BinInt32.Change("c_spearxbow", ChangeType.Troops, 9999),
            
            // Bogenschaden gegen Lanzenträger, original: 3500
            // 0xB4EB00 = 0x18*4 + 0xB4EAA0
            BinInt32.Change("c_spearbow", ChangeType.Troops, 2000),

            

            /*
            * WEAPON & ARMOR AI BUYING - found from routine at 0x4CD62C
            */

            // ai1_buytable 0x01165C78

            // Marschall Waffen- & Rüstungskauf, original: 0
            // run time address: 0x23FF084 + 0x9C
            // edit at 0x4CA5AE
            
            // Friedrich Waffen- & Rüstungskauf, original: 0
            // run time address: 0x23FE0AC + 0x9C
            // edit at 0x004C8DEA
            new BinaryChange("c_aibuy", ChangeType.Bugfix)
            {
                BinHook.CreateEdit("c_marshalbuy", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // mov [EAX+9C], 2
                BinHook.CreateEdit("c_frederickbuy", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // mov [EAX+9C], 2
            },




            /*
            * FIXED AIV CASTLES - https://github.com/Evrey/SHC_AIV
            */

            new AIVChange("ui_evreyfixed", ChangeType.Bugfix, "EvreyFixed"),
            new AIVChange("ui_evreyimproved", ChangeType.AILords, "EvreyImproved", false),
        };
    }
}
