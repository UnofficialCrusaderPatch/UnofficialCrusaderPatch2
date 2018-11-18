namespace UnofficialCrusaderPatch
{
    public struct AddressSpace
    {
        public uint VirtualAddress;
        public uint RawAddress;

        public AddressSpace(uint virtAddr, uint rawAddr)
        {
            this.VirtualAddress = virtAddr;
            this.RawAddress = rawAddr;
        }
    }
}
