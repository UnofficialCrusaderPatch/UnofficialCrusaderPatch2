using System;
using System.Linq;
using System.IO;
using System.Reflection;
using CodeBlox;

namespace UnofficialCrusaderPatch
{
    public abstract class BinaryEdit
    {
        string blockFile;
        public string BlockFile { get { return this.blockFile; } }

        byte[] editedData;

        public BinaryEdit(string blockIdent, byte[] editedData)
        {
            this.editedData = editedData;
            this.blockFile = "UnofficialCrusaderPatch.CodeBlocks." + blockIdent + ".block";
            Assembly asm = Assembly.GetExecutingAssembly();
            if (!asm.GetManifestResourceNames().Contains(this.blockFile))
                throw new Exception("Missing block file " + blockFile);
        }

        public enum Result
        {
            NoErrors,
            BlockNotFound,
            MultipleBlocks,
            NoHookspace,
        }

        protected virtual Result DoEdit(byte[] data, int address, byte[] editedData)
        {
            Buffer.BlockCopy(editedData, 0, data, address, editedData.Length);
            return Result.NoErrors;
        }

        public Result Edit(byte[] data, byte[] oriData)
        {
            CodeBlock block;
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream(blockFile))
            {
                block = new CodeBlock(stream);
            }

            // find equivalent position in original file
            int count = block.SeekCount(data, out int address);
            if (count == 0)
                return Result.BlockNotFound;
            else if (count > 1)
                return Result.MultipleBlocks;
            
            return DoEdit(data, address, this.editedData);
        }
        
        protected static int FindCodeCave(byte[] data, int startAddress, int length)
        {
            int lastIndex = length - 1;
            for (int i = startAddress; i < data.Length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    if (data[i + j] == 0xCC)
                    {
                        if (j == lastIndex)
                        {
                            return i;
                        }
                    }
                    else break;
                }
            }
            return 0;
        }
    }
    
    #region types of edits

    public class EditInt32 : BinaryEdit
    {
        public EditInt32(string blockIdent, int newValue)
            : base(blockIdent, BitConverter.GetBytes(newValue))
        {
        }

        public static BinaryChange Create(string ident, ChangeType type, int newValue)
        {
            return new BinaryChange(ident, type) { new EditInt32(ident, newValue) };
        }
    }

    public class EditFloat : BinaryEdit
    {
        public EditFloat(string blockIdent, float newValue)
            : base(blockIdent, BitConverter.GetBytes(newValue))
        {
        }

        public static BinaryChange Create(string ident, ChangeType type, float newValue)
        {
            return new BinaryChange(ident, type) { new EditFloat(ident, newValue) };
        }
    }

    public class EditBytes : BinaryEdit
    {
        public EditBytes(string blockIdent, params byte[] data)
            : base(blockIdent, data)
        {
        }

        public static BinaryChange Create(string ident, ChangeType type, params byte[] data)
        {
            return new BinaryChange(ident, type) { new EditBytes(ident, data) };
        }
    }

    #endregion
}
