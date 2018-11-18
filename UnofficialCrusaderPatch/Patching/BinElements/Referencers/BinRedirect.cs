using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class BinRedirect : BinGroup, IEnumerable<BinElement>
    {        
        public BinRedirect(bool relative)
        {
            string labelName = this.GetHashCode().ToString();
            this.AddToGroup(new BinRefTo(labelName, relative));

            this.editData.Add(new BinLabel(labelName));
            this.editData.Add(new BinNops(4));
        }

        UCPEdit editData = new UCPEdit();
        public UCPEdit EditData => editData;

        public IEnumerator<BinElement> GetEnumerator() => editData.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public virtual void Add(BinElement input)
        {
            // add in front of nops
            editData.Insert(editData.Count - 1, input);
        }

        public static BinaryEdit CreateEdit(string ident, bool relative, params BinElement[] data)
        {
            BinRedirect result = new BinRedirect(relative);
            foreach (BinElement element in data)
                result.Add(element);

            return new BinaryEdit(ident) { result };
        }
    }
}
