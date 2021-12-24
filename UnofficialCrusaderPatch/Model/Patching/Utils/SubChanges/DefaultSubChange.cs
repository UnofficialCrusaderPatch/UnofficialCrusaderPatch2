using UCP.Model;

namespace UCP.Patching
{
    public class DefaultSubChange : SubChange
    {
        Change parent;
        public Change Parent => parent;
        public virtual void SetParent(Change change)
        {
            this.parent = change;
        }

        string identifier;
        public string Identifier => identifier;

        public bool IsEnabled { get; set; }

        public DefaultSubChange(string identifier)
        {
            this.identifier = identifier;
            this.IsEnabled = false;
        }
    }
}
