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
        private List<BinElement> elements = new List<BinElement>();

        public IEnumerator<BinElement> GetEnumerator() => elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public BinCollection(params BinElement[] input)
        {
            elements.AddRange(input);
        }

        public void Add(BinElement input)
        {
            elements.Add(input);
        }

        public void Insert(int index, BinElement input)
        {
            elements.Insert(index, input);
        }
    }
}
