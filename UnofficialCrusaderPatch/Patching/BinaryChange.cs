using System;
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

        public void Edit(byte[] data, byte[] oriData)
        {
            for (int i = 0; i < edits.Count; i++)
            {
                var result = edits[i].Edit(data, oriData);
                if (result != BinaryEdit.Result.NoErrors)
                {
                    string message = string.Format("Binary Edit Error: {0} for edit {1} of change {2}", result, i, Ident);
                    throw new Exception(message);
                }
            }
        }
    }
}
