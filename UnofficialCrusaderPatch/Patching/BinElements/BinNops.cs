namespace UCP.Patching
{
    /// <summary>
    /// Represents nops (no-operation instructions) with count of the provided number of bytes.
    /// </summary>
    public class BinNops : BinElement
    {
        private         int count;
        public override int Length => count;

        public BinNops(int count)
        {
            this.count = count;
        }

        public override void Write(BinArgs data)
        {
            for (int i = 0; i < count; i++)
                data.Buffer[RawAddress + i] = 0x90;
        }

        public static BinaryEdit CreateEdit(string ident, int count)
        {
            return new BinaryEdit(ident) { new BinNops(count) };
        }
    }
}
