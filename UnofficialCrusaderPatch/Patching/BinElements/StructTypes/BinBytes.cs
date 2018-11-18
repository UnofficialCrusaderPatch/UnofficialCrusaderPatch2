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

        public override void Write(BinArgs data)
        {
            byteBuf.CopyTo(data, this.RawAddress);
        }

        public static Change Change(string ident, ChangeType type, bool checkedDefault, params byte[] input)
        {
            return new Change(ident, type, checkedDefault)
            {
                new DefaultHeader(ident, true)
                {
                    CreateEdit(ident, input)
                }
            };
        }

        public static BinaryEdit CreateEdit(string ident, params byte[] input)
        {
            return new BinaryEdit(ident) { new BinBytes(input) };
        }
    }
}
