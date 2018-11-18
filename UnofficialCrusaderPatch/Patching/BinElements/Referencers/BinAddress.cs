using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    class BinAddress : BinLabel
    {
        int offset;
        public BinAddress(string name, int offset)
            : base(name)
        {
            this.offset = offset;
        }

        public override void Initialize(int rawAddr, int virtAddr, byte[] original)
        {
            virtAddr = BitConverter.ToInt32(original, rawAddr + this.offset);
            rawAddr = virtAddr - 0x400000;
            base.Initialize(rawAddr, virtAddr, original);
        }
    }
}
