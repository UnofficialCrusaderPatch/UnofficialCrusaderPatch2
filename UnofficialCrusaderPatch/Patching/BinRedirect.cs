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

        public override BinaryEdit.Result Write(int address, byte[] data)
        {
            int caveAddress = FindCodeCave(data, address, editData.Length);
            if (caveAddress == 0)
                return BinaryEdit.Result.NoHookspace;

            // write into code cave
            Buffer.BlockCopy(editData, 0, data, caveAddress, editData.Length);

            // write redirection
            int redirectAddress;
            if (this.relative)
                redirectAddress = caveAddress - (address + 4);
            else
                redirectAddress = caveAddress + 0x400000;

            byte[] buffer = BitConverter.GetBytes(redirectAddress);
            Buffer.BlockCopy(buffer, 0, data, address, 4);

            return BinaryEdit.Result.NoErrors;
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
