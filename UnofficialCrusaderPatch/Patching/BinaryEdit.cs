using System;
using System.Linq;
using System.IO;
using System.Reflection;

namespace UnofficialCrusaderPatch
{
    public abstract class BinaryEdit
    {
        protected string blockFile;
        public string BlockFile { get { return this.blockFile; } }

        byte[] editedData;

        public BinaryEdit(string blockIdent, byte[] editedData)
        {
            this.editedData = editedData;
            this.blockFile = "UnofficialCrusaderPatch.CodeBlocks." + blockIdent + ".bin";
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
            // read code block
            byte[] codeBlock;
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stream = asm.GetManifestResourceStream(blockFile))
            {
                codeBlock = new byte[stream.Length];
                stream.Read(codeBlock, 0, codeBlock.Length);
            }

            // find equivalent position in original file
            int address = 0;
            int max = oriData.Length - codeBlock.Length;
            int lastIndex = codeBlock.Length - 1;
            for (int i = 0; i < max; i++)
            {
                for (int j = 0; j < codeBlock.Length; j++)
                {
                    if (oriData[i + j] == codeBlock[j])
                    {
                        if (j == lastIndex)
                        {
                            if (address != 0)
                                return Result.MultipleBlocks;
                            address = i;
                            break;
                        }
                    }
                    else break;
                }
            }

            if (address == 0)
                return Result.BlockNotFound;

            return DoEdit(data, address, this.editedData);
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
