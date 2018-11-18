using System;

namespace UnofficialCrusaderPatch
{
    public class BinInt32 : BinBytes
    {
        public BinInt32(int input)
            : base(BitConverter.GetBytes(input))
        {
        }

        public static Change Change(string locIdent, ChangeType type, int newValue, bool checkedDefault = true)
        {
            return new Change(locIdent, type, checkedDefault)
            {
                new DefaultHeader(locIdent, true)
                {
                    CreateEdit(locIdent, newValue)
                }
            };
        }

        public static BinaryEdit CreateEdit(string ident, int newValue)
        {
            return new BinaryEdit(ident) { new BinInt32(newValue) };
        }
    }
}
