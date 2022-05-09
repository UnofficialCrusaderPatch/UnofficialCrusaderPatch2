namespace UCP.Patching
{
    public abstract class ChangeEdit
    {
        public abstract bool Initialize(ChangeArgs args);
        public abstract void Activate(ChangeArgs args);

        SubChange parent;
        public SubChange Parent => parent;

        public virtual void SetParent(SubChange parent)
        {
            this.parent = parent;
        }
    }
}
