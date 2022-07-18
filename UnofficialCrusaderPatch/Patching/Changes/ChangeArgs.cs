namespace UCP.Patching
{
    public struct ChangeArgs
    {
        public byte[] Data;
        public byte[] OriData;

        public ChangeArgs(byte[] data, byte[] oriData)
        {
            Data = data;
            OriData = oriData;
        }
    }
}
