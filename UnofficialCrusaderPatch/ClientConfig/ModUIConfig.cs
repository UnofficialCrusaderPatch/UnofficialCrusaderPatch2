using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCP.Patching;

namespace UCP.Model
{
    class ModUIConfig
    {
        public ChangeType modType { get; set; }
        public Dictionary<string, string> modDescription { get; set; }
        public ModSelectionType selectionType { get; set; }
        public IEnumerable<ChangeUIConfig> changes { get; set; }
    }
}
