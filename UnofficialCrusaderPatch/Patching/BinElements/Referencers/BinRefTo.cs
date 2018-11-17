using System;

namespace UnofficialCrusaderPatch
{
    public class BinRefTo : BinElement
    {
        public override int Length => 4;

        string labelName;
        bool relative;

        public BinRefTo(string labelName, bool relative = true)
        {
            this.labelName = labelName;
            this.relative = relative;
        }

        public override EditResult Write(int address, BinArgs data)
        {
            int labelAddress = data.Labels.GetLabel(this.labelName);

            int refAddress = labelAddress;
            if (relative)
            {
                refAddress -= (address + 4);
            }

            byte[] buf = BitConverter.GetBytes(refAddress);
            buf.CopyTo(data, address);

            return EditResult.NoErrors;
        }
    }
}
