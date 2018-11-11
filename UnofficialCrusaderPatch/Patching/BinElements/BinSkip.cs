using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    class BinSkip : BinElement
    {
        int count;
        public override int Length => count;

        public BinSkip(int count)
        {
            this.count = count;
        }

        public override BinResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels)
        {
            return BinResult.NoErrors;
        }
    }
}
