namespace UCP.Patching
{
    /// <summary>
    /// Represents a shift of the provided number of bytes.
    /// </summary>
    internal class BinSkip : BinElement
    {
        public override int Length { get; }

        public BinSkip(int count)
        {
            Length = count;
        }
    }
}
