namespace UCP.Patching
{
    /**
     * UNLIMITED SIEGE ENGINES ON TOWERS
     */
    public class Mod_Fix_AI_TowerEngines : Mod
    {
        override public Change getChange()
        {
            // 004D20A2
            return BinBytes.Change("ai_towerengines", ChangeType.Bugfix, true, 0xEB);
        }
    }
}