namespace UnofficialCrusaderPatch
{
    public struct AddressSpace
    {
        uint virtAddr, rawAddr;
        public uint VirtualAddress => virtAddr;
        public uint RawAddress => rawAddr;

        public AddressSpace(uint virtAddr, uint rawAddr)
        {
            this.virtAddr = virtAddr;
            this.rawAddr = rawAddr;
        }
    }
}
