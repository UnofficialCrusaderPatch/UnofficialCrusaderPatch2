namespace UCP.Patching
{
    /**
     * DISABLE DEMOLISHING OF INACCESSIBLE BUILDINGS
     */
    public class Mod_Fix_AI_Access : Mod
    {
        override public Change getChange()
        {
            // 004242C3
            return BinBytes.Change("ai_access", ChangeType.Bugfix, true, 0xEB);
        }
    }
}