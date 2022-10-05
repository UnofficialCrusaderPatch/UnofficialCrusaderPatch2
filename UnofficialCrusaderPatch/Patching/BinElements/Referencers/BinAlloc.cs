using System.Collections;
using System.Collections.Generic;

namespace UCP.Patching
{
    public class BinAlloc : BinElement, IEnumerable<BinElement>
    {
        public  string Name { get; }

        public BinAlloc(string name, uint size)
            : this(name, new byte[size])
        {
        }

        public BinAlloc(string name, byte[] data)
        {
            Name = name;

            EditData.Add(new BinLabel(name));
            if (data != null && data.Length > 0)
            {
                EditData.Add(new BinBytes(data));
            }

            EditData.Add(new BinNops(4));
        }

        public  UCPEdit EditData { get; } = new UCPEdit();

        public IEnumerator<BinElement> GetEnumerator() => EditData.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => EditData.GetEnumerator();

        public virtual void Add(BinElement input)
        {
            // add in front of nops
            EditData.Insert(EditData.Count - 1, input);
        }
    }
}
