using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Media;

namespace UnofficialCrusaderPatch
{
    public enum ChangeType
    {
        Balancing,
        Bugfix,
    }

    public class ChangeCollection : IEnumerable<Change>
    {
        string ident;
        public string Ident { get { return this.ident; } }
        public string Description { get { return Localization.Get(ident); } }
        public bool IsChecked { get; set; }

        ChangeType type;
        public ChangeType Type { get { return this.type; } }

        List<Change> changes;

        public ChangeCollection(string ident, ChangeType type = ChangeType.Balancing)
        {
            this.ident = ident;
            this.IsChecked = true;
            this.changes = new List<Change>(1);
            this.type = type;
        }

        public void Add(Change change)
        {
            this.changes.Add(change);
        }

        public IEnumerator<Change> GetEnumerator() { return changes.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return changes.GetEnumerator(); }

        public void Edit(FileStream fs)
        {
            changes.ForEach(c => c.Edit(fs));
        }
    }

    public abstract class Change
    {
        public abstract void Edit(FileStream fs);
    }

    public abstract class BinaryChange : Change
    {
        int address;
        public int Address { get { return this.address; } }

        byte[] data;

        public BinaryChange(int address, byte[] data)
        {
            this.address = address;
            this.data = data;
        }
        
        public override void Edit(FileStream fs)
        {
            int filePosition = this.Address - 0x400000;
            fs.Seek(filePosition, SeekOrigin.Begin);
            fs.Write(this.data, 0, this.data.Length);
        }
    }

    public class ChangeInt32 : BinaryChange
    {
        public ChangeInt32(int address, int newValue)
            : base(address, BitConverter.GetBytes(newValue))
        {
        }

        public static ChangeCollection Create(string ident, ChangeType type, int address, int newValue)
        {
            return new ChangeCollection(ident, type) { new ChangeInt32(address, newValue) };
        }
    }

    public class ChangeFloat : BinaryChange
    {
        public ChangeFloat(int address, float newValue)
            : base(address, BitConverter.GetBytes(newValue))
        {
        }

        public static ChangeCollection Create(string ident, ChangeType type, int address, float newValue)
        {
            return new ChangeCollection(ident, type) { new ChangeFloat(address, newValue) };
        }
    }

    public class ChangeBytes : BinaryChange
    {
        public ChangeBytes(int address, params byte[] data)
            : base(address, data)
        {
        }
        
        public static ChangeCollection Create(string ident, ChangeType type, int address, params byte[] data)
        {
            return new ChangeCollection(ident, type) { new ChangeBytes(address, data) };
        }
    }
}
