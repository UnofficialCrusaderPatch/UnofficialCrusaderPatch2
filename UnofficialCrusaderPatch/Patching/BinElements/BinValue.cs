using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    abstract class BinValue : BinBytes
    {
        double factor;
        public double Factor => factor;

        string valueIdent;
        public string ValueIdent => valueIdent;

        public BinValue(int size, double factor = 1, string valueIdent = null)
            : base(new byte[size])
        {
            this.factor = factor;
            this.valueIdent = valueIdent;
        }

        public void Set(double value)
        {
            this.editData = this.GetBytes(value);
        }

        protected abstract byte[] GetBytes(double value);
    }

    class BinInt32Value : BinValue
    {
        public BinInt32Value(double factor = 1, string valueIdent = null)
            : base(4, factor, valueIdent)
        {
        }

        protected override byte[] GetBytes(double value)
        {
            return BitConverter.GetBytes((int)(Factor * value));
        }
    }

    class BinByteValue : BinValue
    {
        public BinByteValue(string valueIdent = null, double factor = 1)
            : base(1, factor, valueIdent)
        {
        }

        protected override byte[] GetBytes(double value)
        {
            return new byte[1] { (byte)(Factor * value) };
        }
    }
}
