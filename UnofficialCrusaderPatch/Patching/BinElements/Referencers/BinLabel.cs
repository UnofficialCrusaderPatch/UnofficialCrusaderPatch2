namespace UnofficialCrusaderPatch
{
    public class BinLabel : BinElement
    {
        public override int Length => 0;

        protected int offset;
        protected int labelAddress;
        public int Offset => offset;
        public int Address => labelAddress;
        public virtual void SetOffset(int offset) { this.offset = offset; }
        
        string name;
        public string Name { get { return this.name; } }

        public BinLabel(string name)
        {
            this.name = name;
        }

        public override EditResult Write(int address, BinArgs data)
        {
            return EditResult.NoErrors;
        }

        public virtual void Resolve(int startAddress)
        {
            this.labelAddress = this.offset + startAddress;
        }
    }
}
