namespace UCP.Model.Patching.Section
{
    internal class Utils
    {
        internal static uint GetMultiples(uint size, uint mult)
        {
            uint num = size + mult - 1;
            return num - num % mult;
        }
    }
}
