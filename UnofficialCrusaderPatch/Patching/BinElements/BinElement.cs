namespace UnofficialCrusaderPatch
{

    public abstract class BinElement
    {
        public abstract int Length { get; }
        public abstract EditResult Write(int address, BinArgs data);

        public static implicit operator BinElement(int value)
        {
            return new BinByte((byte)value);
        }
    }
}
