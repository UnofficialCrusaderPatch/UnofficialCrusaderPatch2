using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Reflection;

namespace UnofficialCrusaderPatch
{
    public class BinaryChange : Change, IEnumerable<BinaryEdit>
    {
        List<BinaryEdit> edits;

        public BinaryChange(string ident, ChangeType type, bool checkedDefault = true)
            : base(ident, type, checkedDefault)
        {
            this.edits = new List<BinaryEdit>(1);
        }

        public void Add(BinaryEdit change)
        {
            this.edits.Add(change);
        }

        public IEnumerator<BinaryEdit> GetEnumerator() { return edits.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return edits.GetEnumerator(); }

        public void Edit(FileStream fs, byte[] oriData)
        {
            for (int i = 0; i < edits.Count; i++)
            {
                var result = edits[i].Edit(fs, oriData);
                if (result != BinaryEdit.Result.NoErrors)
                {
                    string message = string.Format("Binary Edit Error: {0} for edit {1} of change {2}", result, i, Ident);
                    throw new Exception(message);
                }
            }
        }
    }

    public abstract class BinaryEdit
    {
        string blockFile;
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
        }

        public Result Edit(FileStream fs, byte[] oriData)
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

            // edit file
            fs.Seek(address, SeekOrigin.Begin);
            fs.Write(this.editedData, 0, this.editedData.Length);
            return Result.NoErrors;
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
