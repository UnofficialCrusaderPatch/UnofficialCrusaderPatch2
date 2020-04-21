namespace UCP.Patching
{
    /**
     * FIX LADDERMEN AI BEHAVIOUR WHEN TARGET CASTLE IS ENCLOSED
     */
    public class Mod_Fix_AI_LaddermenUsage : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            return new Change("ai_fix_laddermen_with_enclosed_keep", ChangeType.Bugfix, true)
            {
                new DefaultHeader("ai_fix_laddermen_with_enclosed_keep")
                {

                    new BinaryEdit("ai_fix_laddermen_with_enclosed_keep") // 5774A
                    {
                        new BinBytes(0x6A, 0x01),
                    }

                }
            };
        }
    }
}