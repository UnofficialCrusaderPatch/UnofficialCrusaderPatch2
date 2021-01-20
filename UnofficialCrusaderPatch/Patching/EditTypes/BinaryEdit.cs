using CodeBlox;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UCP.Patching
{
    public class BinaryEdit : BinaryEditBase
    {
        string blockFile;
        public string BlockFile => this.blockFile;

        CodeBlock block;
        
        public BinaryEdit(string blockIdent)
        {
            this.block = CodeBlock.Get(blockIdent);
            this.blockFile = blockIdent;
        }

        protected override bool GetAddresses(byte[] original, out int rawAddr, out int virtAddr)
        {
            // find equivalent position in original file
            int count = block.SeekCount(original, out rawAddr);
            virtAddr = rawAddr + 0x400000;

            if (count > 1)
            {
                Patcher.AddFailure(this.blockFile, EditFailure.MultipleBlocks);
                return false;
            }
            else if (count == 0)
            {
                Patcher.AddFailure(this.blockFile, EditFailure.BlockNotFound);
                return false;
            }
            return true;
        }
    }
}
