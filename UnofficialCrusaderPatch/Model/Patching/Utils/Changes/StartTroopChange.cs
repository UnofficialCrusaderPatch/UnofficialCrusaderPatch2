using UCP.Patching;

namespace UCP.Model
{
    class StartTroopChange : Change
    {
        public StartTroopChange(string identifier)
            : base(identifier, ChangeType.StartTroops)
        {
        }
    }
}
