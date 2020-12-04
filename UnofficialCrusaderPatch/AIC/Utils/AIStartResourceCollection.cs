using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCP.Startup;
using UCPAIConversion;

namespace UCP.AIC.Utils
{
    internal class AIStartResourceCollection
    {
        public Dictionary<StartingResource, int> normal { get; set; }
        public Dictionary<StartingResource, int> crusader { get; set; }
        public Dictionary<StartingResource, int> deathmatch { get; set; }
    }
}
