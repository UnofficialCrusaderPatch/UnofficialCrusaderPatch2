namespace UnofficialCrusaderPatch
{
    public abstract class BinElement
    {
        public abstract int Length { get; }
        public abstract BinResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels);

        protected static int FindCodeCave(byte[] data, int startAddress, int length)
        {
            // improveme: search backwards

            int lastIndex = length - 1;
            for (int i = startAddress; i < data.Length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (data[i + j] == 0xCC)
                    {
                        if (j == lastIndex)
                        {
                            return i;
                        }
                    }
                    else break;
                }
            }
            return 0;
        }
    }
}
