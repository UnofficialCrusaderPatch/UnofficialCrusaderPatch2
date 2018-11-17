using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class BinRedirect : BinElement, IEnumerable<byte>
    {
        public override int Length => 4;

        protected readonly List<byte> codeData;

        bool relative;

        public BinRedirect(bool relative, params byte[] codeData)
        {
            this.relative = relative;
            this.codeData = new List<byte>(codeData);
        }

        public void Add(byte input)
        {
            this.codeData.Add(input);
        }

        public override EditResult Write(int address, BinArgs data)
        {
            var pos = SectionEditor.GetDataPos();

            SectionEditor.AddData(codeData);
            SectionEditor.AddData(new byte[] { 0x90, 0x90, 0x90, 0x90 });
            
            // write redirection
            int redirectAddress;
            if (this.relative)
                redirectAddress = (int)pos.VirtualAddress - (address + 4);
            else
                redirectAddress = (int)pos.VirtualAddress + 0x400000;

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

        public IEnumerator<byte> GetEnumerator() => codeData.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
