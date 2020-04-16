namespace UCP.Patching
{
    /**
     * CHANGE ARABIAN ARCHER STATS
     */
    public class Mod_Change_Units_ArabianArcher : Mod
    {
        override public Change getChange()
        {
            // Armbrustschaden gegen Arab. Schwertkämpfer, original: 8000
            // 0xB4EE4C = 0x4B*4 + 0xB4ED20
            return BinInt32.Change("u_arabxbow", ChangeType.Troops, 3500);
        }
    }
}