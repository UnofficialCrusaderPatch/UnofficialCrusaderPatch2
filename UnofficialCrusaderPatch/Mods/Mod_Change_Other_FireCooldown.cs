namespace UCP.Patching
{
    /**
     * EXTENDED FIREPROOF DURATION FOR EXTENGUISHED BUILDINGS
     */
    public class Mod_Change_Other_FireCooldown : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            // 0x00410A30 + 8 ushort default = 2000
            return new Change("o_firecooldown", ChangeType.Other)
            {
                new SliderHeader("o_firecooldown", true, 0, 20000, 500, 2000, 6000)
                {
                    new BinaryEdit("o_firecooldown")
                    {
                        new BinSkip(8),
                        new BinInt16Value()
                    },
                }
            };
        }
    }
}