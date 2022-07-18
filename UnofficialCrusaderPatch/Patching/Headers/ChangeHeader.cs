using System.Collections;
using System.Collections.Generic;

namespace UCP.Patching
{
    public class ChangeHeader : IEnumerable<ChangeEdit>
    {
        public  LabelCollection Labels { get; } = new LabelCollection();

        protected readonly List<ChangeEdit> editList = new List<ChangeEdit>();

        public IEnumerator<ChangeEdit> GetEnumerator() => editList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public virtual void Add(ChangeEdit edit)
        {
            editList.Add(edit);
            edit.SetParent(this);
        }

        public void Activate(ChangeArgs args)
        {
            Labels.Clear();
            if (!editList.TrueForAll(e => e.Initialize(args)))
            {
                return;
            }

            editList.ForEach(e => e.Activate(args));
        }
    }
}
