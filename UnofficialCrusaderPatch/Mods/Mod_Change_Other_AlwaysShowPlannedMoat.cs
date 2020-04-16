namespace UCP.Patching
{
    /**
     * PLANNED MOATS ARE ALWAYS VISIBLE
     */
    public class Mod_Change_Other_AlwaysShowPlannedMoat : Mod
    {
        override public Change getChange()
        {
            return new Change("o_moatvisibility", ChangeType.Other)
            {
                new DefaultHeader("o_moatvisibility")
                {
                    // 4EC86C
                    new BinaryEdit("o_moatvisibility")
                    {
                        new BinSkip(0x24),
                        new BinBytes(0x15) // mov [ ], edx = 1 instead of ebp = 0
                    }
                }
            };
        }
    }
}