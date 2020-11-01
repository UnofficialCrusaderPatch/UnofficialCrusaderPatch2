using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * SET DEFAULT MULTIPLAYER SPEED
     */
    public class Mod_Change_Other_Strongholdify : Mod
    {
        public Mod_Change_Other_Strongholdify() : base("o_shfy")
        {
            this.changeList = new List<string>
            {
                "o_shfy_beer",
                "o_shfy_religion",
                "o_shfy_peasantspawnrate",
                "o_shfy_resourcequantity"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_shfy", ChangeType.Other)
            {
                new DefaultSubChange("o_shfy_beer")
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

                new DefaultSubChange("o_shfy_religion")
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
                        new BinBytes(0xEB, 0x6D),
                        new BinSkip(132),
                        new BinBytes(0xB9, 0x00, 0x00, 0x00, 0x00),
                        new BinSkip(9),
                        new BinBytes(0x83, 0xC1, 0x00)
                    }
                },

                new DefaultSubChange("o_shfy_peasantspawnrate")
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

                new DefaultSubChange("o_shfy_resourcequantity")
                // fixes resource quantity
                {
                    new BinaryEdit("o_shfy_resourcequantity")
                    {
                        new BinBytes(0x83, 0xC0, 0x00)
                    },
                }
            };
        }
    }
}