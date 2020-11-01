using System.Collections;
using System.Collections.Generic;
using UCP.Patching;

namespace UCP.Model
{
    public class Change: IEnumerable<DefaultSubChange>
    {
        public string Identifier { get; set; }
        public ChangeType Type { get; set; }
        public object Value { get; set; }

        public Change() { }
        public Change(string ident, ChangeType type)
        {
            this.Identifier = ident;
            this.Type = type;
        }

        public Change withType(ChangeType type)
        {
            this.Type = type;
            return this;
        }

        public Change withIdentifier(string identifier)
        {
            this.Identifier = identifier;
            return this;
        }

        List<DefaultSubChange> headerList = new List<DefaultSubChange>();
        public IEnumerator<DefaultSubChange> GetEnumerator() => headerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void Add(DefaultSubChange header)
        {
            headerList.Add(header);
            header.SetParent(this);
        }

        public virtual void Activate(ChangeArgs args)
        {
            foreach (DefaultSubChange header in headerList)
            {
                if (header.IsEnabled)
                {
                    header.Activate(args);
                }
            }
        }
    }
}