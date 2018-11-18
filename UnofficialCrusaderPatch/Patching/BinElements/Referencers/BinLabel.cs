namespace UnofficialCrusaderPatch
{
    public class BinLabel : BinElement
    {
        string name;
        public string Name { get { return this.name; } }

        public BinLabel(string name)
        {
            this.name = name;
        }
    }
}
