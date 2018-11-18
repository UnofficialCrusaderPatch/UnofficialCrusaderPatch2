namespace UnofficialCrusaderPatch
{
    public class BinArgs
    {
        byte[] data;
        public byte[] Buffer => data;

        LabelCollection labels;
        public LabelCollection Labels => labels;

        double value;
        public double Value => value;

        public BinArgs(byte[] data, LabelCollection labels, double value)
        {
            this.data = data;
            this.labels = labels;
            this.value = value;
        }

        public static implicit operator byte[](BinArgs data)
        {
            return data.Buffer;
        }
    }
}
