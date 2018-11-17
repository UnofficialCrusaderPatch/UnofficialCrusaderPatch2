namespace UnofficialCrusaderPatch
{
    public struct BinArgs
    {
        byte[] data;
        public byte[] Buffer => data;

        byte[] oriData;
        public byte[] Original => oriData;

        LabelCollection labels;
        public LabelCollection Labels => labels;

        public BinArgs(byte[] data, byte[] oriData, LabelCollection labels)
        {
            this.data = data;
            this.oriData = oriData;
            this.labels = labels;
        }

        public static implicit operator byte[](BinArgs data)
        {
            return data.Buffer;
        }
    }
}
