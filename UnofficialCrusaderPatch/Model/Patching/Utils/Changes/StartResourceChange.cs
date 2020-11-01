using UCP.Patching;

namespace UCP.Model
{
    class StartResourceChange : Change
    {
        public StartResourceChange(string identifier)
        : base(identifier, ChangeType.StartResource)
        {
        }
    }
}
