using System.Collections.Generic;

namespace UCP.Model
{
    public class AIVConfiguration
    {
        public Dictionary<string, string> Description { get; set; }

        public AIVConfiguration withDescription(Dictionary<string, string> description)
        {
            this.Description = description;
            return this;
        }
    }
}
