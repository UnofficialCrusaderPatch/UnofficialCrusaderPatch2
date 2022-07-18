﻿namespace UCP.Patching
{
    public class BinRedirect : BinAlloc, IBinCollection
    {
        public  BinCollection Collection { get; } = new BinCollection();

        public BinRedirect(bool relative, params byte[] data)
            : base(data.GetHashCode().ToString(), data)
        {
            Collection.Add(new BinRefTo(Name, relative));
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
