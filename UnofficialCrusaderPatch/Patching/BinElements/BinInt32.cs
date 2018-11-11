using System;

namespace UnofficialCrusaderPatch
{
    public class BinInt32 : BinBytes
    {
        public BinInt32(int input)
            : base(BitConverter.GetBytes(input))
        {
        }

        public static BinaryChange Change(string locIdent, ChangeType type, int newValue, bool checkedDefault = true)
        {
            return new BinaryChange(locIdent, type, checkedDefault)
            {
                new BinaryEdit(locIdent) { new BinInt32(newValue), }
            };
        }

        public static BinaryEdit CreateEdit(string ident, int newValue)
        {
            return new BinaryEdit(ident) { new BinInt32(newValue) };
        }
    }
}
