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

        public override BinResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels)
        {
            editData.CopyTo(data, address);
            return BinResult.NoErrors;
        }

        public static BinaryChange Change(string locIdent, ChangeType type, params byte[] input)
        {
            return Change(locIdent, type, true, input);
        }

        public static BinaryChange Change(string locIdent, ChangeType type, bool checkedDefault, params byte[] input)
        {
            return new BinaryChange(locIdent, type, checkedDefault)
            {
                new BinaryEdit(locIdent) { new BinBytes(input), }
            };
        }
    }
}
