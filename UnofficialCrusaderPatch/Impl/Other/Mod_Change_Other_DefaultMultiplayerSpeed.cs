using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * SET DEFAULT MULTIPLAYER SPEED
     */
    public class Mod_Change_Other_DefaultMultiplayerSpeed : Mod
    {
        public Mod_Change_Other_DefaultMultiplayerSpeed() : base("o_default_multiplayer_speed")
        {
            this.changeList = new List<string>
            {
                "o_default_multiplayer_speed"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_default_multiplayer_speed", ChangeType.Other)
            {
                new ValuedSubChange("o_default_multiplayer_speed")
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
            };
        }
    }
}