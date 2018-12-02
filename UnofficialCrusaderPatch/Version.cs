using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    // farben:
    // unit fade out
    // message shield

    // ai spielen & spectaten
    // wellen
    // holz kaufen?
    // rekrutierungsintervalle
    // bauerngrenze
    // KI Handeln untereinander
    // schlafen legen deaktivieren
    // trader post priority
    // mehr truppen für burggraben ausheben

    // Störkatapult positionierung
    // Schwein Nahrung verkaufen & Friedrich Waffen ?
    // kalif eisen?
    // Scroll-Tempo in 1.41 reduzieren

    // europäische Bogenschützen mit leicht erhöhter Reichweite.

    class Version
    {
        public static string PatcherVersion = "2.06";

        // change version 0x424EF1 + 1
        public static readonly ChangeHeader MenuChange = new ChangeHeader()
        {
            BinRedirect.CreateEdit("menuversion", false, Encoding.UTF8.GetBytes("V1.%d UCP" + PatcherVersion + '\0'))
        };


        public static IEnumerable<Change> Changes { get { return changes; } }
        static List<Change> changes = new List<Change>()
        {
            #region BUG FIXES

            /*
             *  OX TETHER SPAM
             */
             
            // 004EFF9A => jne to jmp
            BinBytes.Change("ai_tethers", ChangeType.Bugfix, true, 0x90, 0xE9),
            
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
                new DefaultHeader("ai_buy")
                {
                    // mov [EAX+84], EDI = 10
                    BinBytes.CreateEdit("ai_foodbuy_wazir", 0x89, 0xB8), // 004C951C
                
                    // mov [EAX+9C], 2
                    BinHook.CreateEdit("ai_wepbuy_marshal", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4CA5AE, runtime: 0x23FF084 + 0x9C
                    BinHook.CreateEdit("ai_wepbuy_frederick", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4C8DEA, runtime: 0x23FE0AC + 0x9C
                    BinHook.CreateEdit("ai_wepbuy_emir", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4C99AB, runtime: 023FE898 + 0x9C
                    BinHook.CreateEdit("ai_wepbuy_abbot", 6, 0xC7, 0x80, 0x9C, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00), // 0x4CA95B, runtime: 023FF328 + 0x9C
                }
            },


            /*
            * FIXED AIV CASTLES - https://github.com/Evrey/SHC_AIV
            */

            new Change("aiv_evrey", ChangeType.Bugfix)
            {
                AIVEdit.Header("evreyfixed", true),
                AIVEdit.Header("evreyimproved", false),
            },

            #endregion

            #region AI LORDS

            /*
             *  AI OVERCLOCK
             */

            // 0045CC20+6
            BinInt32.Change("ai_overclock", ChangeType.AILords, false, 100), // default: 200

            /*
             * IMPROVED ATTACKS
             */ 

            new Change("ai_attackwave", ChangeType.AILords)
            {
                new DefaultHeader("ai_attackwave")
                {
                    // 0x524633
                    new BinaryEdit("ai_attackwave")
                    {
                        new BinAddress("walls", 1, true),
                        new BinAddress("buildings", 0x1C7, true),
                        new BinAddress("towers", 0x20F, true),

                        new BinAlloc("var_type"),

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
                },
            },


            /*
             *  ECONOMY DEMOLISHING
             */
             
            // 004D0280
            new Change("ai_demolish", ChangeType.AILords, false, false)
            {
                new DefaultHeader("ai_demolish_walls", true)
                {
                    // 004D03F2  => jne to jmp
                    BinBytes.CreateEdit("ai_demolish_walls", 0x75, 0x6D, 0xE9, 0x00, 0x01, 0x00, 0x00, 0x90, 0x90)
                },

                new DefaultHeader("ai_demolish_eco", false)
                {
                    // 004D0387  => jne, jmp to end
                    BinBytes.CreateEdit("ai_demolish_eco", 0x75, 0x66, 0xE9, 0x6B, 0x01, 0x00, 0x00, 0x90, 0x90)
                },
            },





            /*
             *  AI RECRUIT ADDITIONAL ATTACK TROOPS 
             */

            // 115EEE0 + (AI1 = 73E8) = stay home troops?
            // +8 attack troops
            
            // absolute limit at 0x4CDEF8 + 1 = 200
            new Change("ai_attacklimit", ChangeType.AILords)
            {
                new SliderHeader("ai_attacklimit", true, 0, 3000, 50, 200, 500)
                {
                    new BinaryEdit("ai_attacklimit")
                    {
                        new BinInt32Value()
                    },
                }
            },

            new Change("ai_addattack", ChangeType.AILords, false)
            {
                // vanilla:
                // additional attack troops = factor * attack number

                new SliderHeader("ai_addattack", true, 0, 60, 1, 5, 12)
                {
                    // 004CDEDC
                    new BinaryEdit("ai_addattack")
                    {
                        // if (ai gold < 10000)
                        new BinBytes(0x7E, 07),      // jle to 8

                        new BinBytes(0xB9),     // mov ecx, value * 7/5 (vanilla = 7)
                        new BinInt32Value(7.0/5.0),

                        new BinBytes(0xEB, 0x05), // jmp
                    
                        new BinBytes(0xB9),     // mov ecx, value (vanilla = 5)
                        new BinInt32Value(1),

                        new BinBytes(0xF7, 0xE9), // imul ecx
                    
                        new BinNops(6),
                        new BinBytes(0x01, 0x86), // mov [addtroops], eax instead of ecx
                    },
                },



                // alternative:
                // additional attack troops = factor * initial attack troops * attack number

                new SliderHeader("ai_addattack_alt", false, 0.0, 3.0, 0.1, 0.0, 0.3)
                {
                    // 004CDE7C
                    new BinaryEdit("ai_addattack_alt")
                    {
                        new BinAddress("attacknum", 0x2F), // [0115F71C]
                        new BinAddress("addtroops", 0x55), // [0115F698]

                        new BinBytes(0x83, 0xE8, 0x01),                 // sub eax,01   => ai_index
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

                        new BinBytes(0xFF, 0x86),  // inc [esi+0115F71C]  => attack number++
                        new BinRefTo("attacknum", false),

                        new BinBytes(0xEB, 0x46), // jmp over nops
                        new BinNops(0x46),
                    },
                },
            },

            /*
             * AI RECRUIT INTERVALS
             */

            // AI_OFFSET = AI_INDEX * 169

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

            #endregion

            #region UNITS
            
            // Armbrust dmg table: 0xB4ED20
            // Bogen dmg table: 0xB4EAA0
            // Sling dmg table: 0xB4EBE0

            // Schutz von Leiternträgern gegen Fernkämpfer
            new Change("u_laddermen", ChangeType.Troops)
            {
                new DefaultHeader("u_laddermen")
                {
                    BinInt32.CreateEdit("u_ladderarmor_bow", 420), // B4EAA0 + 4 * 1D   (vanilla = 1000)
                    BinInt32.CreateEdit("u_ladderarmor_sling", 1000), // B4EBE0 + 4 * 1D   (vanilla = 2500)
                    BinInt32.CreateEdit("u_ladderarmor_xbow", 1000), // B4ED20 + 4 * 1D   (vanilla = 2500)

                    // 0052EC37 + 2
                    BinBytes.CreateEdit("u_laddergold", 9), // 1D - 9 = 14h            (vanilla: 1D - 19 = 4)
                }
            },         
            
            // Armbrustschaden gegen Arab. Schwertkämpfer, original: 8000
            // 0xB4EE4C = 0x4B*4 + 0xB4ED20
            BinInt32.Change("u_arabxbow", ChangeType.Troops, 3500),
            
            // Arab. Schwertkämpfer Angriffsanimation, ca. halbiert
            // 0xB59CD0
            BinBytes.Change("u_arabwall", ChangeType.Troops, true,
                0x01, 0x02, 0x03, 0x04, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E,
                0x10, 0x11, 0x12, 0x13, 0x14, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x00),
            

            // Lanzenträger hp: 10000
            new Change("u_spearmen", ChangeType.Troops)
            {
                new DefaultHeader("u_spearmen")
                {
                    BinInt32.CreateEdit("u_spearbow", 2000), // B4EAA0 + 4 * 18   (vanilla = 3500)
                    BinInt32.CreateEdit("u_spearxbow", 9999), // B4EBE0 + 4 * 18   (vanilla = 15000)
                }
            },                

            #endregion

            #region OTHER

            /*
             * PLAYER 1 COLOR
             */

            new Change("o_playercolor", ChangeType.Other, false)
            {
                new ColorHeader("o_playercolor")
                {
                    #region Round Table

                    // 004AF3D0
                    BinHook.CreateEdit("o_playercolor_table_drag", 6,
                        0x8B, 0xC5, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x8D, 0x80, 0x22, 0x02, 0x00, 0x00 //  lea eax, [EAX + 222]
                    ),   
                    
                    // 004AEFE9
                    BinHook.CreateEdit("o_playercolor_table_back", 6,
                        0x89, 0xF0, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x8D, 0x90, 0x22, 0x02, 0x00, 0x00 //  lea edx, [EAX + 222]
                    ),   

                    // 004AF15A
                    BinHook.CreateEdit("o_playercolor_table1", 7, 
                            0x89, 0xF0, // mov eax, esi
                            0x3C, 0x01, //  CMP AL,1
                            0x75, 0x04, //  JNE SHORT
                            0xB0, new BinByteValue(), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x01, //  MOV AL,1
                            0x8B, 0x14, 0x85, // mov edx, [eax*4 + namecolors]
                            new BinRefTo("namecolors", false)
                    ),

                    // 004AF1A9
                    BinHook.CreateEdit("o_playercolor_table2", 7,
                            0x89, 0xF0, // mov eax, esi
                            0x3C, 0x01, //  CMP AL,1
                            0x75, 0x04, //  JNE SHORT
                            0xB0, new BinByteValue(), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x01, //  MOV AL,1
                            0x8B, 0x04, 0x85, // mov eax, [eax*4 + namecolors]
                            new BinRefTo("namecolors", false)
                    ),

                    #endregion

                    #region scoreboards

                    // 004D60B1 end results scoreboard
                    BinHook.CreateEdit("o_playercolor_endscore", 7,
                            0x89, 0xF0, // mov eax, esi
                            0x3C, 0x01, //  CMP AL,1
                            0x75, 0x04, //  JNE SHORT 
                            0xB0, new BinByteValue(), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x01, //  MOV AL,1
                            0x8B, 0x04, 0x85, // mov eax, [eax*4 + namecolors]
                            new BinRefTo("namecolors", false)
                    ),


                    // 004AFCBD game over
                    new BinaryEdit("o_playercolor_gameover")
                    {
                        new BinAddress("someoffset", 5),

                        BinHook.CreateJMP(9,
                            0x8B, 0xC7, // mov eax, edi
                            0x2D, new BinRefTo("namecolors", false), // sub eax, namecolors
                            0x3C, 0x04, //cmp al, value
                            0x75, 0x04, //  JNE SHORT 
                            0xB0, new BinByteValue(4), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(4), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x04, //  MOV AL,1
                            0x8B, 0x80, // mov eax, [eax + namecolors]
                            new BinRefTo("namecolors", false),
                            0x8B, 0x0C, 0x85, // mov ecx, [eax*4 + someoffset]
                            new BinRefTo("someoffset", false)
                        ),

                        new BinSkip(0x1B),

                        BinHook.CreateJMP(9,
                            0x8B, 0xC7, // mov eax, edi
                            0x2D, new BinRefTo("namecolors", false), // sub eax, namecolors
                            0x3C, 0x04, //cmp al, value
                            0x75, 0x04, //  JNE SHORT 
                            0xB0, new BinByteValue(4), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(4), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x04, //  MOV AL,1
                            0x8B, 0x80, // mov eax, [eax + namecolors]
                            new BinRefTo("namecolors", false),
                            0x8B, 0x0C, 0x85, // mov ecx, [eax*4 + someoffset]
                            new BinRefTo("someoffset", false)
                        ),

                        new BinSkip(0x16),

                        BinHook.CreateJMP(9,
                            0x8B, 0xC7, // mov eax, edi
                            0x2D, new BinRefTo("namecolors", false), // sub eax, namecolors
                            0x3C, 0x04, //cmp al, value
                            0x75, 0x04, //  JNE SHORT 
                            0xB0, new BinByteValue(4), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(4), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x04, //  MOV AL,1
                            0x8B, 0x80, // mov eax, [eax + namecolors]
                            new BinRefTo("namecolors", false),
                            0x8B, 0x14, 0x85, // mov edx, [eax*4 + someoffset]
                            new BinRefTo("someoffset", false)
                        ),

                        new BinSkip(0x20),

                        BinHook.CreateJMP(9,
                            0x8B, 0xC7, // mov eax, edi
                            0x2D, new BinRefTo("namecolors", false), // sub eax, namecolors
                            0x3C, 0x04, //cmp al, value
                            0x75, 0x04, //  JNE SHORT 
                            0xB0, new BinByteValue(4), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(4), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x04, //  MOV AL,1
                            0x8B, 0x80, // mov eax, [eax + namecolors]
                            new BinRefTo("namecolors", false),
                            0x8B, 0x14, 0x85, // mov edx, [eax*4 + someoffset]
                            new BinRefTo("someoffset", false)
                        ),
                    },
                
                    // 004AE562 mightiest lord
                    BinHook.CreateEdit("o_playercolor_scorename", 7,
                            0x89, 0xF0, // mov eax, esi
                            0x3C, 0x01, //  CMP AL,1
                            0x75, 0x04, //  JNE SHORT 
                            0xB0, new BinByteValue(), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x01, //  MOV AL,1
                            0x8B, 0x04, 0x85, // mov eax, [eax*4 + varscore]
                            new BinRefTo("namecolors", false)
                    ),

                    #endregion

                    #region Chat

                    // 0047FA16
                    BinHook.CreateEdit("o_playercolor_chat", 7,
                        0x80, 0xF9, 0x01, //  CMP CL,1
                        0x75, 0x04, //  JNE SHORT 2. CMP
                        0xB1, new BinByteValue(), //  MOV CL, value
                        0xEB, 0x07, //  JMP SHORT END
                        0x80, 0xF9, new BinByteValue(), //  CMP CL, value
                        0x75, 0x02, //  JNE SHORT END
                        0xB1, 0x01, //  MOV CL,1
                        0x8B, 0x14, 0x8D, // mov edx, [ecx*4 + varscore]
                        new BinRefTo("namecolors", false)
                    ),

                    // 0047FAEE
                    BinHook.CreateEdit("o_playercolor_chat2", 7,
                        0x80, 0xF9, 0x01, //  CMP CL,1
                        0x75, 0x04, //  JNE SHORT 2. CMP
                        0xB1, new BinByteValue(), //  MOV CL, value
                        0xEB, 0x07, //  JMP SHORT END
                        0x80, 0xF9, new BinByteValue(), //  CMP CL, value
                        0x75, 0x02, //  JNE SHORT END
                        0xB1, 0x01, //  MOV CL,1
                        0x8B, 0x14, 0x8D, // mov edx, [ecx*4 + varscore]
                        new BinRefTo("namecolors", false)
                    ),

                    #endregion

                    #region Trail

                    // 004D8B05
                    BinHook.CreateEdit("o_playercolor_trail_portrait", 6,
                        0x8B, 0xC3, // mov eax, ebx
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x05, 0x22, 0x02, 0x00, 0x00 //  ADD EAX,222
                    ),    
                    
                    // 004DE2C9
                    BinHook.CreateEdit("o_playercolor_trail_shield2", 7,

                        0x8B, 0xC6, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x04, //  JNE SHORT
                        0xB0, new BinByteValue(), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT
                        0x3C, new BinByteValue(), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT
                        0xB0, 0x01, //  MOV AL,1
                        
                        0x05, 0xD5, 0x01, 0x00, 0x00, // add eax, 1D5
                        0x50 // push eax
                    ),   

                    // 004DDA5F
                    BinHook.CreateEdit("o_playercolor_trail_shield", 6,
                        0x80, 0xFA, 0x00, //  CMP DL,0
                        0x75, 0x04, //  JNE SHORT 2. CMP
                        0xB2, new BinByteValue(offset:-1), //  MOV DL, value
                        0xEB, 0x07, //  JMP SHORT END
                        0x80, 0xFA, new BinByteValue(offset:-1), //  CMP DL, value
                        0x75, 0x02, //  JNE SHORT END
                        0xB2, 0x00, //  MOV DL,0
                        0x81, 0xC2, 0xD6, 0x01, 0x00, 0x00 // ori code,  ADD EDX,1D6
                    ),   


                    // 4DE26D
                    new BinaryEdit("o_playercolor_trail_name")
                    {
                        new BinAddress("namecolors", 3),

                        new BinHook(7, "namelabel", 0xE9)
                        {
                            0x89, 0xF0, // mov eax, esi
                            0x3C, 0x01, //  CMP AL,1
                            0x75, 0x04, //  JNE SHORT
                            0xB0, new BinByteValue(), //  MOV AL, value
                            0xEB, 0x06, //  JMP SHORT
                            0x3C, new BinByteValue(), //  CMP AL, value
                            0x75, 0x02, //  JNE SHORT
                            0xB0, 0x01, //  MOV AL,1
                            0x8B, 0x0C, 0x85, // mov ecx, [esi*4 + varscore]
                            new BinRefTo("namecolors", false)
                        },
                        new BinLabel("namelabel"),
                    },

                    #endregion

                    #region Lineup menu

                    // 00448C78
                    BinHook.CreateEdit("o_playercolor_minilist1", 5,
                        0x57, 0x56, // push edi, esi

                        0x8D, 0x85, 0x31, 0xFD, 0xFF, 0xFF, // lea eax, [ebp - 2CF]
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x04, //  JNE SHORT
                        0xB0, new BinByteValue(offset:-1), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT
                        0x3C, new BinByteValue(offset:-1), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT
                        0xB0, 0x00, //  MOV AL,0
                        
                        0x05, 0xCF, 0x02, 0x00, 0x00, // add eax, 2CF
                        0x50, // push eax
                        0x6A, 0x2E // push 2E
                    ),   

                    // 00448CC3
                    BinHook.CreateEdit("o_playercolor_minilist2", 5,
                        0x57, 0x56, // push edi, esi

                        0x8D, 0x85, 0x31, 0xFD, 0xFF, 0xFF, // lea eax, [ebp - 2CF]
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x04, //  JNE SHORT
                        0xB0, new BinByteValue(offset:-1), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT
                        0x3C, new BinByteValue(offset:-1), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT
                        0xB0, 0x00, //  MOV AL,0
                        
                        0x05, 0xCF, 0x02, 0x00, 0x00, // add eax, 2CF
                        0x50, // push eax
                        0x6A, 0x2E // push 2E
                    ),   


                    // 00428421
                    BinHook.CreateEdit("o_playercolor_mm_shield_hover", 6,
                        0x80, 0xFA, 0x00, //  CMP DL,0
                        0x75, 0x04, //  JNE SHORT 2. CMP
                        0xB2, new BinByteValue(offset:-1), //  MOV DL, value
                        0xEB, 0x07, //  JMP SHORT END
                        0x80, 0xFA, new BinByteValue(offset:-1), //  CMP DL, value
                        0x75, 0x02, //  JNE SHORT END
                        0xB2, 0x00, //  MOV DL,0
                        0x81, 0xC2, 0xD4, 0x00, 0x00, 0x00 // ori code,  ADD EDX,D4
                    ),   

                    // 00428360
                    BinHook.CreateEdit("o_playercolor_mm_shield_drag", 5,
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(offset:-1), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(offset:-1), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x00, //  MOV AL,0
                        0x05, 0xD6, 0x01, 0x00, 0x00 // ori code,  ADD EAX,1D6
                    ),   

                    // 0042845B
                    BinHook.CreateEdit("o_playercolor_mm_shields", 6,
                        0x80, 0xF9, 0x00, //  CMP CL,0
                        0x75, 0x04, //  JNE SHORT 2. CMP
                        0xB1, new BinByteValue(offset:-1), //  MOV CL, value
                        0xEB, 0x07, //  JMP SHORT END
                        0x80, 0xF9, new BinByteValue(offset:-1), //  CMP CL, value
                        0x75, 0x02, //  JNE SHORT END
                        0xB1, 0x00, //  MOV CL,0

                        0x81, 0xC1, 0xD6, 0x01, 0x00, 0x00  // ori code,     add ecx, 1D6
                    ),   

                    // 004283C1
                    BinHook.CreateEdit("o_playercolor_mm_emblem1", 5,
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(offset:-1), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(offset:-1), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x00, //  MOV AL,0
                        0x05, 0xCF, 0x02, 0x00, 0x00 // ori code,  ADD EAX,2CF
                    ),    

                    // 004282DD
                    BinHook.CreateEdit("o_playercolor_mm_emblem2", 5,
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(offset:-1), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(offset:-1), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x00, //  MOV AL,0
                        0x05, 0xCF, 0x02, 0x00, 0x00 // ori code,  ADD EAX,2CF
                    ),

                    #endregion

                    #region ingame

                    // 004B6CC3
                    BinHook.CreateEdit("o_playercolor_minimap", 6,
                        0x80, 0xF9, 0x01, //  CMP CL,1
                        0x75, 0x04, //  JNE SHORT 2. CMP
                        0xB1, new BinByteValue(), //  MOV CL, value
                        0xEB, 0x07, //  JMP SHORT END
                        0x80, 0xF9, new BinByteValue(), //  CMP CL, value
                        0x75, 0x02, //  JNE SHORT END
                        0xB1, 0x01, //  MOV CL,1

                        0x83, 0xC2, 0xD9, 0x83, 0xFA, 0x26 // ori code
                    ),   

                    // 004B05CC
                    BinHook.CreateEdit("o_playercolor_emblem1", 5,
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x05, 0x22, 0x02, 0x00, 0x00 //  ADD EAX,222
                    ),     
                
                    // 004B06EB
                    BinHook.CreateEdit("o_playercolor_emblem2", 5,
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x05, 0x22, 0x02, 0x00, 0x00 //  ADD EAX,222
                    ),


                    // 00427CC2
                    BinHook.CreateEdit("o_playercolor_list", 6,
                        0x89, 0xF0, //  MOV EAX,ESI
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x04, //  JNE SHORT 00427CD2
                        0xB0, new BinByteValue(), //  MOV AL, value
                        0xEB, 0x06, //  JMP SHORT 00427CD8
                        0x3C, new BinByteValue(), //  CMP AL, value
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x05, 0xD5, 0x01, 0x00, 0x00 //  ADD EAX,1D5
                    ),

                    // 0044FC15
                    new BinaryEdit("o_playercolor_ingame")
                    {
                        new BinAddress("var", 2), // [ED3158]

                        new BinBytes(0xA1), // mov eax, [var]
                        new BinRefTo("var", false),

                        new BinBytes(0x3C, 0x01), // cmp al, 1
                        new BinBytes(0x75, 0x04), // jne to next cmp
                        new BinBytes(0xB0), // mov al, value
                        new BinByteValue(),
                        new BinBytes(0xEB, 0x06), // jmp to end
                        new BinBytes(0x3C), // cmp al, value
                        new BinByteValue(),
                        new BinBytes(0x75, 0x02), // jne to end
                        new BinBytes(0xB0, 0x01), // mov al, 1
                        // end

                        new BinBytes(0x3C, 0x01), // cmp al, 1
                        new BinBytes(0x75, 0x04), // jne to next cmp
                        new BinBytes(0xB0, 0x04), // mov al, 4
                        new BinBytes(0xEB, 0x06), // jmp to end
                        new BinBytes(0x3C, 0x04), // cmp al, 4
                        new BinBytes(0x75, 0x02), // jne to end
                        new BinBytes(0xB0, 0x01), // mov al, 1
                        // end

                        new BinBytes(0xA3), // mov [var], eax
                        new BinRefTo("var", false),

                        new BinNops(2),
                    }

                    #endregion
                },
            },
            


            /*
             *  EXTENDED GAME SPEED 
             */ 

            new Change("o_gamespeed", ChangeType.Other)
            {
                new DefaultHeader("o_gamespeed")
                {
                    // 4B4748
                    new BinaryEdit("o_gamespeed_up")
                    {
                        new BinBytes(0x3D, 0xE8, 0x03, 0x00, 0x00),      // cmp eax, 1000

                        new BinBytes(0x7D, 0x19), // jge to end

                        new BinBytes(0x8B, 0xF8),              // mov edi, eax

                        new BinBytes(0x3D, 0xC8, 0x00, 0x00, 0x00),     // cmp eax, 200
                        new BinHook("label1", 0x0F, 0x8C)                // jl hook
                        {
                            0x83, 0xF8, 0x5A, // cmp eax, 90
                            0x7C, 0x03,       // jl to end
                            0x83, 0xC7, 0x05, // add edi, 5
                        },
                        new BinBytes(0x83, 0xC7, 0x5F),        // add edi, 95
                        new BinLabel("label1"),
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
                        new BinHook("label2", 0x0F, 0x8E)                // jle hook
                        {
                            0x83, 0xF8, 0x5A, // cmp eax, 90
                            0x7E, 0x03,       // jle to end
                            0x83, 0xEF, 0x05, // sub edi, 5
                        },
                        new BinBytes(0x83, 0xEF, 0x5F),        // sub edi, 95
                        new BinLabel("label2"),
                        new BinBytes(0x83, 0xEF, 0x05),        // sub edi, 5
                        new BinBytes(0x90, 0x90),
                    }
                }
            },



            /*
             * GATES
             */ 

            new Change("o_responsivegates", ChangeType.Other)
            {
                new DefaultHeader("o_responsivegates")
                {
                    // Gates closing distance to enemy = 200
                    // 0x422ACC + 2
                    BinInt32.CreateEdit("o_gatedistance", 140),

                    // Gates closing time after enemy leaves = 1200
                    // 0x422B35 + 7 (ushort)
                    BinShort.CreateEdit("o_gatetime", 100),
                }
            },



            /*
             * TOTAL TROOP LIMIT
             */

            new Change("o_trooplimit", ChangeType.Other)
            {
                // 00459E10 + 1
                new SliderHeader("o_trooplimit", true, 1000, 10000, 100, 2400, 5000)
                {
                    new BinaryEdit("o_trooplimit")
                    {
                        new BinInt32Value()
                    }
                }
            },



            /*
             * ONLY AI
             */

            // 0048F96C => je to jmp to almost end
            BinBytes.Change("o_onlyai", ChangeType.Other, false, 0xE9, 0x09, 0x01, 0x00, 0x00, 0x90),

            #endregion
        };
    }
}
