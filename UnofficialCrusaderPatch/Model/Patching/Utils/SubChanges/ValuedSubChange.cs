using UCP.Model;

namespace UCP.Patching
{
    public class ValuedSubChange : DefaultSubChange
    {
        public ValuedSubChange(string identifier) : base(identifier)
        {
        }

        public override void SetParent(Change change)
        {
            base.SetParent(change);
        }
        public double Value { get; set; }
    }
}
