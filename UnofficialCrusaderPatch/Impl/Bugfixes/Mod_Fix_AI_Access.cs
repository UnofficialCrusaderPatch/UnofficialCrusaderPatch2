using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * DISABLE DEMOLISHING OF INACCESSIBLE BUILDINGS
     */
    public class Mod_Fix_AI_Access : Mod
    {
        public Mod_Fix_AI_Access() : base("ai_access")
        {
            this.changeList = new List<string>
            {
                "ai_access"
            };
        }
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