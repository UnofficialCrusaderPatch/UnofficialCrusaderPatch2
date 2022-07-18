using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeBlox;

namespace UCP.Patching
{
    public class BinaryEdit : BinaryEditBase
    {
        public  string BlockFile { get; }

        private CodeBlock block;
        
        public BinaryEdit(string blockIdent)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            // check if code block file is there
            string file = $"UCP.CodeBlocks.{blockIdent}.block";
            if (!asm.GetManifestResourceNames().Contains(file))
            {
                throw new Exception("MISSING BLOCK FILE " + file);
            }

            // read code block file
            using (Stream stream = asm.GetManifestResourceStream(file))
                block = new CodeBlock(stream);

            BlockFile = blockIdent;
        }

        protected override bool GetAddresses(byte[] original, out int rawAddr, out int virtAddr)
        {
            // find equivalent position in original file
            int count = block.SeekCount(original, out rawAddr);
            virtAddr = rawAddr + 0x400000;

            if (count > 1)
            {
                Patcher.AddFailure(BlockFile, EditFailure.MultipleBlocks);
                return false;
            }

            if (count != 0)
            {
                return true;
            }

            Patcher.AddFailure(BlockFile, EditFailure.BlockNotFound);
            return false;
        }
    }
}
