using System.Collections;
using System.Collections.Generic;

namespace UCP.Patching
{
    public interface IBinCollection
    {
        BinCollection Collection { get; }
    }

    public class BinCollection : IEnumerable<BinElement>
    {
        List<BinElement> elements = new List<BinElement>();

        public IEnumerator<BinElement> GetEnumerator() => elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public BinCollection(params BinElement[] input)
        {
            this.elements.AddRange(input);
        }

        public void Add(BinElement input)
        {
            this.elements.Add(input);
        }

        public void Insert(int index, BinElement input)
        {
            this.elements.Insert(index, input);
        }
    }
}
