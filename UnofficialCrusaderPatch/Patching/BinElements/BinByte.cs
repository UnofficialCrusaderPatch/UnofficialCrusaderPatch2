using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnofficialCrusaderPatch
{
    class BinByte : BinBytes
    {
        public BinByte(byte input)
            : base(input)
        {
        }

        public static Change Change(string locIdent, ChangeType type, byte newValue, bool checkedDefault = true)
        {
            return new Change(locIdent, type, checkedDefault)
            {
                CreateEdit(locIdent, newValue)
            };
        }

        public static BinaryEdit CreateEdit(string ident, byte newValue)
        {
            return new BinaryEdit(ident) { new BinByte(newValue) };
        }
    }
}
