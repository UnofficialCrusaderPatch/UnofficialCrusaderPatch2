using System.Collections.Generic;

namespace UCP.Model
{
    public class StartTroopConfiguration
    {
        public Dictionary<string, string> Description { get; set; }

        public StartTroopConfiguration withDescription(Dictionary<string, string> description)
        {
            this.Description = description;
            return this;
        }
    }
}
