using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.Patching
{
    class BinSkip : BinElement
    {
        int count;
        public override int Length => count;

        public BinSkip(int count)
        {
            this.count = count;
        }
    }
}
