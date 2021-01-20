using System;

namespace UCP.Patching
{
    /// <summary>
    /// Definition of an address specified by its label, offset, and whether it is relative to code position or fixed.
    /// </summary>
    class BinAddress : BinLabel
    {
        int offset;
        bool isRelative;
        public BinAddress(string name, int offset, bool isRelative = false)
            : base(name)
        {
            this.offset = offset;
            this.isRelative = isRelative;
        }

        public override void Initialize(int rawAddr, int virtAddr, byte[] original)
        {
            int read = BitConverter.ToInt32(original, rawAddr + this.offset);

            if (isRelative)
            {
                virtAddr = (virtAddr + this.offset + 4) + read;
            }
            else
            {
                virtAddr = read;
            }

            rawAddr = virtAddr - 0x400000;
            base.Initialize(rawAddr, virtAddr, original);
        }
    }
}
