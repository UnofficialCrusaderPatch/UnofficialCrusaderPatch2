namespace UCP.Patching
{
    public class BinRedirect : BinAlloc, IBinCollection
    {
        BinCollection coll = new BinCollection();
        public BinCollection Collection => coll;

        public BinRedirect(bool relative, params byte[] data)
            : base(data.GetHashCode().ToString(), data)
        {
            coll.Add(new BinRefTo(Name, relative));
        }

        public static BinaryEdit CreateEdit(string ident, bool relative, params BinElement[] data)
        {
            BinRedirect result = new BinRedirect(relative);
            foreach (BinElement element in data)
                result.Add(element);

            return new BinaryEdit(ident) { result };
        }
    }
}
