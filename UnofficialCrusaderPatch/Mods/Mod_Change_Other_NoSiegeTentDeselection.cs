namespace UCP.Patching
{
    /**
     * SIEGE TENT NO LONGER GETS DESELECTED WHEN BUILDING
     */
    public class Mod_Change_Other_NoSiegeTentDeselection : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            // 0044612B
            // nop out: mov [selection], ebp = 0
            return BinBytes.Change("o_engineertent", ChangeType.Other, true, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90);
        }
    }
}