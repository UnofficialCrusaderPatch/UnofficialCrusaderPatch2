namespace UCP.Patching
{
    /**
     * OVERRIDE IDENTITY MENU TO ADD NEW FUNCTIONALITY
     */
    public class Mod_Change_Other_OverrideIdentityMenu : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_override_identity_menu", ChangeType.Other, false, false)
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
                            0x66, 0x9C, //  pushf
                            0x50, //  push eax
                            0x51, //  push ecx
                            0x52, //  push edx
                            0xBA, 0x00, 0x00, 0x00, 0x00, //  mov edx,00
                            
                            0xA1, new BinRefTo("NormalCrusaderUnitListAddress", false), //  mov eax,NormalCrusaderUnitListAddress
                            0x50, //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            0x58, //  pop eax
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
                            
                            0x83, 0xFA, 0x01, //  cmp edx,01
                            0x0F, 0x84, 0x20, 0x00, 0x00, 0x00, //  je short 20
                            0x42, //  inc edx
                            
                            0x50, //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            0x58, //  pop eax
                            
                            0xA1, new BinRefTo("NormalArabUnitListAddress", false), //  mov eax,NormalArabUnitListAddress
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  jne short 5
                            0xA1, new BinRefTo("ExtremeNormalArabUnitListAddress", false), //  mov eax,ExtremeNormalArabUnitListAddress
                            0xE9, new BinRefTo("ResetRangedUnitsInList"), //  jmp ResetMeleeUnitsInList
                            
                            0x5A,  // pop edx
                            0x59,  // pop ecx
                            0x58, //  pop eax
                            0x66, 0x9D, //  popf
                            0xC3 //  ret
                        },
                        new BinAlloc("RangedUnitButtonClick", null)
                        {
                            0x66, 0x9C, //  pushf
                            0xFF, 0x05, new BinRefTo("CurrentlySelectedRangedUnit", false), //  inc [CurrentlySelectedRangedUnit]
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x06, //  cmp [CurrentlySelectedRangedUnit],06
                            0xF, 0x8C, 0x0A, 0x00, 0x00, 0x00, //  jl short A
                            0xC7, 0x05, new BinRefTo("CurrentlySelectedRangedUnit", false), 0x00, 0x00, 0x00, 0x00, //  mov [CurrentlySelectedRangedUnit],00
                            0xE8, new BinRefTo("SetSelectedRangedUnit"),
                            0x66, 0x9D, //  popf
                            0xC3 //  ret
                        },

                        new BinAlloc("SetSelectedMeleeUnit", null)
                        {
                            0x66, 0x9C, //  pushf
                            0x50, //  push eax
                            0x51, //  push ecx
                            0x52, //  push edx
                            0xBA, 0x00, 0x00, 0x00, 0x00, //  mov edx,00
                            
                            0xA1, new BinRefTo("NormalCrusaderUnitListAddress", false), //  mov eax,NormalCrusaderUnitListAddress
                            0x50, //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            0x58, //  pop eax
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
                            
                            0x83, 0xFA, 0x01, //  cmp edx,01
                            0x0F, 0x84, 0x20, 0x00, 0x00, 0x00, //  je short 20
                            0x42, //  inc edx
                            
                            0x50, //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            0x58, //  pop eax
                            
                            0xA1, new BinRefTo("NormalArabUnitListAddress", false), //  mov eax,NormalArabUnitListAddress
                            0x0F, 0x85, 0x05, 0x00, 0x00, 0x00, //  jne short 5
                            0xA1, new BinRefTo("ExtremeNormalArabUnitListAddress", false), //  mov eax,ExtremeNormalArabUnitListAddress
                            0xE9, new BinRefTo("ResetMeleeUnitsInList"), //  jmp ResetMeleeUnitsInList
                            
                            0x5A,  // pop edx
                            0x59,  // pop ecx
                            0x58, //  pop eax
                            0x66, 0x9D, //  popf
                            0xC3 //  ret
                        },
                        new BinAlloc("MeleeUnitButtonClick", null)
                        {
                            0x66, 0x9C, //  pushf
                            0xFF, 0x05, new BinRefTo("CurrentlySelectedMeleeUnit", false), //  inc [CurrentlySelectedMeleeUnit]
                            0x83, 0x3D, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x0A, //  cmp [CurrentlySelectedMeleeUnit],0A
                            0xF, 0x8C, 0x0A, 0x00, 0x00, 0x00, //  jl short A
                            0xC7, 0x05, new BinRefTo("CurrentlySelectedMeleeUnit", false), 0x00, 0x00, 0x00, 0x00, //  mov [CurrentlySelectedMeleeUnit],00
                            0xE8, new BinRefTo("SetSelectedMeleeUnit"),
                            0x66, 0x9D, //  popf
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
                            0x66, 0x9C, //  pushf
                            0xE8, new BinRefTo("IsExtremeToBool"), // call IsExtremeToBool
                            0x50, //  push eax
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x83, 0x38, 0x01, //  cmp [eax],01
                            0x58, //  pop eax
                            0x53, //  push ebx
                            0x75, 0x0D, //  jne D
                            
                            0x81, 0xFE, 0xA8, 0x13, 0x60, 0x00, //  cmp esi,"Stronghold_Crusader_Extreme.exe"+2013A8
                            0xBB, new BinRefTo("IdentityMenuButtonArrayExtreme", false), //  mov ebx,IdentityMenuButtonArrayExtreme
                            0xEB, 0x0B, //  jmp B
                            0x81, 0xFE, 0x98, 0x14, 0x60, 0x00, //  cmp esi,"Stronghold Crusader.exe"+201498
                            0xBB, new BinRefTo("IdentityMenuButtonArray", false), //  mov ebx,IdentityMenuButtonArray
                            0x0F, 0x85, 0x02, 0x00, 0x00, 0x00, //  jne short 2
                            
                            0x8B, 0xF3, //  mov esi,ebx
                            
                            0x5B, //  pop ebx
                            0x66, 0x9D, //  popf
                            0xC7, 0x43, 0x14, 0x00, 0x00, 0x00, 0x00, //  mov [ebx+14],00
                            0xC3, //  ret
                        },

                        new BinAlloc("IsExtremeToBool", null)
                        {
                            0x50, //  push eax
                            0xB8, 0x20, 0x01, 0x40, 0x00, //  mov eax,00000120
                            0x81, 0x38, 0xBD, 0x93, 0xAF, 0x5A, //  cmp [eax],5AAF93BD
                            0xB8, new BinRefTo("IsExtremeBool", false), //  mov eax,IsExtremeBool
                            0x75, 0x08, //  jne 8
                            0xC7, 0x00, 0x00, 0x00, 0x00, 0x00, //  mov [eax],00
                            0x58, //  pop eax
                            0xC3, //  ret
                            0xC7, 0x00, 0x01, 0x00, 0x00, 0x00, //  mov [eax],01
                            0x58, //  pop eax
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
            };
        }
    }
}