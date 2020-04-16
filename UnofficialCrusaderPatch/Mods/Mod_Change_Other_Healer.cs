namespace UCP.Patching
{
    /**
     * APOTHECARY HEALS UNITS
     */
    public class Mod_Change_Other_Healer : Mod
    {
        override public Change getChange()
        {
            return new Change("o_healer", ChangeType.Other)
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
            };
        }
    }
}