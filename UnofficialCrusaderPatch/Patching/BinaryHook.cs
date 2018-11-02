using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    class BinaryHook : BinaryEdit
    {
        int hookLength;

        public BinaryHook(string blockIdent, int hookLen, params byte[] editedData)
            : base(blockIdent, editedData)
        {
            if (hookLen < 5)
                throw new Exception("Hook length cannot be smaller that 5!");
            this.hookLength = hookLen;
        }

        /// <summary> checkedDefault = true </summary>
        public static Change Create(string ident, ChangeType type, int hookLen, params byte[] editedData)
        {
            return Create(ident, type, hookLen, true, editedData);
        }

        public static Change Create(string ident, ChangeType type, int hookLen, bool checkedDefault, params byte[] editedData)
        {
            return new BinaryChange(ident, type, checkedDefault) { new BinaryHook(ident, hookLen, editedData) };
        }

        protected override Result DoEdit(byte[] data, int address, byte[] editedData)
        {
            int neededSpace = editedData.Length + 5;

            int caveAddress = FindCodeCave(data, address, neededSpace);
            if (caveAddress == 0)
                return Result.NoHookspace;

            using (MemoryStream ms = new MemoryStream(neededSpace))
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                // write into code cave
                bw.Write(editedData);
                bw.Write((byte)0xE9);
                bw.Write((address + hookLength) - (caveAddress + neededSpace));
                Buffer.BlockCopy(ms.ToArray(), 0, data, caveAddress, neededSpace);

                // write hook
                ms.Position = 0;
                ms.SetLength(0);
                bw.Write((byte)0xE9);
                bw.Write(caveAddress - (address + 5));
                for (int i = 5; i < hookLength; i++)
                    bw.Write((byte)0x90);
                Buffer.BlockCopy(ms.ToArray(), 0, data, address, hookLength);
            }

            return Result.NoErrors;
        }
    }
}
