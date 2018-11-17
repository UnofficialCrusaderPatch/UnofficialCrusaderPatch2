using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    class BinHook : BinRedirect
    {
        int hookLen;
        public override int Length => hookLen;

        byte[] jmpBytes;
        string jmpBackLabel;

        /// <summary> Any jump hook </summary>
        public BinHook(string jmpBackLabel, params byte[] jmpBytes)
            : this(jmpBackLabel, jmpBytes, jmpBytes.Length + 4)
        {
        }

        public BinHook(string jmpBackLabel, byte[] jmpBytes, int hookLen, params byte[] code)
            : base(true, code)
        {
            if (hookLen < jmpBytes.Length + 4)
                throw new Exception("Hook length is too short!");

            this.jmpBackLabel = jmpBackLabel;
            this.jmpBytes = jmpBytes;
            this.hookLen = hookLen;
        }

        public override EditResult Write(int address, BinArgs data)
        {
            var secPos = SectionEditor.GetDataPos();
            
            // add jmp back at end of code
            int labelAddress = data.Labels.GetLabel(this.jmpBackLabel);
            int endOfCode = (int)secPos.VirtualAddress + codeData.Count + 5;

            codeData.Add(0xE9);
            int relativeAddress = labelAddress - endOfCode;
            codeData.AddRange(BitConverter.GetBytes(relativeAddress));



            // write hook
            jmpBytes.CopyTo(data, address);
            int pos = address + jmpBytes.Length;

            // write address
            base.Write(pos, data);
            pos += 4;

            // fill rest with nops
            for (int i = pos; i < address + hookLen; i++)
                data.Buffer[i] = 0x90;

            return EditResult.NoErrors;
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
                new BinHook(ident, new byte[1] { 0xE9 }, hookLen, code),
                new BinLabel(ident)
            };
        }
    }
}
