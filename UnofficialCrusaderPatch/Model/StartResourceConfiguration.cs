using System.Collections.Generic;

namespace UCP.Model
{
    public class StartResourceConfiguration
    {
        public Dictionary<string, string> Description { get; set; }

        public StartResourceConfiguration withDescription(Dictionary<string, string> description)
        {
            this.Description = description;
            return this;
        }
    }
}
