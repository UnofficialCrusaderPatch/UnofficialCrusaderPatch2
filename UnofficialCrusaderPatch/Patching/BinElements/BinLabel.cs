namespace UnofficialCrusaderPatch
{
    public class BinLabel : BinElement
    {
        public override int Length => 0;

        protected int offset;
        protected int address;
        public int Offset => offset;
        public int Address => address;
        public virtual void SetOffset(int offset) { this.offset = offset; }
        
        string name;
        public string Name { get { return this.name; } }

        public BinLabel(string name)
        {
            this.name = name;
        }

        public override EditResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels)
        {
            return EditResult.NoErrors;
        }

        public virtual void Resolve(int startAddress)
        {
            this.address = this.offset + startAddress;
        }
    }
}
