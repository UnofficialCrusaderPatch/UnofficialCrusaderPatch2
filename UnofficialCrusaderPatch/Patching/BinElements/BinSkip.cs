namespace UCP.Patching
{
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
