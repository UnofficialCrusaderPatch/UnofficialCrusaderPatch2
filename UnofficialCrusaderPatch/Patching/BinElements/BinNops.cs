namespace UnofficialCrusaderPatch
{
    public class BinNops : BinElement
    {
        int count;
        public override int Length => count;

        public BinNops(int count)
        {
            this.count = count;
        }

        public override EditResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels)
        {
            for (int i = 0; i < count; i++)
                data[address + i] = 0x90;
            return EditResult.NoErrors;
        }
    }
}
