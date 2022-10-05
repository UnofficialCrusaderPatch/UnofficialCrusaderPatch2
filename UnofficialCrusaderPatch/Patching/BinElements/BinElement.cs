namespace UCP.Patching
{
    /// <summary>
    /// Definition a single binary element using target address and byte[] value definition
    /// </summary>
    public abstract class BinElement
    {
        public virtual int Length => 0;

        public  int RawAddress { get; private set; }

        public int VirtAddress { get; private set; }

        public virtual void Initialize(int rawAddr, int virtAddr, byte[] original)
        {
            RawAddress = rawAddr;
            VirtAddress = virtAddr;
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
