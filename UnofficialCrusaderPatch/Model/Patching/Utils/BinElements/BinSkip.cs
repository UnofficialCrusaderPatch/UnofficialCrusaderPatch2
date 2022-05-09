namespace UCP.Patching
{
    /// <summary>
    /// Represents a shift of the provided number of bytes.
    /// </summary>
    class BinSkip : BinElement
    {
        int count;
        public override int Length => count;

        public BinSkip(int count)
        {
            this.count = count;
        }
    }
}
