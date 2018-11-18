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

        public override void Write(BinArgs data)
        {
            for (int i = 0; i < count; i++)
                data.Buffer[this.RawAddress + i] = 0x90;
        }
    }
}
