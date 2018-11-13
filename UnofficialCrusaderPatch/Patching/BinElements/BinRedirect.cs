using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class BinRedirect : BinBytes
    {
        public override int Length => 4;

        bool relative;

        public BinRedirect(bool relative, params byte[] editData)
            : base(editData)
        {
            this.relative = relative;
        }

        public override EditResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels)
        {
            // find code cave
            int caveAddress = FindCodeCave(data, address, editData.Length);
            if (caveAddress == 0)
                return EditResult.NoHookspace;



            // write into code cave
            editData.CopyTo(data, caveAddress);



            // write redirection
            int redirectAddress;
            if (this.relative)
                redirectAddress = caveAddress - (address + 4);
            else
                redirectAddress = caveAddress + 0x400000;

            byte[] buffer = BitConverter.GetBytes(redirectAddress);
            buffer.CopyTo(data, address);

            return EditResult.NoErrors;
        }

        public static BinaryEdit CreateEdit(string ident, bool relative, params byte[] code)
        {
            return new BinaryEdit(ident)
            {
                new BinRedirect(relative, code)
            };
        }
    }
}
