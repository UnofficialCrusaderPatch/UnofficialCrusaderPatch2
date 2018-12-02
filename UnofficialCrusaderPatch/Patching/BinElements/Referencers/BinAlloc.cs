using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class BinAlloc : BinLabel
    {
        uint size;

        public BinAlloc(string name, uint size = 4)
            : base(name)
        {
            this.size = size;
        }

        public override void Initialize(int rawAddr, int virtAddr, byte[] original)
        {
            var space = SectionEditor.ReserveBufferSpace(this.size);
            rawAddr = (int)space.RawAddress;
            virtAddr = (int)space.VirtualAddress + 0x400000;

            base.Initialize(rawAddr, virtAddr, original);
        }
    }
}
