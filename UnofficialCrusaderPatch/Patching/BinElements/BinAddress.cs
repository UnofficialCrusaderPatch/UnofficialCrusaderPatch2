using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    class BinAddress : BinLabel
    {
        public BinAddress(string name, int offset)
            : base(name)
        {
            this.offset = offset;
        }

        public override void SetOffset(int offset) { }
        public override void Resolve(int editAddress) { }

        public override EditResult Write(int address, BinArgs data)
        {
            this.labelAddress = BitConverter.ToInt32(data.Original, address + this.offset);
            return EditResult.NoErrors;
        }

    }
}
