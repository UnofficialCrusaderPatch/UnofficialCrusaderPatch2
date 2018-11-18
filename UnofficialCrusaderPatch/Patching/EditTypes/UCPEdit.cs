using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class UCPEdit : BinaryEditBase
    {
        protected override bool GetAddresses(byte[] original, out int rawAddr, out int virtAddr)
        {
            var space = SectionEditor.ReserveBufferSpace((uint)this.Length);
            rawAddr = (int)space.RawAddress;
            virtAddr = (int)space.VirtualAddress + 0x400000;
            return true;
        }

        public override void Activate(ChangeArgs args)
        {
            args.Data = SectionEditor.GetBuffer();
            base.Activate(args);
        }
    }
}
