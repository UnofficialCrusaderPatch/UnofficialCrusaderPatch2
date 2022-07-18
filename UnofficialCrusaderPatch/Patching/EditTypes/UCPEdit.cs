namespace UCP.Patching
{
    public class UCPEdit : BinaryEditBase
    {
        protected override bool GetAddresses(byte[] original, out int rawAddr, out int virtAddr)
        {
            AddressSpace space = SectionEditor.ReserveBufferSpace((uint)Length);
            rawAddr = (int)space.RawAddress;
            virtAddr = (int)space.VirtualAddress + 0x400000;
            return true;
        }

        public override void Activate(ChangeArgs args)
        {
            args.Data = SectionEditor.GetBuffer();
            base.Activate(args);
        }
    }
}
