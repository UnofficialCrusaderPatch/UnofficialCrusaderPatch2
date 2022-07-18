namespace UCP.Patching
{
    public struct AddressSpace
    {
        public uint VirtualAddress;
        public uint RawAddress;

        public AddressSpace(uint virtAddr, uint rawAddr)
        {
            VirtualAddress = virtAddr;
            RawAddress = rawAddr;
        }
    }
}
