using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Collections;

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

        public void Edit(FileStream fs)
        {
            edits.ForEach(c => c.Edit(fs));
        }
    }
    
    public abstract class BinaryEdit
    {
        int address;
        public int Address { get { return this.address; } }

        byte[] data;

        public BinaryEdit(int address, byte[] data)
        {
            this.address = address;
            this.data = data;
        }

        public void Edit(FileStream fs)
        {
            int filePosition = this.Address - 0x400000;
            fs.Seek(filePosition, SeekOrigin.Begin);
            fs.Write(this.data, 0, this.data.Length);
        }
    }

    public class EditInt32 : BinaryEdit
    {
        public EditInt32(int address, int newValue)
            : base(address, BitConverter.GetBytes(newValue))
        {
        }

        public static BinaryChange Create(string ident, ChangeType type, int address, int newValue)
        {
            return new BinaryChange(ident, type) { new EditInt32(address, newValue) };
        }
    }

    public class EditFloat : BinaryEdit
    {
        public EditFloat(int address, float newValue)
            : base(address, BitConverter.GetBytes(newValue))
        {
        }

        public static BinaryChange Create(string ident, ChangeType type, int address, float newValue)
        {
            return new BinaryChange(ident, type) { new EditFloat(address, newValue) };
        }
    }

    public class EditBytes : BinaryEdit
    {
        public EditBytes(int address, params byte[] data)
            : base(address, data)
        {
        }

        public static BinaryChange Create(string ident, ChangeType type, int address, params byte[] data)
        {
            return new BinaryChange(ident, type) { new EditBytes(address, data) };
        }
    }
}
