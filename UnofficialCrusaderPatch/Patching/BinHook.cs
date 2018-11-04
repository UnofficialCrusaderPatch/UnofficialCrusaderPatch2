using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    class BinHook : BinRefTo, IEnumerable<byte>
    {
        public override int Length => hookLen;


        List<byte> editData = new List<byte>();
        byte[] jmpBytes;
        int hookLen;

        /// <summary> Any jump hook </summary>
        public BinHook(string jmpBackLabel, params byte[] jmpBytes)
            : this(jmpBackLabel, jmpBytes, jmpBytes.Length + 4, null)
        {
        }

        /// <summary> Simple hook with 0xE9 JMP byte </summary>
        public BinHook(string jmpBackLabel, int hookLen, byte[] code)
            : this(jmpBackLabel, new byte[] { 0xE9 }, hookLen, code)
        {
        }

        public BinHook(string jmpBackLabel, byte[] jmpBytes, int hookLen, byte[] code)
            : base(jmpBackLabel)
        {
            if (hookLen < jmpBytes.Length + 4)
                throw new Exception("Hook length is too short!");
            this.jmpBytes = jmpBytes;
            this.hookLen = hookLen;
            if (code != null)
                this.editData.AddRange(code);
        }

        public void Add(byte input)
        {
            if (editData.Count == 10)
                throw new Exception("Hook code can only be < 10 bytes!");

            this.editData.Add(input);
        }

        public IEnumerator<byte> GetEnumerator() { return editData.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return editData.GetEnumerator(); }

        public override BinaryEdit.Result Write(int address, byte[] data)
        {
            int caveAddress = FindCodeCave(data, address, editData.Count + 5);
            if (caveAddress == 0)
                return BinaryEdit.Result.NoHookspace;

            // write hook
            Buffer.BlockCopy(jmpBytes, 0, data, address, jmpBytes.Length);
            int jmpLen = jmpBytes.Length + 4;

            byte[] buffer = BitConverter.GetBytes(caveAddress - (address + jmpLen));
            Buffer.BlockCopy(buffer, 0, data, address + jmpBytes.Length, 4);
            for (int i = jmpLen; i < hookLen; i++)
                data[address + i] = 0x90;


            // write into code cave
            editData.CopyTo(data, caveAddress);

            // jmp back
            data[caveAddress + editData.Count] = 0xE9;
            // jmpBackLabel address will be resolved by BinRefTo later
            this.refAddress = caveAddress + editData.Count + 1;

            return BinaryEdit.Result.NoErrors;
        }

        public static BinaryChange Change(string ident, ChangeType type, int hookLen, params byte[] code)
        {
            return Change(ident, type, true, hookLen, code);
        }

        public static BinaryChange Change(string ident, ChangeType type, bool checkedDefault, int hookLen, params byte[] code)
        {
            return new BinaryChange(ident, type, checkedDefault)
            {
                CreateEdit(ident, hookLen, code)
            };
        }

        public static BinaryEdit CreateEdit(string ident, int hookLen, params byte[] code)
        {
            return new BinaryEdit(ident)
            {
                new BinHook(ident, hookLen, code),
                new BinLabel(ident)
            };
        }
    }
}
