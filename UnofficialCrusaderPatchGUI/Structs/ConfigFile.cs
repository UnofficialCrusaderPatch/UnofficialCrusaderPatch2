using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UCP.Utility;

namespace UCP.Structs
{
    public struct ConfigFile
    {
        public String StrongholdPath;
        public Languages Language;
        public SavedConfig[] Configs;
    }
}
