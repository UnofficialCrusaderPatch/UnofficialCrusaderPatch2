namespace UCP.Patching
{
    /**
     * AI OVERCLOCK
     */
    public class Mod_Change_AI_Overclock : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("ai_overclock", ChangeType.AILords, false)
            {
                new SliderHeader("ai_overclock", true, 0.5, 1, 0.05, 1, 0.75)
                {
                    // 0045CC20+6
                    new BinaryEdit("ai_overclock")
                    {
                        new BinInt32Value(200) // default: 200
                    }
                }
            };
        }
    }
}