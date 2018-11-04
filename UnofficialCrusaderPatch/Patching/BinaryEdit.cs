using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using CodeBlox;
using System.Collections;

namespace UnofficialCrusaderPatch
{
    public abstract class BinElement
    {
        public abstract int Length { get; }
        public abstract BinaryEdit.Result Write(int address, byte[] data);
        
        protected static int FindCodeCave(byte[] data, int startAddress, int length)
        {
            // improveme: search backwards

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

    public class BinLabel : BinElement
    {
        public override int Length => 0;

        string name;
        public string Name { get { return this.name; } }

        int address;
        public int Address { get { return this.address; } }

        public BinLabel(string name)
        {
            this.name = name;
        }

        public override BinaryEdit.Result Write(int address, byte[] data)
        {
            this.address = address;
            return BinaryEdit.Result.NoErrors;
        }
    }

    public class BinRefTo : BinElement
    {
        public override int Length => 4;

        protected int refAddress;
        protected string labelName;

        public BinRefTo(string labelName)
        {
            this.labelName = labelName;
        }

        public override BinaryEdit.Result Write(int address, byte[] data)
        {
            this.refAddress = address;
            return BinaryEdit.Result.NoErrors;
        }

        public void Resolve(List<BinElement> list, byte[] data)
        {
            BinLabel label = (BinLabel)list.Find(e => e is BinLabel l && labelName == l.Name);
            if (label == null)
                throw new Exception("Label could not be found: " + labelName);

            int reference = (label.Address) - (this.refAddress + 4);
            byte[] buf = BitConverter.GetBytes(reference);
            Buffer.BlockCopy(buf, 0, data, this.refAddress, 4);
        }
    }

    public class BinInt32 : BinElement
    {
        public override int Length => 4;

        int value;

        public BinInt32(int input)
        {
            this.value = input;
        }

        public override BinaryEdit.Result Write(int address, byte[] data)
        {
            byte[] buf = BitConverter.GetBytes(value);
            Buffer.BlockCopy(buf, 0, data, address, 4);
            return BinaryEdit.Result.NoErrors;
        }

        public static BinaryChange Change(string locIdent, ChangeType type, int newValue, bool checkedDefault = true)
        {
            return new BinaryChange(locIdent, type, checkedDefault)
            {
                new BinaryEdit(locIdent) { new BinInt32(newValue), }
            };
        }
    }

    public class BinByte : BinElement
    {
        byte value;
        public override int Length => 1;

        public BinByte(byte input)
        {
            this.value = input;
        }

        public override BinaryEdit.Result Write(int address, byte[] data)
        {
            data[address] = value;
            return BinaryEdit.Result.NoErrors;
        }

        public static implicit operator BinByte(byte input)
        {
            return new BinByte(input);
        }
    }

    public class BinBytes : BinElement
    {
        protected byte[] editData;
        public override int Length { get { return this.editData.Length; } }

        public BinBytes(params byte[] input)
        {
            this.editData = input;
        }

        public override BinaryEdit.Result Write(int address, byte[] data)
        {
            Buffer.BlockCopy(editData, 0, data, address, Length);
            return BinaryEdit.Result.NoErrors;
        }

        public static BinaryChange Change(string locIdent, ChangeType type, params byte[] input)
        {
            return Change(locIdent, type, true, input);
        }

        public static BinaryChange Change(string locIdent, ChangeType type, bool checkedDefault, params byte[] input)
        {
            return new BinaryChange(locIdent, type, checkedDefault)
            {
                new BinaryEdit(locIdent) { new BinBytes(input), }
            };
        }
    }

    public class BinaryEdit : IEnumerable<BinElement>
    {
        CodeBlock block;
        List<BinElement> elements = new List<BinElement>();

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
            this.elements.Add(e);
        }

        public IEnumerator<BinElement> GetEnumerator() { return this.elements.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return this.elements.GetEnumerator(); }

        public enum Result
        {
            NoErrors,
            BlockNotFound,
            MultipleBlocks,
            NoHookspace,
        }

        public Result Edit(byte[] data, byte[] oriData)
        {
            // find equivalent position in original file
            int count = block.SeekCount(data, out int address);
            if (count == 0)
                return Result.BlockNotFound;
            else if (count > 1)
                return Result.MultipleBlocks;

            return DoEdit(address, data, oriData);
        }

        Result DoEdit(int address, byte[] data, byte[] oriData)
        {
            foreach (BinElement e in elements)
            {
                Result result = e.Write(address, data);
                if (result != Result.NoErrors)
                    return result;

                address += e.Length;
            }

            foreach (BinRefTo r in elements.Where(e => e is BinRefTo))
                r.Resolve(elements, data);

            return Result.NoErrors;
        }
    }
}
