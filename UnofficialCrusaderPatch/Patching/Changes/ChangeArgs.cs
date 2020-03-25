namespace UCP.Patching
{
    public struct ChangeArgs
    {
        public byte[] Data;
        public byte[] OriData;

        public ChangeArgs(byte[] data, byte[] oriData)
        {
            this.Data = data;
            this.OriData = oriData;
        }
    }
}
