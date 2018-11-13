using System;

namespace UnofficialCrusaderPatch
{
    public class BinShort : BinBytes
    {
        public BinShort(short input)
            : base(BitConverter.GetBytes(input))
        {
        }

        public static Change Change(string locIdent, ChangeType type, short newValue, bool checkedDefault = true)
        {
            return new Change(locIdent, type, checkedDefault)
            {
                new BinaryEdit(locIdent) { new BinShort(newValue), }
            };
        }

        public static BinaryEdit CreateEdit(string ident, short newValue)
        {
            return new BinaryEdit(ident) { new BinShort(newValue) };
        }
    }
}
