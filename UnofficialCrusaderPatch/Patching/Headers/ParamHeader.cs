namespace UCP.Patching
{
    public class ParamHeader : DefaultHeader
    {
        public ParamHeader(string descrIdent) : base(descrIdent)
        {
        }

        public override void SetParent(Change change)
        {
            base.SetParent(change);
        }
    }
}
