using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using CodeBlox;
using System.Collections;

namespace UnofficialCrusaderPatch
{
    public class BinaryEdit : ChangeEdit, IEnumerable<BinElement>
    {
        int length;
        public int Length => length;

        CodeBlock block;
        List<BinElement> elements = new List<BinElement>();
        LabelCollection labels = new LabelCollection();
        

        public BinaryEdit(string blockIdent)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            // check if code block file is there
            string file = string.Format("UnofficialCrusaderPatch.CodeBlocks.{0}.block", blockIdent);
            if (!asm.GetManifestResourceNames().Contains(file))
                throw new Exception("MISSING BLOCK FILE " + file);

            // read code block file
            using (Stream stream = asm.GetManifestResourceStream(file))
                this.block = new CodeBlock(stream);
        }

        public void Add(BinElement e)
        {
            if (e is BinLabel label)
            {
                label.SetOffset(this.length);
                labels.Add(label);
            }

            this.elements.Add(e);
            this.length += e.Length;
        }

        public IEnumerator<BinElement> GetEnumerator() { return this.elements.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return this.elements.GetEnumerator(); }

        public override EditResult Activate(ChangeArgs args)
        {           
            // find equivalent position in original file
            int count = block.SeekCount(args.OriData, out int address);
            if (count > 1)
                return EditResult.MultipleBlocks;
            else if (count == 0)
                return EditResult.BlockNotFound;

            labels.Resolve(address);

            return DoEdit(address, args.Data, args.OriData);
        }

        EditResult DoEdit(int address, byte[] data, byte[] oriData)
        {
            BinArgs args = new BinArgs(data, oriData, labels);
            foreach (BinElement e in elements)
            {
                EditResult result = e.Write(address, args);
                if (result != EditResult.NoErrors)
                    return result;

                address += e.Length;
            }
            return EditResult.NoErrors;
        }
    }
}
