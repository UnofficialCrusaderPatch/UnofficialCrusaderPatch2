using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UnofficialCrusaderPatch
{
    class BinaryRedirect : BinaryEdit
    {
        bool relative;

        public BinaryRedirect(string blockIdent, bool relative, params byte[] editedData)
            : base(blockIdent, editedData)
        {
            this.relative = relative;
        }

        /// <summary> checkedDefault = true </summary>
        public static Change Create(string ident, ChangeType type, bool relative, params byte[] editedData)
        {
            return Create(ident, type, true, relative, editedData);
        }

        public static Change Create(string ident, ChangeType type, bool checkedDefault, bool relative, params byte[] editedData)
        {
            return new BinaryChange(ident, type, checkedDefault) { new BinaryRedirect(ident, relative, editedData) };
        }

        protected override Result DoEdit(byte[] data, int address, byte[] editedData)
        {
            int caveAddress = FindCodeCave(data, address, editedData.Length);
            if (caveAddress == 0)
                return Result.NoHookspace;

            // write into code cave
            Buffer.BlockCopy(editedData, 0, data, caveAddress, editedData.Length);

            // write redirection
            int redirectAddress;
            if (this.relative)
                redirectAddress = caveAddress - (address + 4);
            else
                redirectAddress = caveAddress + 0x400000;

            byte[] buffer = BitConverter.GetBytes(redirectAddress);
            Buffer.BlockCopy(buffer, 0, data, address, 4);

            return Result.NoErrors;
        }
    }
}
