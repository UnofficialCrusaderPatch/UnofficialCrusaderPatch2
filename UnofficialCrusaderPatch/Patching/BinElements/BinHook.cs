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
        
        List<byte> codeData = new List<byte>();
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
                this.codeData.AddRange(code);
        }

        public void Add(byte input)
        {
            if (codeData.Count == 10)
                throw new Exception("Hook code can only be < 10 bytes!");

            this.codeData.Add(input);
        }

        public IEnumerator<byte> GetEnumerator() { return codeData.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return codeData.GetEnumerator(); }

        public override EditResult Write(int address, byte[] data, byte[] oriData, LabelCollection labels)
        {
            // find a code cave
            int caveAddress = FindCodeCave(data, address, codeData.Count + 5);
            if (caveAddress == 0)
                return EditResult.NoHookspace;



            // write hook
            jmpBytes.CopyTo(data, address);
            int jmpLen = jmpBytes.Length + 4;

            byte[] buffer = BitConverter.GetBytes(caveAddress - (address + jmpLen));
            buffer.CopyTo(data, address + jmpBytes.Length);

            // fill rest with nops
            for (int i = jmpLen; i < hookLen; i++)
                data[address + i] = 0x90;

            

            // write into code cave
            codeData.CopyTo(data, caveAddress);

            // jmp back
            data[caveAddress + codeData.Count] = 0xE9;
            // jmpBackLabel address will be resolved by BinRefTo
            return base.Write(caveAddress + codeData.Count + 1, data, oriData, labels);
        }

        public static Change Change(string ident, ChangeType type, int hookLen, params byte[] code)
        {
            return Change(ident, type, true, hookLen, code);
        }

        public static Change Change(string ident, ChangeType type, bool checkedDefault, int hookLen, params byte[] code)
        {
            return new Change(ident, type, checkedDefault)
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
