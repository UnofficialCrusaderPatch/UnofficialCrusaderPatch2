using System;

namespace UCP.Patching
{
    /// <summary>
    /// Represents a single value
    /// </summary>
    internal abstract class BinValue : BinElement
    {
        public  double Factor { get; }

        public  double Offset { get; }

        public BinValue(double factor = 1, double offset = 0)
        {
            Factor = factor;
            Offset = offset;
        }

        protected abstract byte[] GetBytes(double value);

        public override void Write(BinArgs data)
        {
            byte[] buf = GetBytes(Factor * data.Value + Offset);
            buf.CopyTo(data, RawAddress);
        }
    }

    internal class BinInt32Value : BinValue
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

    internal class BinInt16Value : BinValue
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

    internal class BinByteValue : BinValue
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
