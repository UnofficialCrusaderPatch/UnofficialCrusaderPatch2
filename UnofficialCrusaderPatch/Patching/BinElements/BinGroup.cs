using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class BinGroup : BinElement
    {
        List<BinElement> elements = new List<BinElement>();
        public IEnumerable<BinElement> Elements => elements;

        protected BinGroup(params BinElement[] input)
        {
            this.elements.AddRange(input);
        }

        protected void AddToGroup(BinElement input)
        {
            this.elements.Add(input);
        }

        protected void InsertToGroup(int index, BinElement input)
        {
            this.elements.Insert(index, input);
        }
    }
}
