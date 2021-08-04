using System.Collections.Generic;
using System.Text;
using System.Linq;
using UCP.AIV;
using UCP.Patching;
using UCP.Startup;
using static UCP.Patching.BinElements.Register;
using static UCP.Patching.BinElements.OpCodes;
using static UCP.Patching.BinElements.OpCodes.Condition.Values;
using System;

namespace UCP
{
    public class Version
    {
        public static string PatcherVersion = "2.15b";

        // change version 0x424EF1 + 1
        public static readonly ChangeHeader MenuChange = new ChangeHeader()
        {
            BinRedirect.CreateEdit("menuversion", false, Encoding.ASCII.GetBytes("V1.%d UCP" + PatcherVersion + '\0'))
        };
        public static readonly ChangeHeader MenuChange_XT = new ChangeHeader()
        {
            BinRedirect.CreateEdit("menuversion", false, Encoding.ASCII.GetBytes("V1.%d-E UCP" + PatcherVersion + '\0'))
        };

        static List<Change> changes = new List<Change>()
        {
            #region BUG FIXES
            
            // Fix ladder climbing behaviour
            new Change("o_fix_ladderclimb", ChangeType.Bugfix, true)
            {
                new DefaultHeader("o_fix_ladderclimb")
                {
                    
                    // 53D3D9
                    new BinaryEdit("o_fix_ladderclimb_pre")
                    {
                        // cache current unti moved
                        new BinAlloc("currentUnitMoved", 4),

                        new BinSkip(9),

                        new BinHook(6)
                        {
                            0x89, 0x15, new BinRefTo("currentUnitMoved", false), // mov [currentUnitMoved],edx
                            0x69, 0xD2, 0x90, 0x04, 0x00, 0x00, // imul edx,edx,00000490
                        }
                    },
                    
                    // 53D694
                    new BinaryEdit("o_fix_ladderclimb")
                    {
                        // need 120k bytes, because we need 3*4 bytes per unit, and the SHC-E max is 10k units
                        new BinAlloc("savedUnitDestinationForClimbing", 120000),

                        new BinSkip(12), // skip 12 bytes
                        
                        new BinHook(10)
                        {
                            0x50, // push eax
                            0xA1, new BinRefTo("currentUnitMoved", false), // mov eax,[currentUnitMoved]
                            0x83, 0xF8, 0x00, // cmp eax,00
                            0x74, 0x20, // je short 20
                            0x48, // dec eax
                            0x6B, 0xC0, 0x0C, // imul eax,eax,0C
                            0x89, 0xA8, new BinRefTo("savedUnitDestinationForClimbing", false), // mov [eax+savedUnitDestinationForClimbing],ebp
                            0x83, 0xC0, 0x04, // add eax,04
                            0x89, 0xB8, new BinRefTo("savedUnitDestinationForClimbing", false), // mov [eax+savedUnitDestinationForClimbing],edi
                            0x83, 0xC0, 0x04, // add eax,04
                            0xC7, 0x80, new BinRefTo("savedUnitDestinationForClimbing", false), 0x01, 0x00, 0x00, 0x00, // mov [eax+savedUnitDestinationForClimbing],01
                            0x58, // pop eax
                            0x66, 0x39, 0xD8, // cmp ax,bx
                            0x66, 0x89, 0x86, 0xBC, 0x08, 0x00, 0x00, // mov [esi+8BC],ax
                        }
                    },
                    
                    // 5790CB
                    new BinaryEdit("o_fix_ladderclimb_2")
                    {
                        new BinHook(16)
                        {
                            0x50, // push eax
                            0x8B, 0xC3, // mov eax,ebx
                            0x48, // dec eax
                            0x6B, 0xC0, 0x0C, // imul eax,eax,0C
                            0x83, 0xC0, 0x08, // add eax,08
                            0x83, 0xB8, new BinRefTo("savedUnitDestinationForClimbing", false), 0x00, // cmp dword ptr [eax+savedUnitDestinationForClimbing],00
                            0x75, 0x16, // jne short 0x16
                            0x58, // pop eax
                            0x66, 0x89, 0x8C, 0x3E, 0x00, 0x07, 0x00, 0x00, // mov [esi+edi+00000700],cx
                            0x66, 0x89, 0x94, 0x3E, 0x02, 0x07, 0x00, 0x00, // mov [esi+edi+00000702],dx
                            0xE9, new BinRefTo("exit"), // jmp exit
                            0xC7, 0x80, new BinRefTo("savedUnitDestinationForClimbing", false), 0x00, 0x00, 0x00, 0x00, // mov [eax+savedUnitDestinationForClimbing],00000000
                            0x58// pop eax
                        },
                        new BinLabel("exit")
                    },
                    
                    // 53D900
                    new BinaryEdit("o_fix_ladderclimb_3")
                    {
                        new BinHook(5)
                        {
                            0x50, // push eax
                            0x8B, 0xC3, // mov eax,ebx
                            0x48, // dec eax
                            0x6B, 0xC0, 0x0C, // imul eax,eax,0C
                            0x83, 0xC0, 0x08, // add eax,08,
                            0xC7, 0x80, new BinRefTo("savedUnitDestinationForClimbing", false), 0x01, 0x00, 0x00, 0x00, // mov [eax+savedUnitDestinationForClimbing],00000000
                            0x58, // pop eax
                            0x53, // push ebx
                            0x8B, 0x5C, 0x24, 0x08, // mov ebx,[esp+08]
                        }
                    },
                    
                    // 54C3E5
                    new BinaryEdit("o_fix_ladderclimb_4")
                    {
                        new BinHook(14)
                        {
                            0x57, // push edi
                            0x31, 0xFF, // xor edi,edi
                            0x66, 0x8B, 0x7C, 0x24, 0x18, // mov di,[esp+18]
                            0x4F, // dec edi
                            0x6B, 0xFF, 0x0C, // imul edi,edi,0C
                            0x8B, 0x87, new BinRefTo("savedUnitDestinationForClimbing", false), // mov eax,[edi+savedUnitDestinationForClimbing]
                            0x83, 0xC7, 0x04, // add edi,04
                            0x8B, 0x97, new BinRefTo("savedUnitDestinationForClimbing", false), // mov edx,[edi+savedUnitDestinationForClimbing]
                            0x89, 0x86, 0x00, 0x07, 0x00, 0x00, // mov [esi+00000700],eax
                            0x89, 0x96, 0x02, 0x07, 0x00, 0x00, // mov [esi+00000702],edx
                            0x0F, 0xBF, 0x96, 0x02, 0x07, 0x00, 0x00, // movsx edx,word ptr [esi+00000702]
                            0x0F, 0xBF, 0x86, 0x00, 0x07, 0x00, 0x00, // movsx edx,word ptr [esi+00000702]
                            0x5F, // pop edi
                        }
                    }
                }
            },

            /*
             * FIRE BALLISTAS ATTACK MONKS AND TUNNELERS
             */

            new Change("u_fireballistafix", ChangeType.Bugfix, true)
            {
                new DefaultHeader("u_fireballistafix")
                {
                    new BinaryEdit("u_fireballistatunneler")
                    {
                        new BinSkip(13),
                        new BinHook(6)
                        {
                            CMP(EAX, 5), //  cmp eax,05
                            PUSH(FLAGS), //  pushf
                            ADD(EAX, -0x16), //  add eax,-0x16
                            POP(FLAGS), //  popf
                            JMP(NOTEQUALS, 0x05), //  jne short 5
                            MOV(EAX, 5), //  mov eax,05
                            CMP(EAX, 0x37), //  cmp eax,37
                        },
                    },
                    new BinaryEdit("u_fireballistamonk")
                    {
                        new BinBytes(0x00)
                    }
                }
            },

            /*
             * DISABLE DEMOLISHING OF INACCESSIBLE BUILDINGS
             */

            // 004242C3
            BinBytes.Change("ai_access", ChangeType.Bugfix, true, 0xEB),
            
            /*
             * REMANNING WALL DEFENSES
             */

            new Change("ai_defense", ChangeType.Bugfix)
            {
                // Crusader does count defensive units on walls and patrols together
                // this prevents the AI from reinforcing missing troops on walls, if
                // there are still defensive patrols above a certain threshold.
                // 
                // Solution: Implement another counter only for defensive units on walls

                new DefaultHeader("ai_defense")
                {
                    // 4D26AF
                    new BinaryEdit("ai_defense_group")
                    {
                        // offset for the group index of units
                        // 1 == defense, 4 == def patrols
                        new BinAddress("groupVar", 0x1B)
                    },

                    // 579879
                    new BinaryEdit("ai_defense_reset")
                    {
                        new BinAddress("somevar", 1),

                        new BinAlloc("defNum", 9*4),
                        new BinHook(5)
                        {
                            XOR(EAX, EAX),  // xor eax,eax

                            0x89, 0x14, 0x85, new BinRefTo("defNum", false),  // mov [eax*4 + defNum],edx
                            0x40, //inc eax
                            CMP(EAX, 8), //cmp eax,08
                            0x7E, 0xF3,  // jle beneath xor
                            
                            // ori code
                            0xB8, new BinRefTo("somevar", false),     // mov eax, somevar
                        }
                    },

                    // 579A7C
                    new BinaryEdit("ai_defense_count")
                    {
                        new BinHook(6)
                        {
                            // get unit's group index
                            MOV(ECX, EBP),                             // mov ecx,ebp
                            0x69, 0xC9, 0x90, 0x04, 0x00, 0x00,     // imul ecx,ecx, 490 
                            0x0F, 0xB6, 0x89, new BinRefTo("groupVar", false),              // movzx ecx,byte ptr [ecx+01388976]

                            // check if it's a wall defense unit
                            CMP(ECX, 1),                       // cmp ecx,01 
                            JMP(NOTEQUALS, 0x09),                             // jne to ori code

                            // increase wall defense count for this AI
                            0x8D, 0x0C, 0xBD, new BinRefTo("defNum", false), // lea ecx,[edi*4 + ai_defNum]
                            0xFF, 0x01,                             // inc [ecx]

                            // ori code
                            0x69, 0xFF, 0xF4, 0x39, 0x00, 0x00,     // imul edi, edi, 39F4
                        }
                    },

                    // 004D3E6F
                    new BinaryEdit("ai_defense_check")
                    {
                        new BinHook(6)
                        {
                            // AI index
                            0x8B, 0x54, 0x24, 0x04, // mov edx,[esp+04]
                            0x8B, 0x14, 0x95, new BinRefTo("defNum", false), // mov edx,[edx*4 + defNum]
                        }
                    }
                }
            },

            /*
             *  OX TETHER SPAM
             */
             
            // 004EFF9A => jne to jmp
            BinBytes.Change("ai_tethers", ChangeType.Bugfix, true, 0x90, 0xE9),

            /*
             *  IMPROVE WOOD BUYING
             */ 

            // 00457DF4
            new Change("ai_buywood", ChangeType.Bugfix, true)
            {
                new DefaultHeader("ai_buywood")
                {
                    new BinaryEdit("ai_buywood")
                    {
                        new BinAddress("offset", 2),
                        new BinHook(6)
                        {
                            ADD(EBX, 0x02), // add ebx, 2
                            0x3B, 0x9E, new BinRefTo("offset", false), // ori code, cmp ebx, [esi+offset]
                        }
                    }
                }
            },

            /*
             * UNLIMITED SIEGE ENGINES ON TOWERS
             */
             
            // 004D20A2
            BinBytes.Change("ai_towerengines", ChangeType.Bugfix, true, 0xEB),
            
            /*
             *  NO ASSAULT SWITCHES
             */

            
            new Change("ai_assaultswitch", ChangeType.Bugfix)
            {
                new DefaultHeader("ai_assaultswitch")
                {
                    //4D3B41
                    new BinaryEdit("ai_recruitinterval")
                    { // 4d3c1a
                        new BinAddress("order", 0xD9 + 2)
                    },

                    // 004D477B
                    new BinaryEdit("ai_assaultswitch")
                    {
                        new BinAddress("target", 0x15E),
                        new BinHook(8)
                        {
                            new BinBytes(0x83, 0xBB), // cmp [ebx+order], 3
                            new BinRefTo("order",false),
                            new BinBytes(0x03, 0x7C, 0x12, 0x39, 0xBB), // jl, cmp [ebx+target], edi
                            new BinRefTo("target", false),
                            new BinBytes(0x75, 0x0A),

                            new BinBytes(0x5F, 0x5E, 0x5D, 0x5B, 0x83, 0xC4, 0x20, 0xC2, 0x04, 0x00), // ret

                            new BinBytes(0x8B, 0x44, 0x24, 0x1C, 0x8B, 0x4C, 0x24, 0x20) // ori code
                        }
                    }
                }
            },


            /*
             *  AI REBUILD STUFF
             */ 

            new Change("ai_rebuild", ChangeType.Bugfix, true)
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
            },
            
            // AI Fix laddermen with enclosed keep
            new Change("ai_fix_laddermen_with_enclosed_keep", ChangeType.Bugfix, true)
            {
                new DefaultHeader("ai_fix_laddermen_with_enclosed_keep")
                {

                    new BinaryEdit("ai_fix_laddermen_with_enclosed_keep") // 5774A
                    {
                        new BinBytes(0x6A, 0x01),
                    }

                }
            },

            new Change("u_fix_lord_animation_stuck_movement", ChangeType.Bugfix, true)
            {
                new DefaultHeader("u_fix_lord_animation_stuck_movement")
                {

                    new BinaryEdit("u_fix_lord_animation_stuck_movement") // 56E139
                    {
                        new BinAddress("originalCompareAddress", 11),
                        new BinAddress("unitHandle", 18),
                        new BinSkip(9),
                        new BinHook(17)
                        {
                            0x53, // push ebx
                            0xBB, new BinRefTo("unitHandle", false), // mov ebx,unitHandle
                            0xC7, 0x04, 0x33, 0x00, 0x00, 0x00, 0x00, // mov [ebx+esi],00000000
                            0x81, 0xC3, 0x04, 0x00, 0x00, 0x00, // add ebx,00000004
                            0x0F, 0xB7, 0x0C, 0x33, // movzx ecx,word ptr [ebx+esi]
                            
                            0x81, 0xEB, 0xA8, 0x02, 0x00, 0x00, // sub ebx,000002A8
                            0x81, 0xC1, 0x29, 0x00, 0x00, 0x00, // add ecx,00000029
                            0x81, 0x3C, 0x33, 0xCD, 0x00, 0x00, 0x00, // cmp [ebx+esi],000000CD
                            0x74, 0x06, // je short 0x06
                            0x81, 0xC1, 0x80, 0x00, 0x00, 0x00, // add ecx,00000080
                            
                            0x81, 0xC3, 0x24, 0x00, 0x00, 0x00, // add ebx,00000024
                            
                            0xC7, 0x04, 0x33, 0x00, 0x00, 0x00, 0x00, // mov [ebx+esi],00000000
                            0x81, 0xEB, 0x2C, 0x00, 0x00, 0x00, // sub ebx,0000002C
                            0x89, 0x0C, 0x33, // mov [ebx+esi],ecx
                            0x5B, // pop ebx
                            
                            // original compare
                            0x83, 0x3D, new BinRefTo("originalCompareAddress", false), 0x00, // cmp dword ptr [0191DD80],00
                        }
                    },

                    new BinaryEdit("u_fix_lord_animation_stuck_building_attack") // 56D856
                    {
                        new BinAddress("unitVar", 24),
                        new BinSkip(21),
                        new BinHook(7)
                        {
                            0xC7, 0x80, new BinRefTo("unitVar", false), 0x65, 0x00, 0x00, 0x00, // mov [eax+unitVar],00000065
                        }
                    }
                }
            },

            new Change("u_fix_applefarm_blocking", ChangeType.Bugfix, true)
            {
                new DefaultHeader("u_fix_applefarm_blocking")
                {

                    new BinaryEdit("u_fix_applefarm_blocking") // 4F36B2
                    {
                        new BinSkip(11),
                        new BinHook(5)
                        {
                            0x81, 0x47, 0x14, 0x02, 0x00, 0x00, 0x00, // add [edi+14],00000002
                            0x81, 0x47, 0x18, 0x02, 0x00, 0x00, 0x00, // add [edi+18],00000002
                            0x5F, // pop edi
                        }
                    }
                 }
            },
          
            // Fix tanner going back to her hut without cow
            // Block starts: 559C49
            new Change("u_tanner_fix", ChangeType.Bugfix, true)
            {
                // 559C7A
                new DefaultHeader("u_tanner_fix")
                {

                    new BinaryEdit("u_tanner_fix")
                    {
                        new BinAddress("unitBaseAddress", 52, false),
                        new BinSkip(37), // 49
                        new BinAddress("someCowData", 2, false),
                        new BinHook(6)
                        {
                            0x81, 0xBD, new BinRefTo("someCowData", false), 0x00, 0x00, 0x00, 0x00, // cmp [ebp+someCowData],00000000
                            0x75, 0x0E, // jne 0x0E
                            0x66, 0xC7, 0x86, new BinRefTo("unitBaseAddress", false), 0x01, 0x00, // mov word ptr [esi+unitBaseAddress],0001
                            0x5F, // pop edi
                            0x5E, // pop esi
                            0x5D, // pop ebp
                            0x5B, // pop ebx
                            0xC3, // ret
                            0x3B, 0x8D, new BinRefTo("someCowData", false) // cmp ecx,[ebp+someCowData]
                        },
                        new BinSkip(6),
                        new BinHook(8)
                        {
                            0x66, 0x83, 0xBD, new BinRefTo("unitBaseAddress", false), 0x00, // cmp word ptr [ebp+unitBaseAddress],00
                            0x74, 0x19, // je short 0x19
                            0x66, 0x81, 0xBE, new BinRefTo("unitBaseAddress", false), 0x02, 0x00, // cmp word ptr [esi+unitBaseAddress],0002
                            0x75, 0x0E, // jne short 0x0E
                            0x66, 0xC7, 0x86, new BinRefTo("unitBaseAddress", false), 0x01, 0x00, // mov word ptr [esi+unitBaseAddress],0001

                            0x5F, // pop edi
                            0x5E, // pop esi
                            0x5D, // pop ebp
                            0x5B, // pop ebx
                            0xC3, // ret
                        }
                    }
                }
            },
          
            /*
             * Fletcher bugfix 
             */
            new Change("o_fix_fletcher_bug", ChangeType.Bugfix)
            {
                new DefaultHeader("o_fix_fletcher_bug")
                {
                    new BinaryEdit("o_fix_fletcher_bug")
                    {
                        new BinSkip(0x1E), // skip 30 bytes
                        new BinBytes(0x01) // set state to 1 instead of 3

                    }
                }
            },
          
            // Fix AI crusader archers not lighting pitch
            new Change("ai_fix_crusader_archers_pitch", ChangeType.Bugfix, true)
            {
                new DefaultHeader("ai_fix_crusader_archers_pitch")
                {

                    new BinaryEdit("ai_fix_crusader_archers_pitch_fn")
                    {
                        new BinLabel("CheckFunction")
                    },

                    new BinaryEdit("ai_fix_crusader_archers_pitch_attr")
                    {
                        new BinAddress("UnitAttributeOffset",43)
                    },

                    new BinaryEdit("ai_fix_crusader_archers_pitch")
                    {
                        new BinAddress("CurrentTargetIndex",2),
                        new BinSkip(23),
                        new BinHook(7)
                        {
                            0x55, // push ebp
                            0x51, // push ecx
                            0xBE, 0x5B, 0x00, 0x00, 0x00, // mov esi,5B
                            0xE8, new BinRefTo("CheckFunction"),
                            0xA1, new BinRefTo("CurrentTargetIndex", false),
                            0x69, 0xC0, 0x90, 0x04, 0x00, 0x00, // imul eax,eax,490
                            0x0F, 0xB7, 0x80, new BinRefTo("UnitAttributeOffset", false),
                        }
                    }
                }
            },
          
            // Fix baker disappear bug
            new Change("o_fix_baker_disappear", ChangeType.Bugfix, true)
            {
                new DefaultHeader("o_fix_baker_disappear")
                {
                    new BinaryEdit("o_fix_baker_disappear") // 5774A
                    {
                        new BinSkip(19),
                        new BinNops(9)
                    }
                }
            },
          
            // Fix moat digging unit disappearing
            new Change("o_fix_moat_digging_unit_disappearing", ChangeType.Bugfix, true)
            {
                new DefaultHeader("o_fix_moat_digging_unit_disappearing")
                {

                    new BinaryEdit("o_fix_moat_digging_unit_disappearing")
                    {
                        new BinAddress("skip", 11),
                        new BinHook(9)
                        {
                            0x83, 0xBC, 0x30, 0xD4, 0x08, 0x00, 0x00, 0x7D, // cmp dword ptr [eax+esi+8D4],7D
                            0x74, 0x09, // je short 9
                            0x66, 0x83, 0xBC, 0x30, 0xA8, 0x06, 0x00, 0x00, 0x01
                        }
                    }
                }
            },
            #endregion

            #region AI LORDS

            new Change("ai_housing", ChangeType.AILords, false, false)
            {
                new SliderHeader("build_housing", true, 0, 100, 1, 0, 5)
                {
                    new BinaryEdit("ai_buildhousing")
                    {
                        new BinHook(5) // the first 5 bytes are an if condition that just checks if the first house has been built yet
                        {
                            0x81, 0xE9, new BinInt32Value(), // the value of the slider header gets put into the location of new BinInt32Value()
                        },
                        new BinSkip(6),
                        0x7F, 0xDE
                    },

                    new BinaryEdit("ai_deletehousing")
                    {
                        0x90, 0x90
                    }
                },

                new SliderHeader("campfire_housing", true, 0, 25, 1, 5, 10)
                {
                    new BinaryEdit("ai_buildhousing")
                    {
                        new BinSkip(11),
                        0x7D, 0x08,
                        new BinSkip(8), //skip everything until we come to the campfire logic comparison
                        0xB9, new BinInt32Value(), //replace the 5 with the user input value from the slider header
                        new BinSkip(8),
                        0x7C, 0xC7,
                        0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90
                    },

                    new BinaryEdit("ai_deletehousing")
                    {
                        0x90, 0x90
                    }
                },

                new DefaultHeader("delete_housing")
                {
                    new BinaryEdit("ai_deletehousing")
                    {
                        0x90, 0x90
                    }
                }
            },


            new Change("ai_recruitstate_initialtimer", ChangeType.AILords, false)
            {
                new SliderHeader("ai_recruitstate_initialtimervalue", true, 0, 30, 1, 6, 0)
                {
                    new BinaryEdit("ai_recruitstate_initialtimer")
                    {
                        new BinSkip(36),
                        new BinInt32Value(800)
                    }
                }
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

                        new BinAlloc("var_type", 4),

                        new BinHook("back", 0xE9)
                        {
                            0x8B, 0x1D, // mov ebx,[var]
                            new BinRefTo("var_type", false),

                            CMP(EBX, 4),      // cmp ebx,04
                            JMP(GREATERTHANEQUALS, 0x07),            // jge to next cmp
                            0xE8,                  // call find wall
                            new BinRefTo("walls"),
                            0xEB, 0x11,            // jmp to inc

                            CMP(EBX, 0x06),      // cmp ebx,06
                            JMP(GREATERTHANEQUALS, 0x07),            // jge to last call
                            0xE8,                  // call find fortifications
                            new BinRefTo("towers"),
                            JMP(UNCONDITIONAL, 0x05),            // jmp to inc
                            
                            0xE8,                  // call find building
                            new BinRefTo("buildings"),

                            0x43,                  // inc ebx
                            CMP(EBX, 0x07),      // cmp ebx,7
                            JMP(LESS, 0x02),            // jl to mov
                            XOR(EBX, EBX),            // xor ebx,ebx

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

                    // 4D31CD
                    new BinaryEdit("ai_attackwave_lord")
                    {
                        new BinBytes(0x90, 0x90), // when a breach happens, send most troops to enemy lord
                    },
                },
            },


            /*
             * ALWAYS ATTACK NEAREST NEIGHBOR
             */ 

            // 004D47B2
            new Change("ai_attacktarget", ChangeType.AILords, false)
            {
                new DefaultHeader("ai_attacktarget_nearest", true)
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x11, 0x90)
                },

                new DefaultHeader("ai_attacktarget_richest", false)
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x3F, 0x90)
                },

                new DefaultHeader("ai_attacktarget_weakest", false)
                {
                    BinBytes.CreateEdit("ai_attacktarget", 0xEB, 0x52, 0x90)
                },
            },

            /*
             * AI NO SLEEP
             */

            // 004CBCD5
            new Change("ai_nosleep", ChangeType.AILords, false)
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
            },

            /*
             *  ECONOMY DEMOLISHING
             */
             
            // 004D0280
            new Change("ai_demolish", ChangeType.AILords, false, false)
            {
                new DefaultHeader("ai_demolish_walls", true)
                {
                    // 004D03F4  => jmp to end
                    BinBytes.CreateEdit("ai_demolish_walls", 0xE9, 0x15, 0x01, 0x00, 0x00, 0x90, 0x90)
                },

                new DefaultHeader("ai_demolish_trapped", false)
                {
                    // 004F1988  => jne to jmp
                    BinBytes.CreateEdit("ai_demolish_trapped", 0xEB)
                },

                new DefaultHeader("ai_demolish_eco", false)
                {
                    // 004D0280  => retn 8
                    BinBytes.CreateEdit("ai_demolish_eco", 0xC2, 0x08, 0x00, 0x90, 0x90, 0x90)
                },
            },






            new Change("ai_addattack", ChangeType.AILords, false)
            {
                // vanilla:
                // additional attack troops = factor * attack number

                new SliderHeader("ai_addattack", true, 0, 250, 1, 5, 12)
                {
                    // 004CDEDC
                    new BinaryEdit("ai_addattack")
                    {
                        // if (ai gold < 10000)
                        JMP(LESSTHANEQUALS, 7),      // jle to 8

                        new BinBytes(0xB9),     // mov ecx, value * 7/5 (vanilla = 7)
                        new BinInt32Value(7.0/5.0),

                        JMP(UNCONDITIONAL, 0x05), // jmp
                    
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

                        SUB(EAX, 1),                 // sub eax,01   => ai_index
                        new BinBytes(0x69, 0xC0, 0xA4, 0x02, 0x00, 0x00), // imul eax, 2A4 { 676 }  => ai_offset
                        new BinBytes(0x8B, 0x84, 0x28, 0xF4, 0x01, 0x00, 0x00), // mov eax,[eax+ebp+1F4]   => initial attack troops

                        new BinBytes(0x8B, 0x8E), //mov ecx,[esi+0115F71C]   => attack number
                        new BinRefTo("attacknum", false),

                        new BinBytes(0xF7, 0xE9), // imul ecx   => attack number * initial attack troops

                        new BinBytes(0x69, 0xC0), // imul eax, value
                        new BinInt32Value(10),

                        new BinBytes(0xB9, 0x0A, 0x00, 0x00, 0x00), // mov ecx, 0A { 10 }
                        new BinBytes(0xF7, 0xF9), // idiv ecx
                    
                        ADD(EAX, 5), // add eax, 5   => because in vanilla, attackNum was already 1 for first attack

                        new BinBytes(0x89, 0x86), // mov [esi+0115F698],eax   =>  addtroops = result
                        new BinRefTo("addtroops", false),

                        new BinBytes(0xFF, 0x86),  // inc [esi+0115F71C]  => attack number++
                        new BinRefTo("attacknum", false),

                        JMP(UNCONDITIONAL, 0x46), // jmp over nops
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
            
            

            new Change("ai_restore_ai_siege_messages", ChangeType.AILords, false)
            {
                
                new DefaultHeader("ai_restore_ai_siege_messages_always", true)
                {
                    // 4D0FC0
                    new BinaryEdit("ai_restore_ai_siege_messages_data1")
                    {
                        new BinAddress("isPlayerAliveHandle", 7),
                        new BinAddress("IsPlayerAliveFunction", 12, true),
                        new BinAddress("playerDataArray", 30),
                        new BinAddress("playBikHandle", 42),
                        new BinAddress("QueueBikPlayFunction", 47, true),
                    },
                    
                    // 42793C
                    new BinaryEdit("ai_restore_ai_siege_messages_data2")
                    {
                        new BinAddress("playerGroupArray", 22),
                    },
                    
                    // 57BA78
                    new BinaryEdit("ai_restore_ai_siege_messages_data3")
                    {
                        new BinAddress("currentPlayerID", 1),
                        new BinAddress("AICArray", 8),
                    },
                    
                    // 4D4BD6
                    new BinaryEdit("ai_restore_ai_siege_messages")
                    {
                        new BinAlloc("aiToldToAttackBooleanArray", null)
                        {
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        },
                        new BinAlloc("PlaySiegeBikFromPlayer", null)
                        {
                            0x56, // push esi
                            0x8B, 0x74, 0x24, 0x08, // mov esi,[esp+08]
                            0x56, // push esi
                            0xB9, new BinRefTo("isPlayerAliveHandle", false), // mov ecx,isPlayerAliveHandle
                            0xE8, new BinRefTo("IsPlayerAliveFunction"), // call IsPlayerAliveFunction
                            0x85, 0xC0, // test eax,eax
                            0x53, // push ebx
                            0x74, 0x45, // je playSiegeBikFromPlayerSkip
                            
                            // Check if player is in the same group as the attacking AI
                            0xA1, new BinRefTo("currentPlayerID", false), // mov eax,currentPlayerID
                            0x0F, 0xB6, 0x98, new BinRefTo("playerGroupArray", false), // movzx ebx,byte ptr[eax+playerGroupArray]
                            0x38, 0x9E, new BinRefTo("playerGroupArray", false), // cmp [esi+playerGroupArray],bl
                            0x75, 0x31, // jne playSiegeBikFromPlayerSkip
                            
                            // Check if the AI just got the command to attack.
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0x83, 0x3C, 0x33, 0x00, // cmp dword ptr [ebx+esi],00
                            0x75, 0x25, // jne playSiegeBikFromPlayerSkip
                            
                            // We play the bik.
                            0x8B, 0xC6, // mov eax,esi
                            0x69, 0xC0, 0xF4, 0x39, 0x00, 0x00, // imul eax,eax,000039F4
                            0x8B, 0x88, new BinRefTo("playerDataArray", false), // mov ecx,[eax+playerDataArray]
                            0x68, 0x17, 0x00, 0x00, 0x00, // push 17
                            0x81, 0xE9, 0x01, 0x00, 0x00, 0x00, // sub ecx,01
                            0x51, // push ecx
                            0x56, // push esi
                            0xB9, new BinRefTo("playBikHandle", false), // mov ecx,playBikHandle
                            0xE8, new BinRefTo("QueueBikPlayFunction", true), // call QueueBikPlayFunction
                            
                            new BinLabel("playSiegeBikFromPlayerSkip"),
                            // reset aiToldToAttackBooleanArray for ai
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0xC7, 0x04, 0x33, 0x00, 0x00, 0x00, 0x00, // mov [ebx+esi],00000000
                            0x5B, // pop ebx
                            0x5E, // pop esi
                            0xC2, 0x04, 0x00, // ret 0004
                        },
                        new BinAddress("AIStateAddress", 13), // 2BA4
                        new BinAddress("OriginalFunctionAddress", 88, true),
                        new BinSkip(87),
                        new BinHook(5)
                        {
                            0xE8, new BinRefTo("OriginalFunctionAddress", true),
                            
                            // Now we can call PlaySiegeBikFromPlayer
                            0x56, // push esi
                            0xB9, new BinRefTo("AICArray", false), // mov ecx,AICArray
                            0xE8, new BinRefTo("PlaySiegeBikFromPlayer"), // call PlaySiegeBikFromPlayer
                        }
                    }
                },
                
                new DefaultHeader("ai_restore_ai_siege_messages_stronger", false)
                {
                    // 4D0FC0
                    new BinaryEdit("ai_restore_ai_siege_messages_data1")
                    {
                        new BinAddress("isPlayerAliveHandle", 7),
                        new BinAddress("IsPlayerAliveFunction", 12, true),
                        new BinAddress("playerDataArray", 30),
                        new BinAddress("playBikHandle", 42),
                        new BinAddress("QueueBikPlayFunction", 47, true),
                    },
                    
                    // 42793C
                    new BinaryEdit("ai_restore_ai_siege_messages_data2")
                    {
                        new BinAddress("playerGroupArray", 22),
                    },
                    
                    // 57BA78
                    new BinaryEdit("ai_restore_ai_siege_messages_data3")
                    {
                        new BinAddress("currentPlayerID", 1),
                        new BinAddress("AICArray", 8),
                    },
                    
                    // 4D4BD6
                    new BinaryEdit("ai_restore_ai_siege_messages")
                    {
                        new BinAlloc("aiToldToAttackBooleanArray", null)
                        {
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        },
                        new BinAlloc("PlaySiegeBikFromPlayer", null)
                        {
                            0x56, // push esi
                            0x8B, 0x74, 0x24, 0x08, // mov esi,[esp+08]
                            0x56, // push esi
                            0xB9, new BinRefTo("isPlayerAliveHandle", false), // mov ecx,isPlayerAliveHandle
                            0xE8, new BinRefTo("IsPlayerAliveFunction"), // call IsPlayerAliveFunction
                            0x85, 0xC0, // test eax,eax
                            0x53, // push ebx
                            0x74, 0x45, // je playSiegeBikFromPlayerSkip
                            
                            // Check if player is in the same group as the attacking AI
                            0xA1, new BinRefTo("currentPlayerID", false), // mov eax,currentPlayerID
                            0x0F, 0xB6, 0x98, new BinRefTo("playerGroupArray", false), // movzx ebx,byte ptr[eax+playerGroupArray]
                            0x38, 0x9E, new BinRefTo("playerGroupArray", false), // cmp [esi+playerGroupArray],bl
                            0x75, 0x31, // jne playSiegeBikFromPlayerSkip
                            
                            // Check if the AI just got the command to attack.
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0x83, 0x3C, 0x33, 0x00, // cmp dword ptr [ebx+esi],00
                            0x75, 0x25, // jne playSiegeBikFromPlayerSkip
                            
                            // We play the bik.
                            0x8B, 0xC6, // mov eax,esi
                            0x69, 0xC0, 0xF4, 0x39, 0x00, 0x00, // imul eax,eax,000039F4
                            0x8B, 0x88, new BinRefTo("playerDataArray", false), // mov ecx,[eax+playerDataArray]
                            0x68, 0x17, 0x00, 0x00, 0x00, // push 17
                            0x81, 0xE9, 0x01, 0x00, 0x00, 0x00, // sub ecx,01
                            0x51, // push ecx
                            0x56, // push esi
                            0xB9, new BinRefTo("playBikHandle", false), // mov ecx,playBikHandle
                            0xE8, new BinRefTo("QueueBikPlayFunction", true), // call QueueBikPlayFunction
                            
                            new BinLabel("playSiegeBikFromPlayerSkip"),
                            // reset aiToldToAttackBooleanArray for ai
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0xC7, 0x04, 0x33, 0x00, 0x00, 0x00, 0x00, // mov [ebx+esi],00000000
                            0x5B, // pop ebx
                            0x5E, // pop esi
                            0xC2, 0x04, 0x00, // ret 0004
                        },
                        new BinAddress("AIStateAddress", 13), // 2BA4
                        new BinAddress("OriginalFunctionAddress", 88, true),
                        new BinSkip(87),
                        new BinHook(5)
                        {
                            0xE8, new BinRefTo("OriginalFunctionAddress", true),
                            
                            0x50, // push eax
                            0x53, // push ebx
                            0x8B, 0xC6, // mov eax,esi
                            0x69, 0xC0, 0xF4, 0x39, 0x00, 0x00, // imul eax,eax,000039F4
                            0x8D, 0x80, new BinRefTo("AIStateAddress", false), // lea eax,[eax+AIStateAddress]
                            0x8B, 0x58, 0x34, // mov ebx,[eax+34]
                            0x69, 0xDB, 0xF4, 0x39, 0x00, 0x00, // imul ebx,ebx,000039F4
                            0x8D, 0x9B, new BinRefTo("AIStateAddress", false), // lea ebx,[ebx+AIStateAddress]
                            0x8B, 0x5B, 0xD0, // mov ebx,[ebx-30]
                            0x8B, 0x40, 0xD0, // mov eax,[eax-30]
                            0x39, 0xD8, // cmp eax,ebx
                            0x5B, // pop ebx
                            0x58, // pop eax
                            0x0F, 0x8C, new BinRefTo("ai_restore_ai_siege_messages_end", true),
                            
                            // Now we can call PlaySiegeBikFromPlayer
                            0x56, // push esi
                            0xB9, new BinRefTo("AICArray", false), // mov ecx,AICArray
                            0xE8, new BinRefTo("PlaySiegeBikFromPlayer"), // call PlaySiegeBikFromPlayer
                            new BinLabel("ai_restore_ai_siege_messages_end")
                        }
                    }
                },
                
                new DefaultHeader("ai_restore_ai_siege_messages_33_or_stronger", false)
                {
                    // 4D0FC0
                    new BinaryEdit("ai_restore_ai_siege_messages_data1")
                    {
                        new BinAddress("isPlayerAliveHandle", 7),
                        new BinAddress("IsPlayerAliveFunction", 12, true),
                        new BinAddress("playerDataArray", 30),
                        new BinAddress("playBikHandle", 42),
                        new BinAddress("QueueBikPlayFunction", 47, true),
                    },
                    
                    // 42793C
                    new BinaryEdit("ai_restore_ai_siege_messages_data2")
                    {
                        new BinAddress("playerGroupArray", 22),
                    },
                    
                    // 57BA78
                    new BinaryEdit("ai_restore_ai_siege_messages_data3")
                    {
                        new BinAddress("currentPlayerID", 1),
                        new BinAddress("AICArray", 8),
                    },
                    
                    // 47A38B
                    new BinaryEdit("ai_restore_ai_siege_messages_data4")
                    {
                        new BinAddress("DAT_RandomNumber1", 12),
                    },
                    
                    // 4D4BD6
                    new BinaryEdit("ai_restore_ai_siege_messages")
                    {
                        new BinAlloc("aiToldToAttackBooleanArray", null)
                        {
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        },
                        new BinAlloc("PlaySiegeBikFromPlayer", null)
                        {
                            0x56, // push esi
                            0x8B, 0x74, 0x24, 0x08, // mov esi,[esp+08]
                            0x56, // push esi
                            0xB9, new BinRefTo("isPlayerAliveHandle", false), // mov ecx,isPlayerAliveHandle
                            0xE8, new BinRefTo("IsPlayerAliveFunction"), // call IsPlayerAliveFunction
                            0x85, 0xC0, // test eax,eax
                            0x53, // push ebx
                            0x74, 0x45, // je playSiegeBikFromPlayerSkip
                            
                            // Check if player is in the same group as the attacking AI
                            0xA1, new BinRefTo("currentPlayerID", false), // mov eax,currentPlayerID
                            0x0F, 0xB6, 0x98, new BinRefTo("playerGroupArray", false), // movzx ebx,byte ptr[eax+playerGroupArray]
                            0x38, 0x9E, new BinRefTo("playerGroupArray", false), // cmp [esi+playerGroupArray],bl
                            0x75, 0x31, // jne playSiegeBikFromPlayerSkip
                            
                            // Check if the AI just got the command to attack.
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0x83, 0x3C, 0x33, 0x00, // cmp dword ptr [ebx+esi],00
                            0x75, 0x25, // jne playSiegeBikFromPlayerSkip
                            
                            // We play the bik.
                            0x8B, 0xC6, // mov eax,esi
                            0x69, 0xC0, 0xF4, 0x39, 0x00, 0x00, // imul eax,eax,000039F4
                            0x8B, 0x88, new BinRefTo("playerDataArray", false), // mov ecx,[eax+playerDataArray]
                            0x68, 0x17, 0x00, 0x00, 0x00, // push 17
                            0x81, 0xE9, 0x01, 0x00, 0x00, 0x00, // sub ecx,01
                            0x51, // push ecx
                            0x56, // push esi
                            0xB9, new BinRefTo("playBikHandle", false), // mov ecx,playBikHandle
                            0xE8, new BinRefTo("QueueBikPlayFunction", true), // call QueueBikPlayFunction
                            
                            new BinLabel("playSiegeBikFromPlayerSkip"),
                            // reset aiToldToAttackBooleanArray for ai
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0xC7, 0x04, 0x33, 0x00, 0x00, 0x00, 0x00, // mov [ebx+esi],00000000
                            0x5B, // pop ebx
                            0x5E, // pop esi
                            0xC2, 0x04, 0x00, // ret 0004
                        },
                        new BinAddress("AIStateAddress", 13), // 2BA4
                        new BinAddress("OriginalFunctionAddress", 88, true),
                        new BinSkip(87),
                        new BinHook(5)
                        {
                            0xE8, new BinRefTo("OriginalFunctionAddress", true),
                            
                            0x50, // push eax
                            0x53, // push ebx
                            0x8B, 0xC6, // mov eax,esi
                            0x69, 0xC0, 0xF4, 0x39, 0x00, 0x00, // imul eax,eax,000039F4
                            0x8D, 0x80, new BinRefTo("AIStateAddress", false), // lea eax,[eax+AIStateAddress]
                            0x8B, 0x58, 0x34, // mov ebx,[eax+34]
                            0x69, 0xDB, 0xF4, 0x39, 0x00, 0x00, // imul ebx,ebx,000039F4
                            0x8D, 0x9B, new BinRefTo("AIStateAddress", false), // lea ebx,[ebx+AIStateAddress]
                            0x8B, 0x5B, 0xD0, // mov ebx,[ebx-30]
                            0x8B, 0x40, 0xD0, // mov eax,[eax-30]
                            0x39, 0xD8, // cmp eax,ebx
                            0x0F, 0x8E, new BinRefTo("checkChanceLabel", true), // jle checkChanceLabel
                            0x5B, // pop ebx
                            0x58, // pop eax
                            0xE9, new BinRefTo("playBikLabel"), // jmp playBikLabel
                            
                            new BinLabel("checkChanceLabel"),
                            0x0F, 0xB6, 0x05, new BinRefTo("DAT_RandomNumber1", false), // movzx eax,byte ptr [DAT_RandomNumber1]
                            0x25, 0x02, 0x00, 0x00, 0x00, // and eax,00000002
                            0x3D, 0x01, 0x00, 0x00, 0x00, // cmp eax,00000001
                            0x5B, // pop ebx
                            0x58, // pop eax
                            0x0F, 0x85, new BinRefTo("ai_restore_ai_siege_messages_end", true), // jne ai_restore_ai_siege_messages_end
                            
                            new BinLabel("playBikLabel"),
                            // Now we can call PlaySiegeBikFromPlayer
                            0x56, // push esi
                            0xB9, new BinRefTo("AICArray", false), // mov ecx,AICArray
                            0xE8, new BinRefTo("PlaySiegeBikFromPlayer"), // call PlaySiegeBikFromPlayer
                            new BinLabel("ai_restore_ai_siege_messages_end")
                        }
                    }
                },
                
                new DefaultHeader("ai_restore_ai_siege_messages_50", false)
                {
                    // 4D0FC0
                    new BinaryEdit("ai_restore_ai_siege_messages_data1")
                    {
                        new BinAddress("isPlayerAliveHandle", 7),
                        new BinAddress("IsPlayerAliveFunction", 12, true),
                        new BinAddress("playerDataArray", 30),
                        new BinAddress("playBikHandle", 42),
                        new BinAddress("QueueBikPlayFunction", 47, true),
                    },
                    
                    // 42793C
                    new BinaryEdit("ai_restore_ai_siege_messages_data2")
                    {
                        new BinAddress("playerGroupArray", 22),
                    },
                    
                    // 57BA78
                    new BinaryEdit("ai_restore_ai_siege_messages_data3")
                    {
                        new BinAddress("currentPlayerID", 1),
                        new BinAddress("AICArray", 8),
                    },
                    
                    // 47A38B
                    new BinaryEdit("ai_restore_ai_siege_messages_data4")
                    {
                        new BinAddress("DAT_RandomNumber1", 12),
                    },
                    
                    // 4D4BD6
                    new BinaryEdit("ai_restore_ai_siege_messages")
                    {
                        new BinAlloc("aiToldToAttackBooleanArray", null)
                        {
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        },
                        new BinAlloc("PlaySiegeBikFromPlayer", null)
                        {
                            0x56, // push esi
                            0x8B, 0x74, 0x24, 0x08, // mov esi,[esp+08]
                            0x56, // push esi
                            0xB9, new BinRefTo("isPlayerAliveHandle", false), // mov ecx,isPlayerAliveHandle
                            0xE8, new BinRefTo("IsPlayerAliveFunction"), // call IsPlayerAliveFunction
                            0x85, 0xC0, // test eax,eax
                            0x53, // push ebx
                            0x74, 0x45, // je playSiegeBikFromPlayerSkip
                            
                            // Check if player is in the same group as the attacking AI
                            0xA1, new BinRefTo("currentPlayerID", false), // mov eax,currentPlayerID
                            0x0F, 0xB6, 0x98, new BinRefTo("playerGroupArray", false), // movzx ebx,byte ptr[eax+playerGroupArray]
                            0x38, 0x9E, new BinRefTo("playerGroupArray", false), // cmp [esi+playerGroupArray],bl
                            0x75, 0x31, // jne playSiegeBikFromPlayerSkip
                            
                            // Check if the AI just got the command to attack.
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0x83, 0x3C, 0x33, 0x00, // cmp dword ptr [ebx+esi],00
                            0x75, 0x25, // jne playSiegeBikFromPlayerSkip
                            
                            // We play the bik.
                            0x8B, 0xC6, // mov eax,esi
                            0x69, 0xC0, 0xF4, 0x39, 0x00, 0x00, // imul eax,eax,000039F4
                            0x8B, 0x88, new BinRefTo("playerDataArray", false), // mov ecx,[eax+playerDataArray]
                            0x68, 0x17, 0x00, 0x00, 0x00, // push 17
                            0x81, 0xE9, 0x01, 0x00, 0x00, 0x00, // sub ecx,01
                            0x51, // push ecx
                            0x56, // push esi
                            0xB9, new BinRefTo("playBikHandle", false), // mov ecx,playBikHandle
                            0xE8, new BinRefTo("QueueBikPlayFunction", true), // call QueueBikPlayFunction
                            
                            new BinLabel("playSiegeBikFromPlayerSkip"),
                            // reset aiToldToAttackBooleanArray for ai
                            0xBB, new BinRefTo("aiToldToAttackBooleanArray", false), // mov ebx,aiToldToAttackBooleanArray
                            0x4B, // dec ebx
                            0xC7, 0x04, 0x33, 0x00, 0x00, 0x00, 0x00, // mov [ebx+esi],00000000
                            0x5B, // pop ebx
                            0x5E, // pop esi
                            0xC2, 0x04, 0x00, // ret 0004
                        },
                        new BinAddress("AIStateAddress", 13), // 2BA4
                        new BinAddress("OriginalFunctionAddress", 88, true),
                        new BinSkip(87),
                        new BinHook(5)
                        {
                            0xE8, new BinRefTo("OriginalFunctionAddress", true),
                            
                            0xF6, 0x05, new BinRefTo("DAT_RandomNumber1", false), 0x01, // test byte ptr [DAT_RandomNumber1],01
                            0x0F, 0x85, new BinRefTo("ai_restore_ai_siege_messages_end", true), // jne ai_restore_ai_siege_messages_end
                            
                            // Now we can call PlaySiegeBikFromPlayer
                            0x56, // push esi
                            0xB9, new BinRefTo("AICArray", false), // mov ecx,AICArray
                            0xE8, new BinRefTo("PlaySiegeBikFromPlayer"), // call PlaySiegeBikFromPlayer
                            new BinLabel("ai_restore_ai_siege_messages_end")
                        }
                    }
                },
            },

            #endregion

            #region UNITS
            
            //BinInt32.Change("laddermadness", ChangeType.Troops, 1),

            
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
                    BinBytes.CreateEdit("u_laddergold", 0xF7), // 1D - 9 = 14h            (vanilla: 1D - 19 = 4)
                    
                    new BinaryEdit("ui_fix_laddermen_cost_display_in_engineers_guild") // F5C91
                    {
                        new BinBytes(0xBB, 0x14),
                    }
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

            // ladderman: 0xB55AF4 = soldier bool
            
            new Change("o_restore_arabian_engineer_speech", ChangeType.Troops, false)
            {
                new DefaultHeader("o_restore_arabian_engineer_speech")
                {
                    new BinaryEdit("o_restore_arabian_engineer_speech_lord_type")
                    {
                        new BinAddress("SelectedLordType", 2)
                    },
                    new BinaryEdit("o_restore_arabian_engineer_speech")
                    {
                        new BinAlloc("ArabianEngineerWavs", null)
                        {
                            0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x73, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x73, 0x32, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x73, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x73, 0x32, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x73, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x73, 0x32, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x32, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x33, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x6F, 0x61, 0x74, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x6F, 0x61, 0x74, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x6F, 0x61, 0x74, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x6F, 0x61, 0x74, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x6D, 0x6F, 0x61, 0x74, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x61, 0x65, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x65, 0x72, 0x5F, 0x64, 0x69, 0x73, 0x62, 0x61, 0x6E, 0x64, 0x31, 0x2E, 0x77, 0x61, 0x76, 0x00, 0x00
                        },
                        new BinSkip(6),
                        new BinAddress("OriginalWavFileAddressArray", 10),
                        new BinHook(14)
                        {
                            0x3D, 0x0A, 0x00, 0x00, 0x00, // cmp eax,0A
                            0x75, 0x1C, // jne short 1C
                            
                            0x81, 0x3D, new BinRefTo("SelectedLordType", false), 0x01, 0x00, 0x00, 0x00, // cmp [SelectedLordType],00000001
                            0x75, 0x10, // jne short 10
                            
                            0x01, 0xD3, // add ebx,edx
                            0x69, 0xDB, 0x18, 0x00, 0x00, 0x00, // imul ebx,ebx,00000018
                            0x8D, 0x93, new BinRefTo("ArabianEngineerWavs", false), // lea edx,[ebx+ArabianEngineerWavs]
                            0xEB, 0x0E, // jmp short E
                            
                            0x01, 0xD3, // add ebx,edx
                            0x6B, 0xDB, 0x25, // imul ebx,ebx,25
                            0x01, 0xC3, // add ebx,eax
                            0x8B, 0x14, 0x9D, new BinRefTo("OriginalWavFileAddressArray", false) // mov edx,[ebx*4+OriginalWavFileAddressArray]
                        }
                    },
                    new BinaryEdit("o_restore_arabian_engineer_speech_purchase")
                    {
                        0x83, 0x3D, new BinRefTo("SelectedLordType", false), 0x01
                    },
                    // 466EC9
                    new BinaryEdit("o_restore_arabian_engineer_speech_engine")
                    {
                        new BinSkip(9),
                        new BinAddress("OriginalEngineerWavAddress", 1),
                        new BinHook(5)
                        {
                            0x81, 0x3D, new BinRefTo("SelectedLordType", false), 0x01, 0x00, 0x00, 0x00, // cmp [SelectedLordType],00000001
                            0x75, 0x07, // jne short 0x07
                            0x68, new BinRefTo("ArabianEngineerWavs", false), // push ArabianEngineerWavs
                            0xEB, 0x05, // jmp short 0x05
                            0x68, new BinRefTo("OriginalEngineerWavAddress", false) // push OriginalEngineerWavAddress
                        }
                    }
                }
            },

            new Change("u_spearmen_run", ChangeType.Troops, false)
            {
                new DefaultHeader("u_spearmen_run")
                {
                    // 0055E07E
                    new BinaryEdit("u_spearmen_run")
                    {
                        new BinBytes(0x90, 0x90) // remove je
                    }
                }
            },

            #endregion

            #region OTHER

            /*
             *  FIRE COOLDOWN
             */

            // 0x00410A30 + 8 ushort default = 2000
            new Change("o_firecooldown", ChangeType.Other, false)
            {
                new SliderHeader("o_firecooldown", true, 0, 20000, 500, 2000, 4000)
                {
                    new BinaryEdit("o_firecooldown")
                    {
                        new BinSkip(8),
                        new BinInt16Value()
                    },
                }
            },


            /*
             * EXTREME
             */

            new Change("o_xtreme", ChangeType.Other, false)
            {
                new DefaultHeader("o_xtreme")
                {
                    // 0057CAC5 disable manabar rendering
                    BinNops.CreateEdit("o_xtreme_bar1", 10),
                    
                    // 4DA3E0 disable manabar clicks
                    BinBytes.CreateEdit("o_xtreme_bar2", 0xC3),
                }
            },


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

                    // 004BE94F
                    BinHook.CreateEdit("o_playercolor_ai_video_message_shield", 9,
                        0x80, 0xFB, new BinByteValue(), //  CMP EBX, value
                        0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                        0xBB, 0x01, 0x00, 0x00, 0x00, //  MOV EBX, 1

                        0x50, 0x52, 0xC7, 0x41, 0x04, 0x01, 0x00, 0x00, 0x00 // original code
                    ),

                    // 004B7B2C
                    BinHook.CreateEdit("o_playercolor_ai_video_message_shield_pre", 6,
                        0x8B, 0x86, 0xD4, 0x00, 0x00, 0x00, //  MOV EAX, [esi+D4]
                        0x83, 0xF8, new BinByteValue(), //  CMP EAX, value
                        0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                        0xB8, 0x01, 0x00, 0x00, 0x00 //  MOV EAX, 1
                    ),

                    // 004B660A
                    BinHook.CreateEdit("o_playercolor_ai_video_message_shield_enemy_taunt", 6,
                        0x83, 0xF8, new BinByteValue(), //  CMP EAX, value
                        0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                        0xB8, 0x01, 0x00, 0x00, 0x00, //  MOV EAX, 1
                        0x52, 0x05, 0xD5, 0x01, 0x00, 0x00 //  original code
                    ),

                    // 004B7E7F
                    BinHook.CreateEdit("o_playercolor_ai_video_message_emblem", 7,
                        0x83, 0xF8, new BinByteValue(), //  CMP EAX, value
                        0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                        0xB8, 0x01, 0x00, 0x00, 0x00, //  MOV EAX, 1
                        0x55, 0x53, 0x05, 0x22, 0x02, 0x00, 0x00 // original code
                    ),
                    
                    // 004AC8A5
                    new BinaryEdit("o_playercolor_ai_allied_menu_emblem")
                    {
                        new BinSkip(16),
                        new BinHook(8)
                        {
                            0x83, 0xFE, new BinByteValue(), //  CMP ESI, value
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                            0xBE, 0x01, 0x00, 0x00, 0x00, //  MOV ESI, 1
                            0x50, 0x51, 0x8D, 0x96, 0x22, 0x02, 0x00, 0x00 // original code
                        }
                    },
                    
                    // 004ACEED
                    new BinaryEdit("o_playercolor_ai_allied_menu_attack_emblem")
                    {
                        new BinSkip(1),
                        new BinHook(5)
                        {
                            0x83, 0xF8, new BinByteValue(), //  CMP EAX, value
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                            0xB8, 0x01, 0x00, 0x00, 0x00, //  MOV EAX,1
                            0x05, 0xCE, 0x02, 0x00, 0x00 //  ADD EAX,2CE
                        }
                    },
                    
                    // 004AD556
                    new BinaryEdit("o_playercolor_ai_order_menu_emblem")
                    {
                        new BinSkip(62),
                        new BinHook(6)
                        {
                            0x83, 0xFE, new BinByteValue(), //  CMP ESI, value
                            0x0F, 0x85, 0x0B, 0x00, 0x00, 0x00, //  JNE SHORT 11h
                            0x8D, 0x15, 0x23, 0x02, 0x00, 0x00, //  LEA edx,[00000223]
                            0xEB, 0x09, 0x90, 0x90, 0x90, //  JMP SHORT 9
                            0x8D, 0x96, 0x22, 0x02, 0x00, 0x00 //  LEA edx,[00000223]
                        }
                    },
                    
                    // 004ACC84
                    new BinaryEdit("o_playercolor_ai_allied_menu_ally_name")
                    {
                        new BinHook(7)
                        {
                            0x51, //  PUSH EAX
                            0xB9, new BinByteValue(), 0x00, 0x00, 0x00, //  MOV ECX, value
                            0x83, 0xF9, 0x01, //  CMP ECX, 1
                            0x0F, 0x84, 0x97, 0x00, 0x00, 0x00, //  JE SHORT 97h
                            
                            0x83, 0xF9, 0x07, //  CMP ECX, 7
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xB9, 0x07, 0x00, 0x00, 0x00,  //  MOV ECX, 7
                            0xE9, 0x72, 0x00, 0x00, 0x00,  //  JMP SHORT 72h
                            
                            0x83, 0xF9, 0x08, //  CMP ECX, 8
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xB9, 0x08, 0x00, 0x00, 0x00,  //  MOV ECX, 8
                            0xE9, 0x5F, 0x00, 0x00, 0x00,  //  JMP SHORT 5Fh
                            
                            0x83, 0xF9, 0x06, //  CMP ECX, 6
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xB9, 0x05, 0x00, 0x00, 0x00,  //  MOV ECX, 5
                            0xE9, 0x4C, 0x00, 0x00, 0x00,  //  JMP SHORT 4Ch
                            
                            0x83, 0xF9, 0x02, //  CMP ECX, 2
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xB9, 0x03, 0x00, 0x00, 0x00,  //  MOV ECX, 3
                            0xE9, 0x39, 0x00, 0x00, 0x00,  //  JMP SHORT 39h
                            
                            0x83, 0xF9, 0x03, //  CMP ECX, 3
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xB9, 0x04, 0x00, 0x00, 0x00,  //  MOV ECX, 4
                            0xE9, 0x26, 0x00, 0x00, 0x00,  //  JMP SHORT 26h
                            
                            0x83, 0xF9, 0x04, //  CMP ECX, 4
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xB9, 0x02, 0x00, 0x00, 0x00,  //  MOV ECX, 2
                            0xE9, 0x13, 0x00, 0x00, 0x00,  //  JMP SHORT 13h
                            
                            0x83, 0xF9, 0x05, //  CMP ECX, 5
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xB9, 0x06, 0x00, 0x00, 0x00,  //  MOV ECX, 6
                            0xE9, 0x00, 0x00, 0x00, 0x00,  //  JMP SHORT 0
                            
                            0x39, 0xCA, //  CMP EDX,ECX
                            0x0F, 0x85, 0x0A, 0x00, 0x00, 0x00, //  JNE SHORT 0Ah
                            0xBA, 0x01, 0x00, 0x00, 0x00, //  MOV EDX, 1
                            0xE9, 0x00, 0x00, 0x00, 0x00, //  JMP SHORT 0
                            
                            0x8B, 0x0D, 0x7C, 0x50, 0x61, 0x00, //  MOV ECX,0061507C
                            0x83, 0xF9, 0x00, //  CMP ECX,00
                            0x75, 0x0C, //  JNE SHORT C
                            0x8B, 0x04, 0x95, 0x0C, 0x52, 0x61, 0x00, //  extreme
                            0xE9, 0x07, 0x00, 0x00, 0x00, //  JMP SHORT 7
                            0x8B, 0x04, 0x95, 0x7C, 0x50, 0x61, 0x00,  //  original
                            0x59, //  POP ECX
                        }
                    },

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
                    },

                    // 00451E03
                    BinHook.CreateEdit("o_playercolor_fade", 5,
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
                        new BinBytes(0xB0, 0x01) // mov al, 1
                        // end
                    ),

                    // 004E7E45
                    BinHook.CreateEdit("o_playercolor_trebuchet", 5,
                        new BinBytes(0x50), // ori code: push eax

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

                        new BinBytes(0xA3), // mov [var], eax
                        new BinRefTo("var", false),

                        // ori code
                        new BinBytes(0x8B, 0x44, 0x24, 0x14) // mov eax, [esp+14]
                    ),

                    #endregion
                },
            },

            /*
             *  WASD
             */
             
            new Change("o_keys", ChangeType.Other, false)
            {
                new DefaultHeader("o_keys")
                {
                    // 495800
                    new BinaryEdit("o_keys_savefunc")
                    {
                        new BinAddress("self", 17),
                        new BinAddress("c1", 22),
                        new BinAddress("func", 27, true),
                        new BinAddress("savefunc", 50, true),

                        // 0x20 == save, 0x1F == load
                        new BinAlloc("DoSave", null)
                        {
                            0x8B, 0x44, 0x24, 0x04, // mov eax, [esp+4]
                            0xA3, new BinRefTo("c1", false), // mov [c1], eax
                            0xB9, new BinRefTo("self", false), // mov ecx, self
                            0x6A, 0x0E, // push E
                            0xE8, new BinRefTo("func"), // call func
                            0xE9, new BinRefTo("savefunc"), // jmp to save
                        }
                    },    

                    // 004697C0
                    new BinaryEdit("o_keys_savename")
                    {
                        new BinAlloc("namebool", 1),
                        new BinAlloc("name", Encoding.ASCII.GetBytes("Quicksave\0")),
                        new BinHook(9)
                        {
                            0x80, 0x3D, new BinRefTo("namebool", false), 0x00, // cmp byte ptr [namebool], 0
                            JMP(EQUALS, 0x06), // je to ori code
                            0xB8, new BinRefTo("name", false), // mov eax, quicksave
                            0xC3, // ret
                            // ori code:
                            new BinBytes(0x83, 0x79, 0x04, 0x00, 0x75, 0x03, 0x33, 0xC0, 0xC3)
                        }
                    },          

                    // 004B3B5C S key
                    new BinaryEdit("o_keys_s")
                    {
                        new BinAddress("ctrl", 0x106), // 4B3C60
                        
                        0x39, 0x1D, new BinRefTo("ctrl", false),  // cmp [ctrlpressed], ebx = 0
                        0x0F, 0x84, 0xFA, 0xF3, 0xFF, 0xFF,       // jmp to move if equal

                        0xC6, 0x05, new BinRefTo("namebool", false), 0x01,

                        PUSH(0x20), // push 0x20
                        0xE8, new BinRefTo("DoSave"), // call save func
                        

                        0xC6, 0x05, new BinRefTo("namebool", false), 0x00,

                        POP(EAX), // pop eax
                        JMP(UNCONDITIONAL, 0x53) // jmp to default/end 4B3BD3
                    },

                       

                    // 0046C2E0
                    new BinaryEdit("o_keys_loadname")
                    {
                        new BinAddress("someoffset", 25),
                        new BinHook(9)
                        {
                            0x80, 0x3D, new BinRefTo("namebool", false), 0x00, // cmp byte ptr [namebool], 0
                            JMP(EQUALS, 0x08), // je to ori code
                            0xB8, new BinRefTo("name", false), // mov eax, quicksave
                            0xC2, 0x04, 0x00, // ret

                            // ori code:
                            new BinBytes(0x8B, 0x44, 0x24, 0x04, 0x3D, 0xF4, 0x01, 0x00, 0x00)
                        }
                    },  
                    
                    // 004B3DAE L key
                    new BinaryEdit("o_keys_l")
                    {
                        new BinAddress("somevar", 0x02),
                        new BinAddress("default", 0x20, true),
                        new BinHook(6)
                        {
                            0x39, 0x1D, new BinRefTo("ctrl", false),  // cmp [ctrlpressed], ebx = 0
                            JMP(EQUALS, 0x1B), // je to ori code

                            0xC6, 0x05, new BinRefTo("namebool", false), 0x01,

                            PUSH(0x1F), // push 0x1F
                            0xE8, new BinRefTo("DoSave"), // call save func
                            
                            0xC6, 0x05, new BinRefTo("namebool", false), 0x00,

                            POP(EAX), // pop eax

                            0xE9, new BinRefTo("default"), // jump awayy
                            
                            // ori code
                            0x39, 0x1D, new BinRefTo("somevar", false)
                        }
                    },

                    // WASD
                    // Arrow Keys: 4b4ee4 + 1D => 9, A, B, C
                    // WASD Keys: 4b4ee4 + 39, 4F, 3C, 4B
                    new BinaryEdit("o_keys_down")
                    {
                        // 4b4ee4 + 39
                        new BinBytes(0x09),
                        new BinSkip(0x02),
                        new BinBytes(0x0B),
                        //new BinSkip(0x0E),
                        //new BinBytes(0x0C),
                        new BinSkip(0x03 + 0x0E + 1),
                        new BinBytes(0x0A),
                    },

                    // WASD
                    // 004B4C9F
                    new BinaryEdit("o_keys_up")
                    {
                        new BinHook(6, null, 0xE9)
                        {
                            0x83, 0xC0, 0xDB, // add eax, -25

                            // 1C left => 0
                            // 32 top => 1
                            // 1F right => 2
                            // 2E down => 3

                            CMP(EAX, 0x1C), // cmp eax, 1C
                            JMP(NOTEQUALS, 0x04),       // jne to next
                            XOR(EAX, EAX),       // xor eax, eax
                            JMP(UNCONDITIONAL, 0x1C),       // jmp to end
                            
                            CMP(EAX, 0x32), // cmp eax, 32
                            JMP(NOTEQUALS, 0x05),       // jne to next
                            0x8D, 0x40, 0xCF, // lea eax, [eax-31]
                            JMP(UNCONDITIONAL, 0x12),       // jmp to end
                            
                            CMP(EAX, 0x1F), // cmp eax, 1F
                            JMP(NOTEQUALS, 0x05),       // jne to next
                            0x8D, 0x40, 0xE3, // lea eax, [eax-1D]
                            JMP(UNCONDITIONAL, 0x8),       // jmp to end

                            CMP(EAX, 0x2E), // cmp eax, 2E
                            JMP(NOTEQUALS, 0x03),       // jne to end 
                            0x8D, 0x40, 0xD5, // lea eax, [eax-2B]

                            // end
                            CMP(EAX, 0x3), // cmp eax, 3
                        }
                    },

                    new BinaryEdit("o_keys_menu")
                    {
                        new BinAddress("callright", 6, true),
                        new BinAddress("callleft", 0x93, true),

                        new BinSkip(5),
                        new BinHook(5)
                        {
                            0x83, 0xFE, 0x44,
                            JMP(EQUALS, 0x05),
                            0xE8, new BinRefTo("callright")
                        },

                        new BinSkip(0x88),
                        new BinHook(5)
                        {
                            0x83, 0xFE, 0x41,
                            JMP(EQUALS, 0x05),
                            0xE8, new BinRefTo("callleft")
                        }
                    }
                }
            },
            
            /*
             *  Override Identity Menu
             */
             
            new Change("o_override_identity_menu", ChangeType.Other, false, false)
            {
                new DefaultHeader("o_override_identity_menu")
                {
                    // F6286
                    new BinaryEdit("o_override_identity_menu")
                    {
                        new BinAlloc("NormalCrusaderUnitListAddress", null)
                        {
                            0x38, 0x64, 0x61, 0x00
                        },
                        new BinAlloc("NormalArabUnitListAddress", null)
                        {
                            0x28, 0x65, 0x61, 0x00
                        },

                        new BinAlloc("ExtremeNormalCrusaderUnitListAddress", null)
                        {
                            0xC8, 0x65, 0x61, 0x00
                        },
                        new BinAlloc("ExtremeNormalArabUnitListAddress", null)
                        {
                            0xB8, 0x66, 0x61, 0x00
                        },

                        new BinAlloc("CurrentlySelectedRangedUnit", null)
                        {
                            0x00, 0x00, 0x00, 0x00
                        },
                        new BinAlloc("CurrentlySelectedMeleeUnit", null)
                        {
                            0x00, 0x00, 0x00, 0x00
                        },

                        new BinAlloc("SetSelectedRangedUnit", null)
                        {
                            PUSH(FLAGS), //  pushf
                            PUSH(EAX), //  push eax
                            PUSH(ECX), //  push ecx
                            PUSH(EDX), //  push edx
                            0xBA, 0x00, 0x00, 0x00, 0x00, //  mov edx,00
                            
                            0xA1, new BinRefTo("NormalCrusaderUnitListAddress", false), //  mov eax,NormalCrusaderUnitListAddress
                            PUSH(EAX), //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            POP(EAX), //  pop eax
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  jne short 5
                            0xA1, new BinRefTo("ExtremeNormalCrusaderUnitListAddress", false), //  mov eax,ExtremeNormalCrusaderUnitListAddress
                            
                            // Set ranged units to 0 in list
                            new BinLabel("ResetRangedUnitsInList"),
                            0xC7, 0x00, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x04, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x28, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x30, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x38, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x40, 0x00, 0x00, 0x00, 0x00,

                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x00, //  cmp dword ptr [CurrentlySelectedRangedUnit],00
                            0x0F, 0x85, 0x20, 0x00, 0x00, 0x00, //  jne short 20
                            0xC7, 0x00, 0x06, 0x00, 0x00, 0x00, //  mov [eax],06
                            0xC7, 0x46, 0x20, 0x7C, 0x01, 0x00, 0x40, //  mov [esi+20],4000017C
                            0xC7, 0x46, 0x08, 0xAB, 0x00, 0x00, 0x00, //  mov [esi+08],000000AB
                            0xC7, 0x46, 0x04, 0xC0, 0x00, 0x00, 0x00, //  mov [esi+08],000000C0
                            0xE9, 0xE1, 0x00, 0x00, 0x00, //  jmp short E1
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x01, //  cmp dword ptr [CurrentlySelectedRangedUnit],01
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x04, 0x03, 0x00, 0x00, 0x00, //  mov [eax+04],03
                            0xC7, 0x46, 0x20, 0x7F, 0x01, 0x00, 0x40, //  mov [esi+20],4000017F
                            0xC7, 0x46, 0x08, 0xB9, 0x00, 0x00, 0x00, //  mov [esi+08],000000B9
                            0xC7, 0x46, 0x04, 0xC1, 0x00, 0x00, 0x00, //  mov [esi+08],000000C1
                            0xE9, 0xB3, 0x00, 0x00, 0x00, //  jmp short B3
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x02, //  cmp dword ptr [CurrentlySelectedRangedUnit],02
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x28, 0x05, 0x00, 0x00, 0x00, //  mov [eax+28],05
                            0xC7, 0x46, 0x20, 0x1C, 0x02, 0x00, 0x40, //  mov [esi+20],4000021C
                            0xC7, 0x46, 0x08, 0xB1, 0x00, 0x00, 0x00, //  mov [esi+08],000000B1
                            0xC7, 0x46, 0x04, 0xC1, 0x00, 0x00, 0x00, //  mov [esi+08],000000C1
                            0xE9, 0x85, 0x00, 0x00, 0x00, //  jmp short 85
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x03, //  cmp dword ptr [CurrentlySelectedRangedUnit],03
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x30, 0x09, 0x00, 0x00, 0x00, //  mov [eax+30],09
                            0xC7, 0x46, 0x20, 0x1E, 0x02, 0x00, 0x40, //  mov [esi+20],4000021E
                            0xC7, 0x46, 0x08, 0xB2, 0x00, 0x00, 0x00, //  mov [esi+08],000000B2
                            0xC7, 0x46, 0x04, 0xC1, 0x00, 0x00, 0x00, //  mov [esi+08],000000C1
                            0xE9, 0x57, 0x00, 0x00, 0x00, //  jmp short 57
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x04, //  cmp dword ptr [CurrentlySelectedRangedUnit],04
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x38, 0x03, 0x00, 0x00, 0x00, //  mov [eax+30],03
                            0xC7, 0x46, 0x20, 0x20, 0x02, 0x00, 0x40, //  mov [esi+20],40000220
                            0xC7, 0x46, 0x08, 0xAC, 0x00, 0x00, 0x00, //  mov [esi+08],000000AC
                            0xC7, 0x46, 0x04, 0xBB, 0x00, 0x00, 0x00, //  mov [esi+08],000000BB
                            0xE9, 0x29, 0x00, 0x00, 0x00, //  jmp short 29
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x05, //  cmp dword ptr [CurrentlySelectedRangedUnit],05
                            0x0F, 0x85, 0x1C, 0x00, 0x00, 0x00, //  jne short 1C
                            0xC7, 0x40, 0x40, 0x03, 0x00, 0x00, 0x00, //  mov [eax+30],03
                            0xC7, 0x46, 0x20, 0x22, 0x02, 0x00, 0x40, //  mov [esi+20],40000222
                            0xC7, 0x46, 0x08, 0xB9, 0x00, 0x00, 0x00, //  mov [esi+08],000000B9
                            0xC7, 0x46, 0x04, 0xC1, 0x00, 0x00, 0x00, //  mov [esi+08],000000C1
                            
                            CMP(EDX, 0x01), //  cmp edx,01
                            0x0F, 0x84, 0x20, 0x00, 0x00, 0x00, //  je short 20
                            0x42, //  inc edx
                            
                            PUSH(EAX), //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            POP(EAX), //  pop eax
                            
                            0xA1, new BinRefTo("NormalArabUnitListAddress", false), //  mov eax,NormalArabUnitListAddress
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  jne short 5
                            0xA1, new BinRefTo("ExtremeNormalArabUnitListAddress", false), //  mov eax,ExtremeNormalArabUnitListAddress
                            0xE9, new BinRefTo("ResetRangedUnitsInList"), //  jmp ResetMeleeUnitsInList
                            
                            POP(EDX),  // pop edx
                            POP(ECX),  // pop ecx
                            POP(EAX), //  pop eax
                            POP(FLAGS), //  popf
                            0xC3 //  ret
                        },
                        new BinAlloc("RangedUnitButtonClick", null)
                        {
                            PUSH(FLAGS), //  pushf
                            0xFF, 0x05, new BinRefTo("CurrentlySelectedRangedUnit", false), //  inc [CurrentlySelectedRangedUnit]
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x06, //  cmp [CurrentlySelectedRangedUnit],06
                            0xF, 0x8C, 0x0A, 0x00, 0x00, 0x00, //  jl short A
                            0xC7, 0x05, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x00, 0x00, 0x00, 0x00, //  mov [CurrentlySelectedRangedUnit],00
                            0xE8, new BinRefTo("SetSelectedRangedUnit"),
                            POP(FLAGS), //  popf
                            0xC3 //  ret
                        },

                        new BinAlloc("SetSelectedMeleeUnit", null)
                        {
                            PUSH(FLAGS), //  pushf
                            PUSH(EAX), //  push eax
                            PUSH(ECX), //  push ecx
                            PUSH(EDX), //  push edx
                            0xBA, 0x00, 0x00, 0x00, 0x00, //  mov edx,00
                            
                            0xA1, new BinRefTo("NormalCrusaderUnitListAddress", false), //  mov eax,NormalCrusaderUnitListAddress
                            PUSH(EAX), //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            POP(EAX), //  pop eax
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  jne short 5
                            0xA1, new BinRefTo("ExtremeNormalCrusaderUnitListAddress", false), //  mov eax,ExtremeNormalCrusaderUnitListAddress
                            
                            // Set melee units to 0 in list
                            new BinLabel("ResetMeleeUnitsInList"),
                            0xC7, 0x40, 0x08, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x0C, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x10, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x14, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x18, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x20, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x24, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x2C, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x34, 0x00, 0x00, 0x00, 0x00,
                            0xC7, 0x40, 0x3C, 0x00, 0x00, 0x00, 0x00,

                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x00, //  cmp dword ptr [CurrentlySelectedMeleeUnit],00
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x08, 0x07, 0x00, 0x00, 0x00, //  mov [eax+08],07
                            0xC7, 0x46, 0x20, 0x7D, 0x01, 0x00, 0x40, //  mov [esi+20],4000017D
                            0xC7, 0x46, 0x08, 0xB9, 0x00, 0x00, 0x00, //  mov [esi+08],000000B9
                            0xC7, 0x46, 0x04, 0xF7, 0x00, 0x00, 0x00, //  mov [esi+08],000000F7
                            0xE9, 0x99, 0x01, 0x00, 0x00, //  jmp short 99 01
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x01, //  cmp dword ptr [CurrentlySelectedMeleeUnit],01
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x0C, 0x03, 0x00, 0x00, 0x00, //  mov [eax+0C],03
                            0xC7, 0x46, 0x20, 0x80, 0x01, 0x00, 0x40, //  mov [esi+20],40000180
                            0xC7, 0x46, 0x08, 0xA5, 0x00, 0x00, 0x00, //  mov [esi+08],000000A5
                            0xC7, 0x46, 0x04, 0xFB, 0x00, 0x00, 0x00, //  mov [esi+08],000000FB
                            0xE9, 0x6B, 0x01, 0x00, 0x00, //  jmp short 6B 01
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x02, //  cmp dword ptr [CurrentlySelectedMeleeUnit],02
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x10, 0x04, 0x00, 0x00, 0x00, //  mov [eax+10],04
                            0xC7, 0x46, 0x20, 0x7E, 0x01, 0x00, 0x40, //  mov [esi+20],4000017E
                            0xC7, 0x46, 0x08, 0xB9, 0x00, 0x00, 0x00, //  mov [esi+08],000000B9
                            0xC7, 0x46, 0x04, 0xFB, 0x00, 0x00, 0x00, //  mov [esi+08],000000FB
                            0xE9, 0x3D, 0x01, 0x00, 0x00, //  jmp short 3D 01
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x03, //  cmp dword ptr [CurrentlySelectedMeleeUnit],03
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x14, 0x02, 0x00, 0x00, 0x00, //  mov [eax+14],02
                            0xC7, 0x46, 0x20, 0x81, 0x01, 0x00, 0x40, //  mov [esi+20],40000181
                            0xC7, 0x46, 0x08, 0xB1, 0x00, 0x00, 0x00, //  mov [esi+08],000000B1
                            0xC7, 0x46, 0x04, 0xFB, 0x00, 0x00, 0x00, //  mov [esi+08],000000FB
                            0xE9, 0x0F, 0x01, 0x00, 0x00, //  jmp short 0F 01
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x04, //  cmp dword ptr [CurrentlySelectedMeleeUnit],04
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x18, 0x01, 0x00, 0x00, 0x00, //  mov [eax+18],01
                            0xC7, 0x46, 0x20, 0x82, 0x01, 0x00, 0x40, //  mov [esi+20],40000182
                            0xC7, 0x46, 0x08, 0xA5, 0x00, 0x00, 0x00, //  mov [esi+08],000000A5
                            0xC7, 0x46, 0x04, 0xF3, 0x00, 0x00, 0x00, //  mov [esi+08],000000F3
                            0xE9, 0xE1, 0x00, 0x00, 0x00, //  jmp short E1
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x05, //  cmp dword ptr [CurrentlySelectedMeleeUnit],05
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x20, 0x03, 0x00, 0x00, 0x00, //  mov [eax+20],03
                            0xC7, 0x46, 0x20, 0x83, 0x01, 0x00, 0x40, //  mov [esi+20],40000183
                            0xC7, 0x46, 0x08, 0xB9, 0x00, 0x00, 0x00, //  mov [esi+08],000000B9
                            0xC7, 0x46, 0x04, 0xFA, 0x00, 0x00, 0x00, //  mov [esi+08],000000FA
                            0xE9, 0xB3, 0x00, 0x00, 0x00, //  jmp short B3
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x06, //  cmp dword ptr [CurrentlySelectedMeleeUnit],06
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x24, 0x06, 0x00, 0x00, 0x00, //  mov [eax+24],06
                            0xC7, 0x46, 0x20, 0x86, 0x01, 0x00, 0x40, //  mov [esi+20],40000186
                            0xC7, 0x46, 0x08, 0xB9, 0x00, 0x00, 0x00, //  mov [esi+08],000000B9
                            0xC7, 0x46, 0x04, 0xFA, 0x00, 0x00, 0x00, //  mov [esi+08],000000FA
                            0xE9, 0x85, 0x00, 0x00, 0x00, //  jmp short 85
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x07, //  cmp dword ptr [CurrentlySelectedMeleeUnit],07
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x2C, 0x08, 0x00, 0x00, 0x00, //  mov [eax+2C],08
                            0xC7, 0x46, 0x20, 0x1D, 0x02, 0x00, 0x40, //  mov [esi+20],4000021D
                            0xC7, 0x46, 0x08, 0xB0, 0x00, 0x00, 0x00, //  mov [esi+08],000000B0
                            0xC7, 0x46, 0x04, 0xFB, 0x00, 0x00, 0x00, //  mov [esi+08],000000FB
                            0xE9, 0x57, 0x00, 0x00, 0x00, //  jmp short 57
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x08, //  cmp dword ptr [CurrentlySelectedMeleeUnit],08
                            0x0F, 0x85, 0x21, 0x00, 0x00, 0x00, //  jne short 21
                            0xC7, 0x40, 0x34, 0x04, 0x00, 0x00, 0x00, //  mov [eax+34],04
                            0xC7, 0x46, 0x20, 0x1F, 0x02, 0x00, 0x40, //  mov [esi+20],4000021F
                            0xC7, 0x46, 0x08, 0xB8, 0x00, 0x00, 0x00, //  mov [esi+08],000000B8
                            0xC7, 0x46, 0x04, 0xFA, 0x00, 0x00, 0x00, //  mov [esi+08],000000FA
                            0xE9, 0x29, 0x00, 0x00, 0x00, //  jmp short 29
                            
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x09, //  cmp dword ptr [CurrentlySelectedMeleeUnit],09
                            0x0F, 0x85, 0x1C, 0x00, 0x00, 0x00, //  jne short 1C
                            0xC7, 0x40, 0x3C, 0x03, 0x00, 0x00, 0x00, //  mov [eax+3C],03
                            0xC7, 0x46, 0x20, 0x21, 0x02, 0x00, 0x40, //  mov [esi+20],40000221
                            0xC7, 0x46, 0x08, 0xB0, 0x00, 0x00, 0x00, //  mov [esi+08],000000B0
                            0xC7, 0x46, 0x04, 0xFB, 0x00, 0x00, 0x00, //  mov [esi+08],000000FB
                            
                            CMP(EDX, 0x01), //  cmp edx,01
                            0x0F, 0x84, 0x20, 0x00, 0x00, 0x00, //  je short 20
                            0x42, //  inc edx
                            
                            PUSH(EAX), //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            POP(EAX), //  pop eax
                            
                            0xA1, new BinRefTo("NormalArabUnitListAddress", false), //  mov eax,NormalArabUnitListAddress
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  jne short 5
                            0xA1, new BinRefTo("ExtremeNormalArabUnitListAddress", false), //  mov eax,ExtremeNormalArabUnitListAddress
                            0xE9, new BinRefTo("ResetMeleeUnitsInList"), //  jmp ResetMeleeUnitsInList
                            
                            POP(EDX),  // pop edx
                            POP(ECX),  // pop ecx
                            POP(EAX), //  pop eax
                            POP(FLAGS), //  popf
                            0xC3 //  ret
                        },
                        new BinAlloc("MeleeUnitButtonClick", null)
                        {
                            PUSH(FLAGS), //  pushf
                            0xFF, 0x05, new BinRefTo("CurrentlySelectedMeleeUnit", false), //  inc [CurrentlySelectedMeleeUnit]
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x0A, //  cmp [CurrentlySelectedMeleeUnit],0A
                            0xF, 0x8C, 0x0A, 0x00, 0x00, 0x00, //  jl short A
                            0xC7, 0x05, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x00, 0x00, 0x00, 0x00, //  mov [CurrentlySelectedMeleeUnit],00
                            0xE8, new BinRefTo("SetSelectedMeleeUnit"),
                            POP(FLAGS), //  popf
                            0xC3 //  ret
                        },

                        new BinAlloc("IdentityMenuButtonArray", null)
                        {
                            0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xE0,0x1C,0x49,0x00,0x00,0x00,0x00,0x00,0x70,0x1D,0x49,0x00,0x05,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x50,0x00,0x00,0x00,0x5F,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0xE0,0x1C,0x49,0x00,0x15,0x00,0x00,0x00,0x70,0x1D,0x49,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xEC,0xB3,0x3D,0x00,0xEC,0xB3,0x3D,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0xDA,0x00,0x00,0x00,0x5F,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0xE0,0x1C,0x49,0x00,0x16,0x00,0x00,0x00,0x70,0x1D,0x49,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x5E,0xB2,0x3D,0x00,0x5E,0xB2,0x3D,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x50,0x00,0x00,0x00,0xB9,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0xE0,0x1C,0x49,0x00,0x17,0x00,0x00,0x00,0x70,0x1D,0x49,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x99,0x00,0x00,0x00,0xB9,0x00,0x00,0x00,0x18,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0xE0,0x1C,0x49,0x00,0x28,0x00,0x00,0x00,0x70,0x1D,0x49,0x00,0xF2,0x01,0x00,0x00,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xDB,0xAF,0x3D,0x00,0xDB,0xAF,0x3D,0x00,0x00,0x00,0x00,0x00,0xB7,0x02,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x99,0x00,0x00,0x00,0xD6,0x00,0x00,0x00,0x18,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0xE0,0x1C,0x49,0x00,0x29,0x00,0x00,0x00,0x70,0x1D,0x49,0x00,0xF3,0x01,0x00,0x00,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFA,0xB0,0x3D,0x00,0xFA,0xB0,0x3D,0x00,0x00,0x00,0x00,0x00,0xB9,0x02,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x30,0x3D,0x49,0x00,0x00,0x00,0x00,0x00,0xF0,0x3F,0x46,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x01,0x9C,0xFF,0xFF,0xFF,0x76,0x01,0x00,0x00,0xB4,0x00,0x00,0x00,0xB4,0x00,0x00,0x00,0x30,0x3D,0x49,0x00,0x11,0x00,0x00,0x00,0xF0,0x3F,0x46,0x00,0x70,0x01,0x00,0x40,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x45,0x01,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x45,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x10,0xCC,0x47,0x00,0x00,0x00,0x00,0x00,0xA0,0xCC,0x47,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x50,0x00,0x00,0x00,0x13,0x01,0x00,0x00,0x54,0x01,0x00,0x00,0x22,0x00,0x00,0x00,0x10,0xCC,0x47,0x00,0x04,0x00,0x00,0x00,0xA0,0xCC,0x47,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC3,0xA0,0x86,0x00,0xC3,0xA0,0x86,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0xC0,0x00,0x00,0x00,0xAB,0x00,0x00,0x00,0x2C,0x00,0x00,0x00,0x58,0x00,0x00,0x00,new BinRefTo("RangedUnitButtonClick", false),0x11,0x00,0x00,0x00,0xF0,0x3F,0x46,0x00,0x7C,0x01,0x00,0x40,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3D,0xB8,0x54,0x00,0x3D,0xB8,0x54,0x00,0x00,0x00,0x00,0x00,0x51,0x01,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0xF7,0x00,0x00,0x00,0xB9,0x00,0x00,0x00,0x2C,0x00,0x00,0x00,0x58,0x00,0x00,0x00,new BinRefTo("MeleeUnitButtonClick", false),0x11,0x00,0x00,0x00,0xF0,0x3F,0x46,0x00,0x7D,0x01,0x00,0x40,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3D,0xB8,0x54,0x00,0x3D,0xB8,0x54,0x00,0x00,0x00,0x00,0x00,0x51,0x01,0x00,0x00,0x28,0x67,0xB9,0x00,
                            0x66, 0x00, 0x00, 0x00
                        },

                        new BinAlloc("IdentityMenuButtonArrayExtreme", null)
                        {
                            0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x40,0x1E,0x49,0x00,0x00,0x00,0x00,0x00,0xD0,0x1E,0x49,0x00,0x05,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x50,0x00,0x00,0x00,0x5F,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x40,0x1E,0x49,0x00,0x15,0x00,0x00,0x00,0xD0,0x1E,0x49,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0xDA,0x00,0x00,0x00,0x5F,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x40,0x1E,0x49,0x00,0x16,0x00,0x00,0x00,0xD0,0x1E,0x49,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x50,0x00,0x00,0x00,0xB9,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x48,0x00,0x00,0x00,0x40,0x1E,0x49,0x00,0x17,0x00,0x00,0x00,0xD0,0x1E,0x49,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x99,0x00,0x00,0x00,0xB9,0x00,0x00,0x00,0x18,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0x40,0x1E,0x49,0x00,0x28,0x00,0x00,0x00,0xD0,0x1E,0x49,0x00,0xF2,0x01,0x00,0x00,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xB7,0x02,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x99,0x00,0x00,0x00,0xD6,0x00,0x00,0x00,0x18,0x00,0x00,0x00,0x1C,0x00,0x00,0x00,0x40,0x1E,0x49,0x00,0x29,0x00,0x00,0x00,0xD0,0x1E,0x49,0x00,0xF3,0x01,0x00,0x00,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xB9,0x02,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x90,0x3E,0x49,0x00,0x00,0x00,0x00,0x00,0x00,0x42,0x46,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x28,0x00,0x00,0x00,0x13,0x01,0x00,0x00,0xB4,0x00,0x00,0x00,0xB4,0x00,0x00,0x00,0x90,0x3E,0x49,0x00,0x11,0x00,0x00,0x00,0x00,0x42,0x46,0x00,0x70,0x01,0x00,0x40,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x45,0x01,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x45,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xE0,0xCD,0x47,0x00,0x00,0x00,0x00,0x00,0x70,0xCE,0x47,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0x50,0x00,0x00,0x00,0x13,0x01,0x00,0x00,0x54,0x01,0x00,0x00,0x22,0x00,0x00,0x00,0xE0,0xCD,0x47,0x00,0x04,0x00,0x00,0x00,0x70,0xCE,0x47,0x00,0x00,0x00,0x00,0x00,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0xC0,0x00,0x00,0x00,0xAB,0x00,0x00,0x00,0x2C,0x00,0x00,0x00,0x58,0x00,0x00,0x00,new BinRefTo("RangedUnitButtonClick", false),0x11,0x00,0x00,0x00,0x00,0x42,0x46,0x00,0x7C,0x01,0x00,0x40,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3D,0xB8,0x54,0x00,0x3D,0xB8,0x54,0x00,0x00,0x00,0x00,0x00,0x51,0x01,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x03,0x00,0x00,0x02,0xF7,0x00,0x00,0x00,0xB9,0x00,0x00,0x00,0x2C,0x00,0x00,0x00,0x58,0x00,0x00,0x00,new BinRefTo("MeleeUnitButtonClick", false),0x11,0x00,0x00,0x00,0x00,0x42,0x46,0x00,0x7D,0x01,0x00,0x40,0x03,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0xFF,0xFF,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x3D,0xB8,0x54,0x00,0x3D,0xB8,0x54,0x00,0x00,0x00,0x00,0x00,0x51,0x01,0x00,0x00,0xC8,0x68,0xB9,0x00,
                            0x66, 0x00, 0x00, 0x00
                        },

                        new BinAlloc("OverrideIdentityMenuRender", null)
                        {
                            PUSH(FLAGS), //  pushf
                            0xE8, new BinRefTo("IsExtremeToBool"), // call IsExtremeToBool
                            PUSH(EAX), //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            POP(EAX), //  pop eax
                            PUSH(EBX), //  push ebx
                            JMP(NOTEQUALS, 0x0D), //  jne D
                            
                            0x81, 0xFE, 0xA8, 0x13, 0x60, 0x00, //  cmp esi,"Stronghold_Crusader_Extreme.exe"+2013A8
                            0xBB, new BinRefTo("IdentityMenuButtonArrayExtreme", false), //  mov ebx,IdentityMenuButtonArrayExtreme
                            JMP(UNCONDITIONAL, 0x0B), //  jmp B
                            0x81, 0xFE, 0x98, 0x14, 0x60, 0x00, //  cmp esi,"Stronghold Crusader.exe"+201498
                            0xBB, new BinRefTo("IdentityMenuButtonArray", false), //  mov ebx,IdentityMenuButtonArray
                            0x0F, 0x85, 0x02, 0x00, 0x00, 0x00, //  jne short 2
                            
                            MOV(ESI, EBX), //  mov esi,ebx
                            
                            POP(EBX), //  pop ebx
                            POP(FLAGS), //  popf
                            0xC7, 0x43, 0x14, 0x00, 0x00, 0x00, 0x00, //  mov [ebx+14],00
                            0xC3, //  ret
                        },

                        new BinAlloc("IsExtremeToBool", null)
                        {
                            PUSH(EAX), //  push eax
                            0xB8, 0x20, 0x01, 0x40, 0x00, //  mov eax,00000120
                            0x81, 0x38, 0xBD, 0x93, 0xAF, 0x5A, //  cmp [eax],5AAF93BD
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            JMP(NOTEQUALS, 0x08), //  jne 8
                            0xC7, 0x00, 0x00, 0x00, 0x00, 0x00, //  mov [eax],00
                            POP(EAX), //  pop eax
                            0xC3, //  ret
                            0xC7, 0x00, 0x01, 0x00, 0x00, 0x00, //  mov [eax],01
                            POP(EAX), //  pop eax
                            0xC3, //  ret
                        },

                        new BinAlloc("IsExtremeBool", null)
                        {
                            0x00, 0x00, 0x00, 0x00
                        },

                        new BinHook(7)
                        {
                            0xE8, new BinRefTo("OverrideIdentityMenuRender") // jmp
                        }
                    }
                }
            },


            /*
             *   HEALER
             */

            new Change("o_healer", ChangeType.Other)
            {
                new DefaultHeader("o_healer")
                {
                    // 0040AF43
                    new BinaryEdit("random")
                    {
                        new BinAddress("random", 3),
                        new BinAddress("randomecx", 0xB),
                        new BinAddress("randomcall", 0x12, true),

                        new BinAlloc("GetRandomShort", null)
                        {
                            0xB9, new BinRefTo("randomecx", false), // mov ecx, randomecx
                            0xE8, new BinRefTo("randomcall"), // call refreshrandom
                            0x0F, 0xBF, 0x05, new BinRefTo("random", false), // movsx eax,[random]
                            0xC3, // ret
                        }
                    },

                    new BinaryEdit("o_healer_plague")
                    {
                        new BinAddress("pvar1", 0xFF),
                        new BinAddress("pvar2", 0x119),
                        new BinAddress("pvar3", 0x120),
                        new BinAddress("presult", 0x133),

                    },

                    // edi + 128 = HP
                    // edi + 12C = HPMax
               
                    // 005345A0
                    new BinaryEdit("o_healer_find")
                    {
                        new BinAddress("fvar2", 0xBB),
                        new BinAddress("distfunc", 0xC0, true),

                        new BinLabel("jesterfind"),

                        new BinAlloc("healerfind", null)
                        {
                            0x83, 0xEC, 0x08,           //   - sub esp,08 { 8 }
                            0x53,                 //   - push ebx
                            0x55,                  //  - push ebp
                            0x33, 0xC0,               //  - xor eax,eax
                            0x8B, 0xE9,               //  - mov ebp,ecx
                            0xBB, 0x01, 0x00, 0x00, 0x00,         //  - mov ebx,00000001 { 1 }
                            0x39, 0x5D, 0x00,            //  - cmp [ebp+00],ebx
                            0x89, 0x44, 0x24, 0x0C,         //  - mov [esp+0C],eax
                            0xC7, 0x44, 0x24, 0x08, 0xFF, 0xFF, 0xFF, 0x7F, //  - mov [esp+08],0x7FFFFFFF
                            0x0F, 0x8E, 0xEB, 0x00, 0x00, 0x00,       //  - jng end

                            0x56,                  //  - push esi
                            0x57,                  //  - push edi
                            0x8D, 0xBD, 0x44, 0x0D, 0x00, 0x00,      //  - lea edi,[ebp+00000D44]

                            // CHECK NPC
                            0x66, 0x83, 0xBF, 0xEC, 0xFD, 0xFF, 0xFF, 0x02, // - cmp word ptr [edi-00000214],02 { 2 }
                            0x0F, 0x85, 0x9C, 0x00, 0x00, 0x00,        // - jne 0053467F


                            // CHECK ALIVE?
                            0x66, 0x83, 0x3F, 0x00,          // - cmp word ptr [edi],00 { 0 }
                            0x0F, 0x85, 0x92, 0x00, 0x00, 0x00,        // - jne 0053467F

                            // CHECK TEAM
                            0x0F, 0xBF, 0x87, 0xF6, 0xFD, 0xFF, 0xFF,    //  - movsx eax,word ptr [edi-0000020A]
                            0x3B, 0x44, 0x24, 0x20,          // - cmp eax,[esp+20]
                            0x0F, 0x85, 0x81, 0x00, 0x00, 0x00,        // - jne 0053467F
                            
                            // CHECK HEALTH
                            0x8B, 0x87, 0x28, 0x01, 0x00, 0x00, // mov eax, [edi+128]
                            0x3B, 0x87, 0x2C, 0x01, 0x00, 0x00, // cmp eax, [edi+12C]
                            0x7D, 0x73, // jge end

                            // CHECK EXCLUDE
                            0x8B, 0x44, 0x24, 0x1C,          // mov eax, [esp+1C]
                            0x39, 0xC3,          // - cmp ebx,eax
                            0x74, 0x6B, //  - je 0053467F

                            // CHECK DISTANCE
                            0x69, 0xC0, 0x90, 0x04, 0x00, 0x00, //        - imul eax,eax,490
                            0x0F, 0xBF, 0x80, new BinRefTo("pvar1", false), // movsx, eax, word ptr [eax+pvar1]
                            0x85, 0xC0, // test eax, eax
                            0x74, 0x3B, // je skip dist check

                            // self pos
                            0x0F, 0xBF, 0x8F, 0x26, 0xFE, 0xFF, 0xFF, //  movsx ecx,word ptr [edi-000001DA]
                            0x0F, 0xBF, 0x97, 0x24, 0xFE, 0xFF, 0xFF, // movsx edx,word ptr [edi-000001DC]

                            0x51,  //                   - push ecx
                            0x52, //                    - push edx

                            0x69, 0xC0, 0x2C, 0x03, 0x00, 0x00, //        - imul eax,eax,0000032C { 812 }
                            0x0F, 0xBF, 0x90, new BinRefTo("pvar2", false), // movsx edx,word ptr [eax+00F98624]
                            0x0F, 0xBF, 0x80, new BinRefTo("pvar3", false), // - movsx eax,word ptr [eax+00F98622]
                            0x52, //                    - push edx
                            0x50, //                    - push eax
                            0xB9,  new BinRefTo("fvar2", false), // - mov ecx,00EE23BC { [00000000] }
                            0xE8, new BinRefTo("distfunc", true),         //  - call 0046CC80
                            0x8B, 0x35, new BinRefTo("presult", false), // mov esi, [presult]
                            0x83, 0xFE, 0x28, // cmp esi, 28
                            0x7F, 0x1F, //                 - jg 0040184D // too far away
                            
                            // first time ? jump over
                            0x83, 0x7C, 0x24, 0x2C, 0x00,      //  - cmp dword ptr [esp+2C],00
                            0x74, 0x0A,               //  - je 00534671

                            // continued ? add some randomness
                            0xE8, new BinRefTo("GetRandomShort"), // call getrandomshort
                            0xC1, 0xF8, 0x0C, // sar eax, C
                            0x01, 0xC6, // add esi, eax


                            0x3B, 0x74, 0x24, 0x10,         //  - cmp esi,[esp+10]
                            0x7D, 0x08,               //  - jge 0053467F
                            0x89, 0x74, 0x24, 0x10,         //  - mov [esp+10],esi
                            0x89, 0x5C, 0x24, 0x14,         //  - mov [esp+14],ebx

                            0x83, 0xC3, 0x01,            //  - add ebx,01 { 1 }
                            0x81, 0xC7, 0x90, 0x04, 0x00, 0x00,      //  - add edi,00000490 { 1168 }
                            0x3B, 0x5D, 0x00,            //  - cmp ebx,[ebp+00]
                            0x0F, 0x8C, 0x44, 0xFF, 0xFF, 0xFF,        // - jl 005345D0

                            0x8B, 0x44, 0x24, 0x14,          // - mov eax,[esp+14]
                            0x85, 0xC0,                // - test eax,eax
                            0x5F,                   // - pop edi
                            0x5E,                   // - pop esi
                            0x74, 0x1D,                // - je 005346B8
                            0x8B, 0x15, 0xA8, 0x7D, 0xFE, 0x01,       // - mov edx,[01FE7DA8] { [0000F19D] }
                            0x8B, 0xC8,                // - mov ecx,eax
                            0x69, 0xC9, 0x90, 0x04, 0x00, 0x00,       // - imul ecx,ecx,00000490 { 1168 }
                            0x89, 0x94, 0x29, 0x44, 0x09, 0x00, 0x00,    // - mov [ecx+ebp+00000944],edx
                            0x5D,                   // - pop ebp
                            0x5B,                    //- pop ebx
                            0x83, 0xC4, 0x08,              //- add esp,08 { 8 }
                            0xC2, 0x18, 0x00,               //- ret 0018 { 24 }
                            0x5D,                    //- pop ebp
                            0x33, 0xC0,                // - xor eax,eax
                            0x5B,                   // - pop ebx
                            0x83, 0xC4, 0x08,             // - add esp,08 { 8 }
                            0xC2, 0x18, 0x00,              // - ret 0018 { 24 }
                        },

                        new BinHook(5)  // hook to own func
                        {
                            0x80, 0x3D, new BinRefTo("healerbool", false), 0x01, // cmp [healerbool], 1
                            0x0F, 0x84, new BinRefTo("healerfind"), // je
                            new BinBytes(0x83, 0xEC, 0x08, 0x53, 0x55),
                        }
                    },

                    // 0x56E190
                    new BinaryEdit("o_jesterroam")
                    {
                        new BinAddress("posy", 0x178),
                        new BinAddress("posx", 0x17F),
                        new BinAddress("ecx", 0x18E),
                        new BinAddress("target", 0x1F9),
                        new BinAddress("currentroam", 0x515),
                        new BinAddress("somevar", 0x69D),
                        new BinAddress("distance", 0x4F0),
                        new BinAddress("walkspeed", 0x490),

                        new BinHook(5)
                        {
                            new BinBytes(0xC6, 0x05), new BinRefTo("healerbool", false),
                            new BinBytes(0x00, 0x51, 0x53, 0x55, 0x56, 0x57)
                        },

                        new BinSkip(0x1F1),
                        new BinLabel("findcontinue"), // 56E386

                        new BinSkip(0x201),
                        new BinLabel("walkto"), // 56E587

                        new BinSkip(0x93),
                        new BinHook(0x13) // 0056E61D // walk fix
                        {
                            0x80, 0x3D, new BinRefTo("healerbool", false), 0x01, // cmp [healerbool], 1
                            0x74, 0x05, // je
                            
                            0x66, 0x3B, 0xC5, // cmp ax,bp
                            0x7D, 0x03, // jge 

                            0x66, 0x89, 0xE8, // mov ax, bp
                            
                            0x66, 0x89, 0x86, new BinRefTo("walkspeed", false) // mov [walkspeed],ax
                        },

                        new BinSkip(0x51),
                        new BinHook(7) // 0056E67E // DISTANCE
                        {
                            0x8D, 0x43, 0x05, // lea eax, [ebx+5]
                            0x80, 0x3D, new BinRefTo("healerbool", false), 0x01, // cmp [healerbool], 1
                            0x75, 0x02, // jne
                            0x2C, 0x03, // sub al, 3
                            0x39, 0x05, new BinRefTo("distance",false) // cmp [distance], eax
                        },

                        new BinSkip(0x1D),
                        new BinHook(9) // 56E6A2 // PLAY ANIMATION
                        {
                            0x66, 0xB9, 0x04, 0x00, // mov cx, 4
                            
                            0x80, 0x3D, new BinRefTo("healerbool", false), 0x01, // cmp [healerbool], 1
                            0x75, 0x04, // jne
                            0x66, 0x83, 0xC1, 0x04, // add cx, 4

                            0x66, 0x89, 0x88, new BinRefTo("currentroam", false), // mov [eax+currentroam], cx
                            
                        },

                        new BinSkip(0x68),
                        new BinLabel("animation"), // 56E713

                        new BinSkip(0x58),
                        new BinHook(8) // 0056E76B // USE CORRECT ANIMATION
                        {
                            0xBE, 0x01, 0x00, 0x00, 0x00, // mov esi, 1
                            
                            0x80, 0x3D, new BinRefTo("healerbool", false), 0x01, // cmp [healerbool], 1
                            0x75, 0x1A, // jne
                            0x8B, 0x88, new BinRefTo("avar1", false), // mov ecx, [eax+avar1]
                            0x0F, 0xBE, 0x89, new BinRefTo("avar2", false), // movsx ecx, [ecx+avar2]
                            0xBD, 0x81, 0x00, 0x00, 0x00, // mov ebp, 81
                            0xE9, new BinRefTo("anicontinue"),

                            0x66, 0x3B, 0xCB // cmp cx, bx
                        },

                        new BinSkip(0x57),
                        new BinLabel("anicontinue"),

                        new BinSkip(0x61),
                        // healing
                        new BinHook(6) // 56E82B // DO HEAL
                        {
                            0x80, 0x3D, new BinRefTo("healerbool", false), 0x01, // cmp [healerbool], 1
                            0x75, 0x65, // jne
                            0x50, // push eax // self
                            0x52, // push edx
                            
                            // get target
                            0x0F, 0xBF, 0x88, new BinRefTo("target", false), // movsx ecx, word ptr [eax + target]
                            0x69, 0xC9, 0x90, 0x04, 0x00, 0x00, // imul ecx, 490
                            0x03, 0x4C, 0x24, 0x0C, // add ecx, [esp+C] 

                            // Get current HP data
                            0x8B, 0x81, 0xDC, 0x09, 0x00, 0x00, // mov eax, [ecx+9DC] == hp
                            0x8B, 0xA9, 0xE0, 0x09, 0x00, 0x00, // mov ebp, [ecx+9E0] == max hp

                            // Increase HP 
                            0x05, 0xB8, 0x0B, 0x00, 0x00, // add eax, BB8
                            0x39, 0xE8, // cmp eax, ebp
                            0x7E, 0x02, // jle
                            0x8B, 0xC5, // mov eax, ebp
                            0x89, 0x81, 0xDC, 0x09, 0x00, 0x00, // mov [ecx+9DC], eax

                            // get some percentage value
                            0x85, 0xED, // test ebp, ebp
                            0x75, 0x07, //                 - jne 005320D1
                            0xB8, 0x64, 0x00, 0x00, 0x00, //           - mov eax,00000064 { 100 }
                            0xEB, 0x0D, //                 - jmp 005320DE
                            0x6B, 0xC0, 0x64, //              - imul eax,eax,64
                            0x99, //                    - cdq
                            0xF7, 0xFD, //                 - idiv ebp

                            // update health bar
                            0x66, 0x89, 0x81, 0xE8, 0x08, 0x00, 0x00, // mov [ecx+8E8], ax
                            0x0F, 0xBF, 0xE8, //               - movsx ebp,ax
                            0xB8, 0x67, 0x66, 0x66, 0x66, //           - mov eax,66666667
                            0xF7, 0xED, //                 - imul ebp
                            0xC1, 0xFA, 0x02, //              - sar edx,02
                            0x8B, 0xC2, //                 - mov eax,edx
                            0xC1, 0xE8, 0x1F, //              - shr eax,1F
                            0x01, 0xD0, //                 - add eax,edx
                            0x66, 0x89, 0x81, 0x4C, 0x06, 0x00, 0x00, //  - mov [ecx+0000064C],ax

                            0x5A, // pop edx
                            0x58, // pop eax                            
                            0x89, 0x98, new BinRefTo("somevar", false), // ori code
                        },

                        new BinSkip(0x46),
                        new BinHook(9) // 56E877 // WALK TO TARGET
                        {
                            0x66, 0xBB, 0x03, 0x00, // mov bx, 3
                            
                            0x80, 0x3D, new BinRefTo("healerbool", false), 0x01, // cmp [healerbool], 1
                            0x75, 0x04, // jne
                            0x66, 0x83, 0xC3, 0x04, // add bx, 4

                            0x66, 0x89, 0x99, new BinRefTo("currentroam", false), // mov [ecx+currentroam], bx
                        },

                    },
              
                    // 00540440
                    new BinaryEdit("o_healerroam")
                    {
                        new BinAlloc("healerbool", 1),

                        new BinAddress("nextroam", 0x31B),
                        new BinAddress("avar1", 0x7B4),
                        new BinAddress("avar2", 0x7BB),

                        new BinHook(5)
                        {
                            new BinBytes(0xC6, 0x05), new BinRefTo("healerbool", false),
                            new BinBytes(0x01, 0x51, 0x53, 0x55, 0x56, 0x57)
                        },

                        new BinSkip(0x30B),

                        new BinHook(0x11) // 540750 // FIND HURT PERSON
                        {
                            0x52, // push edx
                            0x8B, 0xF2, // mov esi, edx
                            0x69, 0xF6, 0x90, 0x04, 0x00, 0x00, // imul esi, 490
                            0x0F, 0xBF, 0x86, new BinRefTo("posy", false), //     - movsx eax,word ptr [esi+01388612]
                            0x0F, 0xBF, 0x8E, new BinRefTo("posx", false), //    - movsx ecx,word ptr [esi+01388610]
                            0x8B, 0x6C, 0x24, 0x14, //           - mov ebp,[esp+14]
                            0x53, //                    - push ebx
                            0x53, //                    - push ebx
                            0x50, //                    - push eax
                            0x51, //                    - push ecx
                            0x55, //                    - push ebp
                            0x57, //                    - push edi
                            0xB9, new BinRefTo("ecx", false), //          - mov ecx,01387F38 { [000009C4] }
                            0xE8, new BinRefTo("healerfind"), //          - call findunit
                            0x5A, // pop edx
                       
                            0x85, 0xC0, // test eax, eax
                            0x74, 0x1A, // je

                            0x8B, 0xF0, // mov esi, eax
                            0x8B, 0xC2, //                 - mov eax,edx
                            0x8B, 0xCA, //                 - mov ecx,edx
                            0x69, 0xC0, 0x90, 0x04, 0x00, 0x00, //        - imul eax,eax,00000490 { 1168 }
                            0x66, 0xC7, 0x80, new BinRefTo("nextroam",false), 0x07, 0x00, // - mov word ptr [eax+0138880E],0007 { 7 }
                            0xE9, new BinRefTo("findcontinue"),


                            0x8B, 0xC2,               //  - mov eax,edx
                            0x69, 0xC0, 0x90, 0x04, 0x00, 0x00,       // - imul eax,eax,00000490 { 1168 }
                            0x66, 0xC7, 0x80, new BinRefTo("nextroam", false), 0x04, 0x00, // - mov word ptr [eax+0138880E],0004 { 4 }
                        },

                        new BinSkip(0x10B),


                        new BinHook("othercmp", 0x0F, 0x85) // 54086C // JUMPS TO JESTER
                        {
                            0x66, 0x83, 0xFA, 0x07, // cmp dx, 7
                            0x0F, 0x84, new BinRefTo("walkto"), // // je to walk
                            0x66, 0x83, 0xFA, 0x08, // cmp dx, 8
                            0x0F, 0x84, new BinRefTo("animation"),// // je to animation
                        },

                        new BinSkip(0xFB),
                        new BinLabel("othercmp"), // 54096D
                    },

                }
            },






            /* 
             *  FREE TRADER POST
             */
             
            // trader post: runtime 01124EFC
            // 005C23D8
            BinBytes.Change("o_freetrader", ChangeType.Other, true, 0x00),


            /*
             * SIEGE EQUIPMENT BUILDING
             */

            // 0044612B
            // nop out: mov [selection], ebp = 0
            BinBytes.Change("o_engineertent", ChangeType.Other, true, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90),

            /*
             *  MOAT VISIBILITY
             */
             
            new Change("o_moatvisibility", ChangeType.Other)
            {
                new DefaultHeader("o_moatvisibility")
                {
                    // 4EC86C
                    new BinaryEdit("o_moatvisibility")
                    {
                        new BinSkip(0x24),
                        new BinBytes(0x15) // mov [ ], edx = 1 instead of ebp = 0
                    }
                }
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
                        CMP(EAX, 10000),      // cmp eax, 10000

                        JMP(GREATERTHANEQUALS, 0x19), // jge to end

                        MOV(EDI, EAX),              // mov edi, eax

                        CMP(EAX, 200),     // cmp eax, 200
                        new BinHook("label1", 0x0F, 0x8C)                // jl hook
                        {
                            CMP(EAX, 90), // cmp eax, 90
                            JMP(LESS, 0x03),       // jl to end
                            ADD(EDI, 5), // add edi, 5
                        },
                        ADD(EDI, 0x5F),        // add edi, 95
                        new BinLabel("label1"),
                        ADD(EDI, 5),       // add edi, 5
                        JMP(UNCONDITIONAL, 0x75),              // jmp to gamespeed_down set value
                        new BinBytes(0x90, 0x90, 0x90, 0x90),
                    },

                    // 004B47C2
                    new BinaryEdit("o_gamespeed_down")
                    {
                        JMP(LESSTHANEQUALS, 0x1B), // jle to end

                        MOV(EDI, EAX),              // mov edi, eax

                        CMP(EAX, 200),     // cmp eax, 200
                        new BinHook("label2", 0x0F, 0x8E)                // jle hook
                        {
                            CMP(EAX, 0x5A), // cmp eax, 90
                            JMP(LESSTHANEQUALS, 0x03),       // jle to end
                            SUB(EDI, 0x05), // sub edi, 5
                        },
                        SUB(EDI, 0x5F),        // sub edi, 95
                        new BinLabel("label2"),
                        SUB(EDI, 5),        // sub edi, 5
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
             * ONLY AI / SPECTATOR MODE
             */

            new Change("o_onlyai", ChangeType.Other, false)
            {
                new DefaultHeader("o_onlyai")
                {   
                    // reset player list
                    // 0048F919
                    new BinaryEdit("o_onlyai_reset")
                    {
                        new BinAddress("selfindex", 2),
                        new BinAddress("selfai", 8),

                        new BinHook(12)
                        {
                            XOR(EAX, EAX), // xor eax, eax
                            0xA3, new BinRefTo("selfindex", false),

                            SUB(EAX, 1), // sub eax, 1
                            0xA3, new BinRefTo("selfai", false),
                        }
                    },

                    // game start
                    // 0048F96C => je to jmp to almost end
                    BinBytes.CreateEdit("o_onlyai", 0xE9, 0x09, 0x01, 0x00, 0x00, 0x90),
                    
                    // loading
                    // 004956FB
                    new BinaryEdit("o_onlyai_load1")
                    {
                        //  => mov [selfindex], eax   to   mov [selfindex], ebx = 0
                        new BinBytes(0x90, 0x90, 0x90, 0x89, 0x1D),
                        new BinSkip(5),
                        new BinBytes(0x3C) // mov ..., ebp  => mov ..., edi
                    },

                    // missing in 1.3
                    // after loading, hide buildings menu
                    // 0046B3FA => mov ecx, [selfindex]   to   xor ecx, ecx
                    //BinBytes.CreateEdit("o_onlyai_load2", 0x31, 0xC9, 0x90, 0x90, 0x90, 0x90),

                    // happy face :)
                    // 0x4334A6
                    BinBytes.CreateEdit("o_onlyai_face", 0xB9, 0xD3, 0x13, 0x00, 0x00, 0x90),

                    // show assassins
                    // 004EA265
                    BinBytes.CreateEdit("o_onlyai_assassins", 0xEB), // change je to jmp
                }
            },
            
            
            // Armory / Marketplace weapon order fix
            new Change("o_armory_marketplace_weapon_order_fix", ChangeType.Other, false)
            {
                new DefaultHeader("o_armory_marketplace_weapon_order_fix")
                {

                    new BinaryEdit("o_armory_marketplace_weapon_order_fix1") // 217F50
                    {
                        // Armory item ID order
                        new BinBytes(0x11, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00),
                        // Armory item image ID
                        new BinBytes(0x4C, 0x00, 0x00, 0x00, 0x50, 0x00, 0x00, 0x00, 0x54, 0x00, 0x00, 0x00, 0x58, 0x00, 0x00, 0x00, 0x4E, 0x00, 0x00, 0x00, 0x52, 0x00, 0x00, 0x00, 0x5A, 0x00, 0x00, 0x00, 0x56, 0x00, 0x00, 0x00),
                        // Armory item image offset
                        new BinBytes(0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFC, 0xFF, 0xFF, 0xFF, 0x04, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFC, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0xFE, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00)
                    },
                    new BinaryEdit("o_armory_marketplace_weapon_order_fix2") // 6B90E8
                    {
                        // Marketplace item order
                        new BinBytes(0x11, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00)
                    },
                    new BinaryEdit("o_armory_marketplace_weapon_order_fix3") // 7343C0
                    {
                        // Marketplace image order
                        new BinBytes(0x50, 0x00, 0x00, 0x00),
                        new BinSkip(52),
                        new BinBytes(0x4C, 0x00, 0x00, 0x00),
                        new BinSkip(80),
                        new BinBytes(0x5A, 0x00, 0x00, 0x00),
                        new BinSkip(52),
                        new BinBytes(0x56, 0x00, 0x00, 0x00)
                    },
                    new BinaryEdit("o_armory_marketplace_weapon_order_fix4") // 218050
                    {
                        // Swap marketplace trade weapons item count references
                        new BinBytes(0x11, 0x00, 0x00, 0x00, 0x13, 0x00, 0x00, 0x00, 0x15, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0x14, 0x00, 0x00, 0x00, 0x18, 0x00, 0x00, 0x00, 0x16, 0x00, 0x00, 0x00)
                    },
                    new BinaryEdit("o_armory_marketplace_weapon_order_fix5") // 1FD8B0
                    {
                        // Fix marketplace item order
                        new BinBytes(0x11, 0x00, 0x00, 0x00),
                        new BinSkip(76),
                        new BinBytes(0x13, 0x00, 0x00, 0x00),
                        new BinSkip(396),
                        new BinBytes(0x18, 0x00, 0x00, 0x00),
                        new BinSkip(76),
                        new BinBytes(0x16, 0x00, 0x00, 0x00)
                    }

                }
            },

            new Change("o_increase_path_update_tick_rate", ChangeType.Other, true)
            {
                new DefaultHeader("o_increase_path_update_tick_rate")
                {
                    // 499605
                    new BinaryEdit("o_increase_path_update_tick_rate")
                    {
                        new BinSkip(25),
                        new BinBytes(0x32)
                    },
                },
            },

            new Change("o_default_multiplayer_speed", ChangeType.Other)
            {
                new SliderHeader("o_default_multiplayer_speed", true, 20, 90, 1, 40, 50)
                {
                    // 878FB
                    new BinaryEdit("o_default_multiplayer_speed")
                    {
                        new BinSkip(16),
                        new BinByteValue()
                    },
                    new BinaryEdit("o_default_multiplayer_speed_reset")
                    {
                        new BinSkip(6),
                        new BinByteValue()
                    }
                }
            },

            new Change("o_shfy", ChangeType.Other, false, false)
            {
                new DefaultHeader("o_shfy_beer", false)
                    // fixes beer popularity bonus
                    {
                        new BinaryEdit("o_shfy_beerpopularity")
                        {
                            new BinSkip(21),
                            new BinBytes(0xB8, 0x19, 0x00, 0x00, 0x00),
                            new BinSkip(7),
                            new BinBytes(0xB8, 0x32, 0x00, 0x00, 0x00),
                            new BinSkip(13),
                            new BinBytes(0x83, 0xE2, 0x9C, 0x83, 0xC2, 0x64, 0x90, 0x90, 0x90)
                        },

                        new BinaryEdit("o_shfy_beerpopularitytab")
                        {
                            new BinSkip(14),
                            new BinBytes(0xBE, 0x19, 0x00, 0x00, 0x00),
                            new BinSkip(7),
                            new BinBytes(0xBE, 0x32, 0x00, 0x00, 0x00),
                            new BinSkip(13),
                            new BinBytes(0x83, 0xE1, 0xE7, 0x83, 0xC1, 0x64, 0x90, 0x90, 0x90)
                        },

                        new BinaryEdit("o_shfy_beertab")
                        {
                            new BinSkip(14),
                            new BinBytes(0xB8, 0x19, 0x00, 0x00, 0x00),
                            new BinSkip(7),
                            new BinBytes(0xB8, 0x32, 0x00, 0x00, 0x00),
                            new BinSkip(13),
                            new BinBytes(0x83, 0xE0, 0xE7, 0x83, 0xC0, 0x64, 0x90, 0x90)
                        }
                    },

                new DefaultHeader("o_shfy_religion", false)
                    // fixes religion popularity bonus
                    {
                        new BinaryEdit("o_shfy_religionpopularity")
                        {
                            new BinSkip(9),
                            new BinBytes(0x83, 0xC1, 0x00),
                            new BinSkip(9),
                            new BinBytes(0x83, 0xC1, 0x00),
                        },

                        new BinaryEdit("o_shfy_religionpopularitytab")
                        {
                            new BinSkip(9),
                            new BinBytes(0x83, 0xC6, 0x00),
                            new BinSkip(9),
                            new BinBytes(0x83, 0xC6, 0x00),
                        },

                        new BinaryEdit("o_shfy_religiontab")
                        {
                            JMP(UNCONDITIONAL, 0x6D),
                            new BinSkip(132),
                            new BinBytes(0xB9, 0x00, 0x00, 0x00, 0x00),
                            new BinSkip(9),
                            new BinBytes(0x83, 0xC1, 0x00)
                        }
                    },

                new DefaultHeader("o_shfy_peasantspawnrate", false)
                    // fixes peasant spawnrate
                    {
                        new BinaryEdit("o_shfy_peasantspawnrate")
                        {
                            new BinSkip(8),
                            new BinBytes(0x83, 0xFB, 0x00, 0x90, 0x90, 0x90),
                            new BinSkip(8),
                            new BinBytes(0x83, 0xFB, 0x00, 0x90, 0x90, 0x90)
                        }
                    },

                new DefaultHeader("o_shfy_resourcequantity", false)
                    // fixes resource quantity
                    {
                        new BinaryEdit("o_shfy_resourcequantity")
                        {
                            new BinBytes(0x83, 0xC0, 0x00)
                        },
                    }
            },
            
            // 4FA620
            new Change("fix_apple_orchard_build_size", ChangeType.Other)
            {
                new DefaultHeader("fix_apple_orchard_build_size")
                {
                    new BinaryEdit("fix_apple_orchard_build_size")
                    {
                        new BinSkip(16),
                        0x0A // this is not the size, it is the ID in the switch case!
                    }
                }
            },
            
            new Change("o_seed_modification_possibility_title", ChangeType.Other, false)
            {
                new DefaultHeader("o_seed_modification_possibility_only_set", true)
                {
                    // 004964AB
                    new BinaryEdit("o_seed_modification_possibility_fn1")
                    {
                        new BinAddress("_fopen", 1, true),
                        new BinAddress("_fclose", 972, true),
                    },
                    
                    // 0046C381
                    new BinaryEdit("o_seed_modification_possibility_fn3")
                    {
                        new BinAddress("_internalFileRead", 10, true),
                    },
                    
                    // 00588FF2
                    new BinaryEdit("o_seed_modification_possibility_fn4")
                    {
                        new BinAddress("_atol", 2, true),
                    },
                    
                    // 0046A764
                    new BinaryEdit("o_seed_modification_possibility")
                    {
                        new BinAlloc("isInited", null)
                        {
                            0x00,
                        },
                        
                        new BinAlloc("liveSeedFile", null)
                        {
                            0x67, 0x61, 0x6D, 0x65, 0x73, 0x65, 0x65, 0x64, 0x73, 0x2F,
                            0x6C, 0x69, 0x76, 0x65, 0x00
                        },
                        
                        new BinAlloc("readTextFlag", null)
                        {
                            0x72, 0x00
                        },
                        
                        new BinAlloc("readSeedString", null)
                        {
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        },
                        
                        new BinAlloc("loadedSeed", null)
                        {
                           0x00, 0x00, 0x00, 0x00
                        },
                        
                        new BinAlloc("cachedSeed", null)
                        {
                           0x00, 0x00, 0x00, 0x00
                        },
                         // -- end of alloc
                        
                        new BinSkip(4), // skip 4
                        new BinHook(5)
                        {
                            0x60, // pushad
                            0x83, 0x3D, new BinRefTo("isInited", false), 00, // cmp [isInited],00
                            0x75, 0x0C, // jne short 0C
                            0xC7, 0x05, new BinRefTo("isInited", false), 0x01, 0x00, 0x00, 0x00, // mov [isInited],01
                            0xEB, 0x6A, // jmp short 6A
                            
                            0x8B, 0x4E, 0x04, // mov ecx,[esi+04]
                            0x89, 0x0D, new BinRefTo("cachedSeed", false), // mov [cachedSeed],ecx
                            
                            0x31, 0xC9, // xor ecx,ecx
                            // -- loop
                            0xC7, 0x81, new BinRefTo("readSeedString", false), 0x00, 0x00, 0x00, 0x00, // mov [readSeedString+ecx],00
                            0x41, // inc ecx
                            0x81, 0xF9, 0x0A, 0x00, 0x00, 0x00, // cmp ecx,0A
                            0x75, 0xED, // jne short ED (backwards)
                            
                            0x68, new BinRefTo("readTextFlag", false), // push readTextFlag
                            0x68, new BinRefTo("liveSeedFile", false), // push liveSeedFile
                            0xE8, new BinRefTo("_fopen", true), // call _fopen
                            0x8B, 0xF0, // mov esi,eax
                            0x81, 0xC4, 0x08, 0x00, 0x00, 0x00, // add esp,08
                            0x83, 0xFE, 0x00, // cmp esi,00
                            0x74, 0x43, // je short 46
                            
                            0x56, // push esi
                            0x68, 0x01, 0x00, 0x00, 0x00, // push 01
                            0x68, 0x0A, 0x00, 0x00, 0x00, // push 0A
                            0x68, new BinRefTo("readSeedString", false), // push readSeedString
                            0xE8, new BinRefTo("_internalFileRead", true), // call _internalFileRead
                            
                            0x68, new BinRefTo("readSeedString", false), // push readSeedString
                            0xE8, new BinRefTo("_atol", true), // call _atol
                            
                            0xA3, new BinRefTo("loadedSeed", false), // mov [loadedSeed],eax
                            
                            0x56, // push esi
                            0xE8, new BinRefTo("_fclose", true), // call _fclose
                            0x81, 0xC4, 0x18, 0x00, 0x00, 0x00, // add esp,18
                            0x61, // popad
                            
                            0x53, // push ebx
                            0x8B, 0x1D, new BinRefTo("loadedSeed", false), // mov ebx,[loadedSeed]
                            0x89, 0x5E, 0x04, // mov [esi+04],ebx
                            0x5B, // pop ebx
                            0x8B, 0x46, 0x04, // mov eax,[esi+04]
                            0x57, // push edi
                            0x50, // push eax
                            0xEB, 0x06, // jmp end
                            // -- end of Read Set Seed
                            
                            0x61, // popad
                            
                            0x8B, 0x46, 0x04, // mov eax,[esi+04]
                            0x57, // push edi
                            0x50, // push eax
                        },
                    }
                },
                new DefaultHeader("o_seed_modification_possibility", false)
                {
                    // 004964AB
                    new BinaryEdit("o_seed_modification_possibility_fn1")
                    {
                        new BinAddress("_fopen", 1, true),
                        new BinAddress("_fwrite", 59, true),
                        new BinAddress("_fclose", 972, true),
                    },
                    
                    // 00592CA6
                    new BinaryEdit("o_seed_modification_possibility_fn2")
                    {
                        new BinAddress("_itoa", 13, true),
                    },
                    
                    // 0046C381
                    new BinaryEdit("o_seed_modification_possibility_fn3")
                    {
                        new BinAddress("_internalFileRead", 10, true),
                    },
                    
                    // 00588FF2
                    new BinaryEdit("o_seed_modification_possibility_fn4")
                    {
                        new BinAddress("_atol", 2, true),
                    },
                    
                    // 00588E4B
                    new BinaryEdit("o_seed_modification_possibility_fn5")
                    {
                        new BinAddress("_strlen", 2, true),
                    },
                    
                    // 0046A764
                    new BinaryEdit("o_seed_modification_possibility")
                    {
                        new BinAlloc("isInited", null)
                        {
                            0x00,
                        },
                        
                        new BinAlloc("needSeedSave", null)
                        {
                            0x01
                        },
                        
                        new BinAlloc("seedFolder", null)
                        {
                            0x67, 0x61, 0x6D, 0x65, 0x73, 0x65, 0x65, 0x64, 0x73, 0x2F, 0x00
                        },
                        
                        new BinAlloc("liveSeedFile", null)
                        {
                            0x67, 0x61, 0x6D, 0x65, 0x73, 0x65, 0x65, 0x64, 0x73, 0x2F,
                            0x6C, 0x69, 0x76, 0x65, 0x00
                        },
                        
                        new BinAlloc("readTextFlag", null)
                        {
                            0x72, 0x00
                        },
                        
                        new BinAlloc("writeTextFlag", null)
                        {
                            0x77, 0x00
                        },
                        
                        new BinAlloc("readSeedString", null)
                        {
                            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        },
                        
                        new BinAlloc("saveSeedStringBuffer", null)
                        {
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                           0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                        },
                        
                        new BinAlloc("loadedSeed", null)
                        {
                           0x00, 0x00, 0x00, 0x00
                        },
                        
                        new BinAlloc("cachedSeed", null)
                        {
                           0x00, 0x00, 0x00, 0x00
                        },
                         // -- end of alloc
                        
                        new BinSkip(4), // skip 4
                        new BinHook(5)
                        {
                            0x60, // pushad
                            0x83, 0x3D, new BinRefTo("isInited", false), 00, // cmp [isInited],00
                            0x75, 0x0C, // jne short 0C
                            0xC7, 0x05, new BinRefTo("isInited", false), 0x01, 0x00, 0x00, 0x00, // mov [isInited],01
                            0xEB, 0x6A, // jmp short 6A
                            
                            0x8B, 0x4E, 0x04, // mov ecx,[esi+04]
                            0x89, 0x0D, new BinRefTo("cachedSeed", false), // mov [cachedSeed],ecx
                            
                            0x31, 0xC9, // xor ecx,ecx
                            // -- loop
                            0xC7, 0x81, new BinRefTo("readSeedString", false), 0x00, 0x00, 0x00, 0x00, // mov [readSeedString+ecx],00
                            0x41, // inc ecx
                            0x81, 0xF9, 0x0A, 0x00, 0x00, 0x00, // cmp ecx,0A
                            0x75, 0xED, // jne short ED (backwards)
                            
                            0x68, new BinRefTo("readTextFlag", false), // push readTextFlag
                            0x68, new BinRefTo("liveSeedFile", false), // push liveSeedFile
                            0xE8, new BinRefTo("_fopen", true), // call _fopen
                            0x8B, 0xF0, // mov esi,eax
                            0x81, 0xC4, 0x08, 0x00, 0x00, 0x00, // add esp,08
                            0x83, 0xFE, 0x00, // cmp esi,00
                            0x74, 0x46, // je short 46
                            
                            0x56, // push esi
                            0x68, 0x01, 0x00, 0x00, 0x00, // push 01
                            0x68, 0x0A, 0x00, 0x00, 0x00, // push 0A
                            0x68, new BinRefTo("readSeedString", false), // push readSeedString
                            0xE8, new BinRefTo("_internalFileRead", true), // call _internalFileRead
                            
                            0x68, new BinRefTo("readSeedString", false), // push readSeedString
                            0xE8, new BinRefTo("_atol", true), // call _atol
                            
                            0xA3, new BinRefTo("loadedSeed", false), // mov [loadedSeed],eax
                            
                            0x56, // push esi
                            0xE8, new BinRefTo("_fclose", true), // call _fclose
                            0x81, 0xC4, 0x18, 0x00, 0x00, 0x00, // add esp,18
                            0x61, // popad
                            
                            0x53, // push ebx
                            0x8B, 0x1D, new BinRefTo("loadedSeed", false), // mov ebx,[loadedSeed]
                            0x89, 0x5E, 0x04, // mov [esi+04],ebx
                            0x5B, // pop ebx
                            0x8B, 0x46, 0x04, // mov eax,[esi+04]
                            0x57, // push edi
                            0x50, // push eax
                            0xE9, 0xAB, 0x00, 0x00, 0x00, // jmp end
                            // -- end of Read Set Seed
                            
                            
                            
                            
                            0x61, // popad
                            0x80, 0x3D, new BinRefTo("needSeedSave", false), 01, // cmp byte ptr[needSeedSave],01
                            0x0F, 0x85, 0x98, 0x00, 0x00, 0x00, // jne short 98
                            0x60, // pushad
                            
                            0x31, 0xC9, // xor ecx,ecx
                            // -- loop
                            0xC7, 0x81, new BinRefTo("saveSeedStringBuffer", false), 0x00, 0x00, 0x00, 0x00, // mov [saveSeedStringBuffer+ecx],00
                            0x41, // inc ecx
                            0x81, 0xF9, 0x3D, 0x00, 0x00, 0x00, // ecx,3D
                            0x75, 0xED, // jne short ED (backwards)
                            
                            0x31, 0xC9, // xor ecx,ecx
                            0x31, 0xDB, // xor ebx,ebx
                            // -- loop
                            0x8A, 0x99, new BinRefTo("seedFolder", false), // mov bl,byte ptr[seedFolder+ecx]
                            0x88, 0x99, new BinRefTo("saveSeedStringBuffer", false), // mov [saveSeedStringBuffer+ecx],bl
                            0x41, // inc ecx
                            0x84, 0xDB, // test bl,bl
                            0x75, 0xEF, // jne short EF (backwards)
                            
                            0x49, // dec ecx
                            0x8D, 0x81, new BinRefTo("saveSeedStringBuffer", false), // lea eax,[saveSeedStringBuffer+ecx]
                            0x68, 0x0A, 0x00, 0x00, 0x00, // push 0A
                            0x68, 0x10, 0x00, 0x00, 0x00, // push 10
                            0x50, // push eax
                            0xFF, 0x35, new BinRefTo("cachedSeed", false), // push [cachedSeed]
                            0xE8, new BinRefTo("_itoa", true), // call _itoa
                            0x8B, 0x44, 0x24, 0x04, // mov eax,[esp+04]
                            0xA3, new BinRefTo("loadedSeed", false), // mov [loadedSeed],eax
                            0x81, 0xC4, 0x10, 0x00, 0x00, 0x00, // add esp,10
                            
                            0x68, new BinRefTo("writeTextFlag", false), // push writeTextFlag
                            0x68, new BinRefTo("saveSeedStringBuffer", false), // push saveSeedStringBuffer
                            0xE8, new BinRefTo("_fopen", true), // call _fopen
                            0x8B, 0xF0, // mov esi,eax
                            0x81, 0xC4, 0x08, 0x00, 0x00, 0x00, // add esp,08
                            
                            0xFF, 0x35, new BinRefTo("loadedSeed", false), // push [loadedSeed]
                            0xE8, new BinRefTo("_strlen", true), // call _strlen
                            
                            0x56, // push esi
                            0x68, 0x01, 0x00, 0x00, 0x00, // push 01
                            0x50, // push eax
                            0xFF, 0x35, new BinRefTo("loadedSeed", false), // push [loadedSeed]
                            0xE8, new BinRefTo("_fwrite", true), // call _fwrite
                            
                            0x56, // push esi
                            0xE8, new BinRefTo("_fclose", true), // call _fclose
                            
                            0x81, 0xC4, 0x18, 0x00, 0x00, 0x00, // add esp,18
                            0x61, // popad
                            
                            0x8B, 0x46, 0x04, // mov eax,[esi+04]
                            0x57, // push edi
                            0x50, // push eax
                        },
                    }
                }
            },

            new Change("o_change_siege_engine_spawn_position_catapult", ChangeType.Other, false)
            {
                new DefaultHeader("o_change_siege_engine_spawn_position_catapult")
                {
                    // 41F4A2
                    new BinaryEdit("o_change_siege_engine_spawn_position_catapult")
                    {
                        new BinAddress("yPositionAddress", 43),
                        new BinSkip(40),
                        new BinHook(7)
                        {
                            0x0F, 0xBF, 0x80, new BinRefTo("yPositionAddress", false),
                            0x40, // inc eax
                            0x42, // inc edx
                        }
                    },
                    
                    // 41F8AF
                    new BinaryEdit("o_change_siege_engine_spawn_position_trebutchet")
                    {
                        new BinSkip(40),
                        new BinHook(7)
                        {
                            0x0F, 0xBF, 0x80, new BinRefTo("yPositionAddress", false),
                            0x40, // inc eax
                            0x42, // inc edx
                        }
                    },
                    
                    // 41FD7F
                    new BinaryEdit("o_change_siege_engine_spawn_position_tower")
                    {
                        new BinSkip(40),
                        new BinHook(7)
                        {
                            0x0F, 0xBF, 0x80, new BinRefTo("yPositionAddress", false),
                            0x40, // inc eax
                            0x42, // inc edx
                        }
                    },
                    
                    // 42020F
                    new BinaryEdit("o_change_siege_engine_spawn_position_ram")
                    {
                        new BinSkip(40),
                        new BinHook(7)
                        {
                            0x0F, 0xBF, 0x80, new BinRefTo("yPositionAddress", false),
                            0x40, // inc eax
                            0x42, // inc edx
                        }
                    },
                    
                    // 42069F
                    new BinaryEdit("o_change_siege_engine_spawn_position_shield")
                    {
                        new BinSkip(37),
                        new BinHook(7)
                        {
                            0x0F, 0xBF, 0x80, new BinRefTo("yPositionAddress", false),
                            0x40, // inc eax
                            0x42, // inc edx
                        }
                    },
                    
                    // 41E332
                    new BinaryEdit("o_change_siege_engine_spawn_position_fireballista")
                    {
                        new BinSkip(40),
                        new BinHook(7)
                        {
                            0x0F, 0xBF, 0x80, new BinRefTo("yPositionAddress", false),
                            0x40, // inc eax
                            0x42, // inc edx
                        }
                    },
                }
            },

            new Change("o_stop_player_keep_rotation", ChangeType.Other, false)
            {
                new DefaultHeader("o_stop_player_keep_rotation")
                {
                    // 4ECF93
                    new BinaryEdit("o_stop_player_keep_rotation_get_preferred_relative_orientation")
                    {
                        new BinAddress("getPreferredRelativeOrientationHandle", 12),
                        new BinAddress("getPreferredRelativeOrientation", 23, true)
                    },
                    
                    // 441D3F
                    new BinaryEdit("o_stop_player_keep_rotation")
                    {
                        new BinSkip(32),
                        new BinAddress("originalFun", 1, true),
                        new BinHook(5)
                        {
                            0x57, // push edi
                            0x50, // push eax
                            0x51, // push ecx
                            0x68, 0xC8, 0x00, 0x00, 0x00, // push C8
                            0x68, 0xC8, 0x00, 0x00, 0x00, // push C8
                            0x57, // push edi
                            0x50, // push eax
                            0xB9, new BinRefTo("getPreferredRelativeOrientationHandle", false), // mov ecx,getPreferredRelativeOrientationHandle
                            0xE8, new BinRefTo("getPreferredRelativeOrientation", true), // call getPreferredRelativeOrientation
                            0xB8, new BinRefTo("getPreferredRelativeOrientationHandle", false), // mov eax,getPreferredRelativeOrientationHandle
                            0x05, 0x10, 0x00, 0x00, 0x00, // add eax,10
                            0x8B, 0x00, // mov eax,[eax]
                            
                            0x25, 0xFE, 0xFF, 0x00, 0x00, // and eax,0000FFFE
                            
                            0x3D, 0x06, 0x00, 0x00, 0x00, // cmp eax,00000006
                            0x74, 0x09, // je short 9
                            0x3D, 0x02, 0x00, 0x00, 0x00, // cmp eax,02
                            0x74, 0x09, // je short 9
                            0xEB, 0x0C, // jmp short C
                            0xB8, 0x02, 0x00, 0x00, 0x00, // mov eax,02
                            0xEB, 0x05, // jmp short 5
                            0XB8, 0x06, 0x00, 0x00, 0x00, // mov eax,06
                            
                            0x89, 0x44, 0x24, 0x20, // mov [esp+20],eax
                            0x59, // pop ecx
                            0x58, // pop eax
                            0x5F, // pop edi
                            0xE8, new BinRefTo("originalFun"), // call originalFun
                        }
                    }
                }
            }

            #endregion
        };

        public static List<Change> Changes { get { return changes; } }

        /// <summary>
        /// Load changes stored in submodules
        /// </summary>
        public static void AddExternalChanges()
        {
            Version.changes.AddRange(ResourceChange.changes);
            Version.changes.AddRange(AIVChange.changes);
            Version.changes.AddRange(StartTroopChange.changes);
        }

        /// <summary>
        /// Remove all changes of a specified type
        /// </summary>
        /// <param name="type"></param>
        public static void RemoveChanges(ChangeType type)
        {
            changes.RemoveAll(x => x.Type == type);
        }
    }
}
