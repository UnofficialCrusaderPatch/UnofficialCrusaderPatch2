namespace UCP.Patching
{
    /**
     * CHANGE PLAYER 1 COLOR
     */
    public class Mod_Change_Other_PlayerColorChange : Mod
    {
        override public Change getChange()
        {
            return new Change("o_playercolor", ChangeType.Other, false)
            {
                new ColorHeader("o_playercolor")
                {
                    // ident doesn't matter, we just need a location to allocate it
                    new BinaryEdit("o_playercolor_ai_allied_menu_emblem")
                    {
                        new BinAlloc("PlayerColorChosen", null, true)
                        {
                            new BinByteValue(), 0x00, 0x00, 0x00
                        },
                        new BinAlloc("PlayerColorChosenFactor4", null, true)
                        {
                            new BinByteValue(4), 0x00, 0x00, 0x00
                        },
                        new BinAlloc("PlayerColorChosenMinusOne", null, true)
                        {
                            new BinByteValue(offset:-1), 0x00, 0x00, 0x00
                        },
                    },
                    
                    #region Round Table

                    // 004AF3D0
                    BinHook.CreateEdit("o_playercolor_table_drag", 6,
                        0x8B, 0xC5, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x8D, 0x80, 0x22, 0x02, 0x00, 0x00 //  lea eax, [EAX + 222]
                    ),   
                    
                    // 004AEFE9
                    BinHook.CreateEdit("o_playercolor_table_back", 6,
                        0x89, 0xF0, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x8D, 0x90, 0x22, 0x02, 0x00, 0x00 //  lea edx, [EAX + 222]
                    ),   

                    // 004AF15A
                    BinHook.CreateEdit("o_playercolor_table1", 7,
                        0x89, 0xF0, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT
                        0xB0, 0x01, //  MOV AL,1
                        0x8B, 0x14, 0x85, // mov edx, [eax*4 + namecolors]
                        new BinRefTo("namecolors", false)
                    ),

                    // 004AF1A9
                    BinHook.CreateEdit("o_playercolor_table2", 7,
                        0x89, 0xF0, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
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
                            0x75, 0x07, //  JNE SHORT 7
                            0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                            0xEB, 0x0A, //  JMP SHORT A
                            0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
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
                            0x75, 0x07, //  JNE SHORT 7
                            0xA0, new BinRefTo("PlayerColorChosenFactor4", false), //  MOV AL, [PlayerColorChosenFactor4]
                            0xEB, 0x0A, //  JMP SHORT A
                            0x3A, 0x05, new BinRefTo("PlayerColorChosenFactor4", false), //  CMP AL,[PlayerColorChosenFactor4]
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
                            0x75, 0x07, //  JNE SHORT 7
                            0xA0, new BinRefTo("PlayerColorChosenFactor4", false), //  MOV AL, [PlayerColorChosenFactor4]
                            0xEB, 0x0A, //  JMP SHORT A
                            0x3A, 0x05, new BinRefTo("PlayerColorChosenFactor4", false), //  CMP AL,[PlayerColorChosenFactor4]
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
                            0x75, 0x07, //  JNE SHORT 7
                            0xA0, new BinRefTo("PlayerColorChosenFactor4", false), //  MOV AL, [PlayerColorChosenFactor4]
                            0xEB, 0x0A, //  JMP SHORT A
                            0x3A, 0x05, new BinRefTo("PlayerColorChosenFactor4", false), //  CMP AL,[PlayerColorChosenFactor4]
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
                            0x75, 0x07, //  JNE SHORT 7
                            0xA0, new BinRefTo("PlayerColorChosenFactor4", false), //  MOV AL, [PlayerColorChosenFactor4]
                            0xEB, 0x0A, //  JMP SHORT A
                            0x3A, 0x05, new BinRefTo("PlayerColorChosenFactor4", false), //  CMP AL,[PlayerColorChosenFactor4]
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
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
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
                        0x75, 0x08, //  JNE SHORT 8
                        0x8A, 0x0D, new BinRefTo("PlayerColorChosen", false), //  MOV CL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x0D, new BinRefTo("PlayerColorChosen", false), //  CMP CL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT END
                        0xB1, 0x01, //  MOV CL,1
                        0x8B, 0x14, 0x8D, // mov edx, [ecx*4 + varscore]
                        new BinRefTo("namecolors", false)
                    ),

                    // 0047FAEE
                    BinHook.CreateEdit("o_playercolor_chat2", 7,
                        0x80, 0xF9, 0x01, //  CMP CL,1
                        0x75, 0x08, //  JNE SHORT 8
                        0x8A, 0x0D, new BinRefTo("PlayerColorChosen", false), //  MOV CL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x0D, new BinRefTo("PlayerColorChosen", false), //  CMP CL,[PlayerColorChosen]
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
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x05, 0x22, 0x02, 0x00, 0x00 //  ADD EAX,222
                    ),    
                    
                    // 004DE2C9
                    BinHook.CreateEdit("o_playercolor_trail_shield2", 7,

                        0x8B, 0xC6, // mov eax, esi
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT
                        0xB0, 0x01, //  MOV AL,1
                        
                        0x05, 0xD5, 0x01, 0x00, 0x00, // add eax, 1D5
                        0x50 // push eax
                    ),   

                    // 004DDA5F
                    BinHook.CreateEdit("o_playercolor_trail_shield", 6,
                        0x80, 0xFA, 0x00, //  CMP DL,0
                        0x75, 0x08, //  JNE SHORT 8
                        0x8A, 0x15, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV DL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x15, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP DL,[PlayerColorChosenMinusOne]
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
                            0x75, 0x07, //  JNE SHORT 7
                            0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                            0xEB, 0x0A, //  JMP SHORT A
                            0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
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
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV AL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP AL,[PlayerColorChosenMinusOne]
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
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV AL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP AL,[PlayerColorChosenMinusOne]
                        0x75, 0x02, //  JNE SHORT
                        0xB0, 0x00, //  MOV AL,0
                        
                        0x05, 0xCF, 0x02, 0x00, 0x00, // add eax, 2CF
                        0x50, // push eax
                        0x6A, 0x2E // push 2E
                    ),   


                    // 00428421
                    BinHook.CreateEdit("o_playercolor_mm_shield_hover", 6,
                        0x80, 0xFA, 0x00, //  CMP DL,0
                        0x75, 0x08, //  JNE SHORT 8
                        0x8A, 0x15, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV DL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x15, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP DL,[PlayerColorChosenMinusOne]
                        0x75, 0x02, //  JNE SHORT END
                        0xB2, 0x00, //  MOV DL,0
                        0x81, 0xC2, 0xD4, 0x00, 0x00, 0x00 // ori code,  ADD EDX,D4
                    ),   

                    // 00428360
                    BinHook.CreateEdit("o_playercolor_mm_shield_drag", 5,
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV AL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP AL,[PlayerColorChosenMinusOne]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x00, //  MOV AL,0
                        0x05, 0xD6, 0x01, 0x00, 0x00 // ori code,  ADD EAX,1D6
                    ),   

                    // 0042845B
                    BinHook.CreateEdit("o_playercolor_mm_shields", 6,
                        0x80, 0xF9, 0x00, //  CMP CL,0
                        0x75, 0x08, //  JNE SHORT 8
                        0x8A, 0x0D, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV CL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x0D, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP CL,[PlayerColorChosenMinusOne]
                        0x75, 0x02, //  JNE SHORT END
                        0xB1, 0x00, //  MOV CL,0

                        0x81, 0xC1, 0xD6, 0x01, 0x00, 0x00  // ori code,     add ecx, 1D6
                    ),   

                    // 004283C1
                    BinHook.CreateEdit("o_playercolor_mm_emblem1", 5,
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV AL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP AL,[PlayerColorChosenMinusOne]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x00, //  MOV AL,0
                        0x05, 0xCF, 0x02, 0x00, 0x00 // ori code,  ADD EAX,2CF
                    ),    

                    // 004282DD
                    BinHook.CreateEdit("o_playercolor_mm_emblem2", 5,
                        0x3C, 0x00, //  CMP AL,0
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosenMinusOne", false), //  MOV AL, [PlayerColorChosenMinusOne]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosenMinusOne", false), //  CMP AL,[PlayerColorChosenMinusOne]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x00, //  MOV AL,0
                        0x05, 0xCF, 0x02, 0x00, 0x00 // ori code,  ADD EAX,2CF
                    ),

                    #endregion

                    #region ingame

                    // 004BE94F
                    BinHook.CreateEdit("o_playercolor_ai_video_message_shield", 9,
                        0x3B, 0x1D, new BinRefTo("PlayerColorChosen", false), //  CMP EBX,[PlayerColorChosen]
                        0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                        0xBB, 0x01, 0x00, 0x00, 0x00, //  MOV EBX, 1

                        0x50, 0x52, 0xC7, 0x41, 0x04, 0x01, 0x00, 0x00, 0x00 // original code
                    ),

                    // 004B7B2C
                    BinHook.CreateEdit("o_playercolor_ai_video_message_shield_pre", 6,
                        0x8B, 0x86, 0xD4, 0x00, 0x00, 0x00, //  MOV EAX, [esi+D4]
                        0x3B, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP EAX,[PlayerColorChosen]
                        0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                        0xB8, 0x01, 0x00, 0x00, 0x00 //  MOV EAX, 1
                    ),

                    // 004B660A
                    BinHook.CreateEdit("o_playercolor_ai_video_message_shield_enemy_taunt", 6,
                        0x3B, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP EAX,[PlayerColorChosen]
                        0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  JNE SHORT 5
                        0xB8, 0x01, 0x00, 0x00, 0x00, //  MOV EAX, 1
                        0x52, 0x05, 0xD5, 0x01, 0x00, 0x00 //  original code
                    ),

                    // 004B7E7F
                    BinHook.CreateEdit("o_playercolor_ai_video_message_emblem", 7,
                        0x3B, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP EAX,[PlayerColorChosen]
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
                            0x3B, 0x35, new BinRefTo("PlayerColorChosen", false), //  CMP ESI,[PlayerColorChosen]
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
                            0x3B, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP EAX,[PlayerColorChosen]
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
                            0x3B, 0x35, new BinRefTo("PlayerColorChosen", false), //  CMP ESI,[PlayerColorChosen]
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
                            0x8B, 0x0D, new BinRefTo("PlayerColorChosen", false), //  MOV ECX,[PlayerColorChosen]
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
                        0x75, 0x08, //  JNE SHORT 8
                        0x8A, 0x0D, new BinRefTo("PlayerColorChosen", false), //  MOV CL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x0D, new BinRefTo("PlayerColorChosen", false), //  CMP CL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT END
                        0xB1, 0x01, //  MOV CL,1

                        0x83, 0xC2, 0xD9, 0x83, 0xFA, 0x26 // ori code
                    ),   

                    // 004B05CC
                    BinHook.CreateEdit("o_playercolor_emblem1", 5,
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x05, 0x22, 0x02, 0x00, 0x00 //  ADD EAX,222
                    ),     
                
                    // 004B06EB
                    BinHook.CreateEdit("o_playercolor_emblem2", 5,
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
                        0x75, 0x02, //  JNE SHORT 00427CD8
                        0xB0, 0x01, //  MOV AL,1
                        0x05, 0x22, 0x02, 0x00, 0x00 //  ADD EAX,222
                    ),


                    // 00427CC2
                    BinHook.CreateEdit("o_playercolor_list", 6,
                        0x89, 0xF0, //  MOV EAX,ESI
                        0x3C, 0x01, //  CMP AL,1
                        0x75, 0x07, //  JNE SHORT 7
                        0xA0, new BinRefTo("PlayerColorChosen", false), //  MOV AL, [PlayerColorChosen]
                        0xEB, 0x0A, //  JMP SHORT A
                        0x3A, 0x05, new BinRefTo("PlayerColorChosen", false), //  CMP AL,[PlayerColorChosen]
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
                        new BinBytes(0x75, 0x07), // jne to next cmp
                        new BinBytes(0xA0), // mov al, value
                        new BinRefTo("PlayerColorChosen", false),
                        new BinBytes(0xEB, 0x0A), // jmp to end
                        new BinBytes(0x3A, 0x05), // cmp al, value
                        new BinRefTo("PlayerColorChosen", false),
                        new BinBytes(0x75, 0x02), // jne to end
                        new BinBytes(0xB0, 0x01), // mov al, 1
                        // end
                        
                        new BinHook("endlabel", 0xE9)                // jmp hook
                        {
                            new BinBytes(0x3C, 0x01), // cmp al, 1
                            new BinBytes(0x75, 0x04), // jne to next cmp
                            new BinBytes(0xB0, 0x04), // mov al, 4
                            new BinBytes(0xEB, 0x0E), // jmp to end
                            new BinBytes(0x3C, 0x04), // cmp al, 4
                            new BinBytes(0x75, 0x04), // jne 4
                            new BinBytes(0xB0, 0x01), // mov al, 1
                            new BinBytes(0xEB, 0x06), // jmp to end
                            
                            new BinBytes(0x3C, 0x02), // cmp al, 2
                            new BinBytes(0x75, 0x02), // jne 2
                            new BinBytes(0x3C, 0x03), // cmp al, 3
                        },
                        // end

                        new BinLabel("endlabel"),
                        new BinBytes(0xA3), // mov [var], eax
                        new BinRefTo("var", false),

                        new BinNops(11),
                    },

                    // 00451E03
                    BinHook.CreateEdit("o_playercolor_fade", 5,
                        new BinBytes(0xA1), // mov eax, [var]
                        new BinRefTo("var", false),

                        new BinBytes(0x3C, 0x01), // cmp al, 1
                        new BinBytes(0x75, 0x07), // jne to next cmp
                        new BinBytes(0xA0), // mov al, value
                        new BinRefTo("PlayerColorChosen", false),
                        new BinBytes(0xEB, 0x0A), // jmp to end
                        new BinBytes(0x3A, 0x05), // cmp al, value
                        new BinRefTo("PlayerColorChosen", false),
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
                        new BinBytes(0x75, 0x07), // jne to next cmp
                        new BinBytes(0xA0), // mov al, value
                        new BinRefTo("PlayerColorChosen", false),
                        new BinBytes(0xEB, 0x0A), // jmp to end
                        new BinBytes(0x3A, 0x05), // cmp al, value
                        new BinRefTo("PlayerColorChosen", false),
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
            };
        }
    }
}