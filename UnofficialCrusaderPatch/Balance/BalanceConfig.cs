using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.Balance
{
    public class BalanceConfig
    {
        public class BuildingConfig
        {
            public int[] cost;
            public int health;
        }

        public class UnitConfig
        {
            public int health;
            public int arrowDamage;
            public int xbowDamage;
            public int stoneDamage;
            public Dictionary<string, int> meleeDamageVs;
        }

        public class ResourceConfig
        {
            public int buy;
            public int sell;
        }

        public Dictionary<string, string> description;
        public Dictionary<string, BuildingConfig> buildings;
        public Dictionary<string, UnitConfig> units;
        public Dictionary<string, ResourceConfig> resources;
        public Dictionary<string, object> siege;

        public BalanceConfig() { }
    }
}
