﻿using System;

namespace UCP.Patching
{
    /// <summary>
    /// Convenience class for defining a 16-bit integer to be written to target CodeBlock
    /// </summary>
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
                new DefaultHeader(locIdent, true)
                {
                    CreateEdit(locIdent, newValue)
                }
            };
        }

        public static BinaryEdit CreateEdit(string ident, short newValue)
        {
            return new BinaryEdit(ident) { new BinShort(newValue) };
        }
    }
}
