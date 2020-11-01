using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * UNLIMITED SIEGE ENGINES ON TOWERS
     */
    public class Mod_Fix_AI_TowerEngines : Mod
    {
        public Mod_Fix_AI_TowerEngines() : base("ai_towerengines")
        {
            this.changeList = new List<string>
            {
                "ai_towerengines"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // 004D20A2
            return BinBytes.Change("ai_towerengines", ChangeType.Bugfix, true, 0xEB);
        }
    }
}