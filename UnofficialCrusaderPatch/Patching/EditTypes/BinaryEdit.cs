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
            Assembly asm = Assembly.GetExecutingAssembly();

            // check if code block file is there
            string file = string.Format("UCP.CodeBlocks.{0}.block", blockIdent);
            if (!asm.GetManifestResourceNames().Contains(file))
                throw new Exception("MISSING BLOCK FILE " + file);

            // read code block file
            using (Stream stream = asm.GetManifestResourceStream(file))
                this.block = new CodeBlock(stream);

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
