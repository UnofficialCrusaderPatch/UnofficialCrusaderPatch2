namespace UCP.Patching
{
    /**
     * UNLIMITED SIEGE ENGINES ON TOWERS
     */
    public class Mod_Fix_AI_TowerEngines : Mod
    {

        override protected Change CreateExtremeChange()
        {
            return change;
        }
        
        override protected Change CreateChange()
        {
            // 004D20A2
            return BinBytes.Change("ai_towerengines", ChangeType.Bugfix, true, 0xEB);
        }
    }
}