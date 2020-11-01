using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * FIXES WEAPON ORDERS IN MARKETPLACE AND ARMORY
     */
    public class Mod_Change_Other_ArmoryMarketplaceWeaponOrderFix : Mod
    {
        public Mod_Change_Other_ArmoryMarketplaceWeaponOrderFix() : base("o_armory_marketplace_weapon_order_fix")
        {
            this.changeList = new List<string>
            {
                "o_armory_marketplace_weapon_order_fix"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_armory_marketplace_weapon_order_fix", ChangeType.Other)
            {
                new DefaultSubChange("o_armory_marketplace_weapon_order_fix")
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
            };
        }
    }
}