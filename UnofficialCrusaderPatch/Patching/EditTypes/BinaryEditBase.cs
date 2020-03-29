using System.Collections;
using System.Collections.Generic;

namespace UCP.Patching
{
    public abstract class BinaryEditBase : ChangeEdit, IEnumerable<BinElement>
    {
        int length;
        public int Length => length;

        public override void SetParent(ChangeHeader parent)
        {
            base.SetParent(parent);

            foreach (BinElement element in elements)
            {
                if (element is BinAlloc alloc)
                {
                    parent.Add(alloc.EditData);
                }
            }
        }

        List<BinElement> elements = new List<BinElement>();
        public int Count => elements.Count;

        public IEnumerator<BinElement> GetEnumerator() => elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Add(BinElement e)
        {
            this.Insert(elements.Count, e);
        }

        public void Insert(int index, BinElement e)
        {
            this.elements.Insert(index, e);
            this.length += e.Length;
        }

        protected abstract bool GetAddresses(byte[] original, out int rawAddr, out int virtAddr);

        public override bool Initialize(ChangeArgs args)
        {
            if (!GetAddresses(args.OriData, out int rawAddr, out int virtAddr))
                return false;

            foreach (BinElement e in elements)
            {
                if (e is IBinCollection group)
                {
                    foreach (BinElement ge in group.Collection)
                    {
                        InitElement(ge, ref rawAddr, ref virtAddr, args.OriData);
                    }
                }
                else
                {
                    InitElement(e, ref rawAddr, ref virtAddr, args.OriData);
                }
            }

            return true;
        }

        void InitElement(BinElement element, ref int rawAddr, ref int virtAddr, byte[] original)
        {
            element.Initialize(rawAddr, virtAddr, original);
            if (element is BinLabel label)
                this.Parent.Labels.Add(label);

            rawAddr += element.Length;
            virtAddr += element.Length;
        }

        public override void Activate(ChangeArgs args)
        {
            double value = Parent is ValueHeader vHeader ? vHeader.Value : 0;
            BinArgs binArgs = new BinArgs(args.Data, this.Parent.Labels, value);

            foreach (BinElement e in elements)
            {
                if (e is IBinCollection group)
                {
                    foreach (BinElement ge in group.Collection)
                        ge.Write(binArgs);
                }
                else
                {
                    e.Write(binArgs);
                }
            }
        }
    }
}
