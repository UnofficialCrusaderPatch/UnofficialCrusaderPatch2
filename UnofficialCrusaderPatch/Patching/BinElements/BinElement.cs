namespace UCP.Patching
{

    public abstract class BinElement
    {
        public virtual int Length => 0;

        int rawAddr, virtAddr;
        public int RawAddress => rawAddr;
        public int VirtAddress => virtAddr;

        public virtual void Initialize(int rawAddr, int virtAddr, byte[] original)
        {
            this.rawAddr = rawAddr;
            this.virtAddr = virtAddr;
        }

        public virtual void Write(BinArgs data)
        {
        }

        public static implicit operator BinElement(byte value)
        {
            return new BinBytes(value);
        }

        public static implicit operator BinElement(byte[] buffer)
        {
            return new BinBytes(buffer);
        }
    }
}
