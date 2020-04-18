namespace UCP.Patching
{
    /**
     * SET DEFAULT MULTIPLAYER SPEED
     */
    public class Mod_Change_Other_DefaultMultiplayerSpeed : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("o_default_multiplayer_speed", ChangeType.Other)
            {
                new SliderHeader("o_default_multiplayer_speed", false, 20, 90, 1, 40, 40)
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