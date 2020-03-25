using System;

namespace UCP.Patching
{
    abstract class BinValue : BinElement
    {
        double factor;
        public double Factor => factor;

        double offset;
        public double Offset => offset;

        public BinValue(double factor = 1, double offset = 0)
        {
            this.factor = factor;
            this.offset = offset;
        }

        protected abstract byte[] GetBytes(double value);

        public override void Write(BinArgs data)
        {
            byte[] buf = this.GetBytes(factor * data.Value + offset);
            buf.CopyTo(data, this.RawAddress);
        }
    }

    class BinInt32Value : BinValue
    {
        public override int Length => 4;

        public BinInt32Value(double factor = 1, double offset = 0)
            : base(factor, offset)
        {
        }

        protected override byte[] GetBytes(double value)
        {
            return BitConverter.GetBytes((int)value);
        }
    }

    class BinInt16Value : BinValue
    {
        public override int Length => 2;

        public BinInt16Value(double factor = 1, double offset = 0)
            : base(factor, offset)
        {
        }

        protected override byte[] GetBytes(double value)
        {
            return BitConverter.GetBytes((short)value);
        }
    }

    class BinByteValue : BinValue
    {
        public override int Length => 1;

        public BinByteValue(double factor = 1, double offset = 0)
            : base(factor, offset)
        {
        }

        protected override byte[] GetBytes(double value)
        {
            return new byte[1] { (byte)value };
        }
    }
}
