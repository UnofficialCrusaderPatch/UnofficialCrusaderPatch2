using System.Collections.Generic;
using UCP.Patching;

namespace UCP.Model
{
    class ModUIConfig
    {
        public ChangeType modType { get; set; }
        public Dictionary<string, string> modDescription { get; set; }
        public Dictionary<string, string> detailedDescription { get; set; }
        public string modSelectionRule { get; set; }
        public IEnumerable<ChangeUIConfig> changes { get; set; }
    }
}
