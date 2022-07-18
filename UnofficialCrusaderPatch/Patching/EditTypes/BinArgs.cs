namespace UCP.Patching
{
    public class BinArgs
    {
        public  byte[] Buffer { get; }

        public  LabelCollection Labels { get; }

        public  double Value { get; }

        public BinArgs(byte[] data, LabelCollection labels, double value)
        {
            Buffer = data;
            Labels = labels;
            Value = value;
        }

        public static implicit operator byte[](BinArgs data)
        {
            return data.Buffer;
        }
    }
}
