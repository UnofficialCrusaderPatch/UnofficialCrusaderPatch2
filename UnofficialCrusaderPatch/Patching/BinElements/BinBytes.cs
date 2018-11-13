using System;

namespace UnofficialCrusaderPatch
{
    public class BinBytes : BinElement
    {
        protected byte[] editData;
        public override int Length { get { return this.editData.Length; } }

        public BinBytes(params byte[] input)
        {
            this.editData = input;
        }

        public override EditResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels)
        {
            editData.CopyTo(data, address);
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
