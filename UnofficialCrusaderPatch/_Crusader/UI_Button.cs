namespace UCP._Crusader
{
    class UI_Button
    {
        public enum Header
        {
            Default = 0x02000003,
            EndItem = 0x66, // empty item
        }

        public Header Var1 = Header.Default;
        public int PosX;
        public int PosY;
        public int Var04 = 0x48;
        public int Var05 = 0x48;
        public int ClickFunc = 0x4AE7D0;
        public int Argument;
        public int RenderFunc = 0x4AE950;
        public int Var09 = 0;
        public int Var10 = 1;
        public int Var11 = 0;
        public int Var12 = 0;
        public int Var13 = 0xFFFF;
        public int Var14 = 0;
        public int Var15 = 0;
        public int Var16 = 0;
        public int Var17 = 0;
        public int Var18 = 0;
        public int Var19 = 0;
        public int Var20 = 0xB96560;
    }
}
