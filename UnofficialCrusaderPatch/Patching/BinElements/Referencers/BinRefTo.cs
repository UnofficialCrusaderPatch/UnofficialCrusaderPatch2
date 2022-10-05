using System;

namespace UCP.Patching
{
    /// <summary>
    /// Defines a reference to the address of an existing BinLabel element
    /// </summary>
    public class BinRefTo : BinElement
    {
        public override int Length => 4;

        public  string LabelName { get; }

        public  bool Relative { get; }

        public BinRefTo(string labelName, bool relative = true)
        {
            LabelName = labelName;
            Relative = relative;
        }

        public override void Write(BinArgs data)
        {
            int labelAddress = data.Labels.GetLabel(LabelName);

            int refAddress = labelAddress;
            if (Relative)
            {
                refAddress -= (VirtAddress + 4);
            }
            
            BitConverter.GetBytes(refAddress).CopyTo(data, RawAddress);
        }
    }
}
