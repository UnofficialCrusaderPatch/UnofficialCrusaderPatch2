using System.Collections.Generic;

namespace UCP.Model
{
    public class ModBackendConfig
    {
        public string modType { get; set; }
        public string modIdentifier { get; set; }
        public Dictionary<string, string> modDescription { get; set; }
        public Dictionary<string, string> detailedDescription { get; set; }
        public string modSelectionRule { get; set; }
        public IEnumerable<ChangeBackendConfig> changes { get; set; }
    }
}
