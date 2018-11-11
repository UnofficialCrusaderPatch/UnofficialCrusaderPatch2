using System;
using System.Collections.Generic;
using System.Collections;

namespace UnofficialCrusaderPatch
{
    public class BinaryChange : Change, IEnumerable<BinaryEdit>
    {
        List<BinaryEdit> edits = new List<BinaryEdit>();

        public BinaryChange(string locIdent, ChangeType type, bool checkedDefault = true)
            : base(locIdent, type, checkedDefault)
        {
        }

        public void Add(BinaryEdit change)
        {
            this.edits.Add(change);
        }

        public IEnumerator<BinaryEdit> GetEnumerator() { return edits.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return edits.GetEnumerator(); }

        public virtual void Edit(byte[] data, byte[] oriData)
        {
            for (int i = 0; i < edits.Count; i++)
            {
                var result = edits[i].Edit(data, oriData);
                if (result != BinResult.NoErrors)
                {
                    string message = string.Format("Your version is probably not supported: {0} for edit {1} of change {2}", result, i, Ident);
                    throw new Exception(message);
                }
            }
        }
    }
}
