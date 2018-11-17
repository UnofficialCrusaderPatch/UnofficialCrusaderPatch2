using System;
using System.Collections;
using System.Collections.Generic;

namespace UnofficialCrusaderPatch
{
    public class BinBytes : BinElement
    {

        protected readonly byte[] byteBuf;
        public override int Length => byteBuf.Length;

        public BinBytes(params byte[] input)
        {
            this.byteBuf = input;
        }

        public override EditResult Write(int address, BinArgs data)
        {
            byteBuf.CopyTo(data, address);
            return EditResult.NoErrors;
        }

        public static Change Change(string locIdent, ChangeType type, params byte[] input)
        {
            return Change(locIdent, type, true, input);
        }

        public static Change Change(string locIdent, ChangeType type, bool checkedDefault, params byte[] input)
        {
            return new Change(locIdent, type, checkedDefault)
            {
                new BinaryEdit(locIdent) { new BinBytes(input), }
            };
        }

        public static BinaryEdit CreateEdit(string ident, params byte[] input)
        {
            return new BinaryEdit(ident) { new BinBytes(input) };
        }
    }
}
