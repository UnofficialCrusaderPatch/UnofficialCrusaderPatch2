using UCP.Model;

namespace UCP.Patching
{
    /// <summary>
    /// Convenience class for defining an array of bytes to be written to target CodeBlock
    /// </summary>
    public class BinBytes : BinElement
    {
        protected byte[] byteBuf;
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
            return new Change(ident, type)
            {
                Header(ident, true, input)
            };
        }

        public static DefaultSubChange Header(string ident, bool suggested, params byte[] input)
        {
            return new DefaultSubChange(ident)
                {
                    CreateEdit(ident, input)
                };
        }

        public static BinaryEdit CreateEdit(string ident, params byte[] input)
        {
            return new BinaryEdit(ident) { new BinBytes(input) };
        }
    }
}
