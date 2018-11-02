using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    // Wazir & Nizar Korn & Mehl kaufen lassen
    // Abreißen deaktivieren
    // ki rekrutierungen anheben
    // Scroll-Tempo in 1.41 reduzieren

    // Leiterträger, die deutlich mehr Pfeile und Bolzen aushalten
    // europäische Bogenschützen mit leicht erhöhter Reichweite.

    class Version
    {
        public static string PatcherVersion = "2.01";

        // change version 0x424EF1 + 1
        public static readonly BinaryRedirect MenuChange = new BinaryRedirect("menuversion", false,
                                                           Encoding.UTF8.GetBytes("V1.%d UCP" + PatcherVersion + '\0'));

        public static IEnumerable<Change> Changes { get { return changes; } }
        static List<Change> changes = new List<Change>()
        {
            /* 
             * STAT CHANGES
             */

            // Armbrust dmg table: 0xB4ED20
            // Bogen dmg table: 0xB4EAA0
            // Lanzenträger hp: 10000
            
            // Armbrustschaden gegen Arab. Schwertkämpfer, original: 8000
            // 0xB4EE4C = 0x4B*4 + 0xB4ED20
            EditInt32.Create("c_arabxbow", ChangeType.Balancing, 3500),
            
            // Arab. Schwertkämpfer Angriffsanimation, ca. halbiert
            // 0xB59CD0
            EditBytes.Create("c_arabwall", ChangeType.Balancing,
                0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
                0x10, 0x11, 0x12, 0x13, 0x14, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x00),
            
            // Armbrustschaden gegen Lanzenträger, original: 15000
            // 0xB4ED80 = 0x18*4 + 0xB4ED20
            EditInt32.Create("c_spearxbow", ChangeType.Balancing, 9999),
            
            // Bogenschaden gegen Lanzenträger, original: 3500
            // 0xB4EB00 = 0x18*4 + 0xB4EAA0
            EditInt32.Create("c_spearbow", ChangeType.Balancing, 2000),

            

            /*
            * WEAPON & ARMOR AI BUYING - found from routine at 0x4CD62C
            */

            // ai1_buytable 0x01165C78

            // Marschall Waffen- & Rüstungskauf, original: 0
            // run time address: 0x23FF084 + 0x9C
            // edit at 0x4CA5AE
            BinaryHook.Create("c_marshalbuy", ChangeType.Bugfix, 6, 
                0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // mov [EAX+9C], 2
            
            // Friedrich Waffen- & Rüstungskauf, original: 0
            // run time address: 0x23FE0AC + 0x9C
            // edit at 0x004C8DEA
            BinaryHook.Create("c_frederickbuy", ChangeType.Bugfix, 6,
                0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x03, 0x00, 0x00, 0x00), // mov [EAX+9C], 3



            /*
            * FIXED AIV CASTLES - https://github.com/Evrey/SHC_AIV
            */

            new AIVChange("ui_evreyfixed", ChangeType.Bugfix, "EvreyFixed"),
            new AIVChange("ui_evreyimproved", ChangeType.Balancing, "EvreyImproved", false),
        };
    }
}
