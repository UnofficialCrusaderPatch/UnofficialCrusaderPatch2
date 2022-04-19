using System.Collections.Generic;

namespace UCP.Model
{
    public class ModUIConfig
    {
        public string modType { get; set; }
        public string modIdentifier { get; set;  }
        public string modDescription { get; set; }
        public string detailedDescription { get; set; }
        public string modSelectionRule { get; set; }
        public IEnumerable<ChangeUIConfig> changes { get; set; }
    }
}
