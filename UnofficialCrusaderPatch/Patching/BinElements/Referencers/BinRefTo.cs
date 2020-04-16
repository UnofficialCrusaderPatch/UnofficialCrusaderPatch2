using System;

namespace UCP.Patching
{
    public class BinRefTo : BinElement
    {
        public override int Length => 4;

        string labelName;
        public string LabelName => labelName;

        bool relative;
        public bool Relative => relative;

        public BinRefTo(string labelName, bool relative = true)
        {
            this.labelName = labelName;
            this.relative = relative;
        }

        public override void Write(BinArgs data)
        {
            int labelAddress = -1;
            
            try
            {
                labelAddress = data.Labels.GetLabel(this.labelName);
            }
            catch (Exception e)
            {
                labelAddress = GlobalLabels.GetLabel(this.labelName);
            }

            int refAddress = labelAddress;
            if (relative)
            {
                refAddress -= (this.VirtAddress + 4);
            }
            
            BitConverter.GetBytes(refAddress).CopyTo(data, this.RawAddress);
        }
    }
}
