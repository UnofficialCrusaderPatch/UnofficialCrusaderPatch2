using System.Collections.Generic;
using UCP.Model;

namespace UCP.Patching
{
    /**
     * CHANGE ARABIAN ARCHER STATS
     */
    public class Mod_Change_Units_ArabianArcher : Mod
    {
        public Mod_Change_Units_ArabianArcher() : base("u_arabxbow")
        {
            this.changeList = new List<string>
            {
                "u_arabxbow"
            };
        }
        override protected Change CreateExtremeChange()
        {
            return Change;
        }
        
        override protected Change CreateChange()
        {
            // Armbrustschaden gegen Arab. Schwertkämpfer, original: 8000
            // 0xB4EE4C = 0x4B*4 + 0xB4ED20
            return BinInt32.Change("u_arabxbow", ChangeType.Troops, 3500);
        }
    }
}