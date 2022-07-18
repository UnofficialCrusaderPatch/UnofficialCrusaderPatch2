namespace UCP.Patching
{
    public abstract class ChangeEdit
    {
        public abstract bool Initialize(ChangeArgs args);
        public abstract void Activate(ChangeArgs args);

        public  ChangeHeader Parent { get; private set; }

        public virtual void SetParent(ChangeHeader parent)
        {
            Parent = parent;
        }
    }
}
