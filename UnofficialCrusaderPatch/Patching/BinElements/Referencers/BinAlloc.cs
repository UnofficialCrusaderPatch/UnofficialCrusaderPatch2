using System.Collections;
using System.Collections.Generic;

namespace UCP.Patching
{
    public class BinAlloc : BinElement, IEnumerable<BinElement>
    {
        string name;
        public string Name => name;

        public BinAlloc(string name, uint size, bool isGlobal = false)
            : this(name, new byte[size], isGlobal)
        {
        }

        public BinAlloc(string name, byte[] data, bool isGlobal = false)
        {
            this.name = name;

            BinLabel newBinLabel = new BinLabel(name);

            if (isGlobal)
            {
                GlobalLabels.Add(newBinLabel);
            }

            editData.Add(newBinLabel);
            if (data != null && data.Length > 0)
                editData.Add(new BinBytes(data));
            editData.Add(new BinNops(4));
        }

        UCPEdit editData = new UCPEdit();
        public UCPEdit EditData => editData;

        public IEnumerator<BinElement> GetEnumerator() => editData.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => editData.GetEnumerator();

        public virtual void Add(BinElement input)
        {
            // add in front of nops
            editData.Insert(editData.Count - 1, input);
        }
    }
}
