namespace UCP.Patching
{
    /**
     * DISABLE DEMOLISHING OF INACCESSIBLE BUILDINGS
     */
    public class Mod_Fix_AI_Access : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 004242C3
            return BinBytes.Change("ai_access", ChangeType.Bugfix, true, 0xEB);
        }
    }
}