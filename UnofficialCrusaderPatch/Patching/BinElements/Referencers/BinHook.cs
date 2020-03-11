using System;

namespace UCP.Patching
{
    class BinHook : BinRedirect
    {
        public BinHook(int hookLen)
            : this(hookLen, null, 0xE9)
        {
        }

        public BinHook(string jmpBackLabel, params byte[] jmpBytes)
            : this(jmpBytes.Length + 4, jmpBackLabel, jmpBytes)
        {
        }

        public BinHook(int hookLen, string jmpBackLabel, params byte[] jmpBytes)
            : base(true)
        {
            if (hookLen < jmpBytes.Length + 4)
                throw new Exception("Hook length is too short!");

            this.Collection.Insert(0, jmpBytes);

            int nopsLen = hookLen - (4 + jmpBytes.Length);
            if (nopsLen > 0)
                this.Collection.Add(new BinNops(nopsLen));

            if (jmpBackLabel == null)
            {
                jmpBackLabel = this.GetHashCode().ToString() + "back";
                this.Collection.Add(new BinLabel(jmpBackLabel));
            }

            base.Add(new BinBytes(0xE9));
            base.Add(new BinRefTo(jmpBackLabel));
        }

        public override void Add(BinElement input)
        {
            // add in front of jmpBytes, refTo, nops
            EditData.Insert(EditData.Count - 3, input);
        }

        public static Change Change(string ident, ChangeType type, bool checkedDefault, int hookLen, params BinElement[] code)
        {
            return new Change(ident, type, checkedDefault)
            {
                new DefaultHeader(ident, true)
                {
                    CreateEdit(ident, hookLen, code)
                }
            };
        }

        public static BinaryEdit CreateEdit(string ident, int hookLen, params BinElement[] code)
        {
            var hook = new BinHook(hookLen, ident, new byte[1] { 0xE9 });
            foreach (BinElement element in code)
                hook.Add(element);

            return new BinaryEdit(ident)
            {
                hook,
                new BinLabel(ident)
            };
        }

        public static BinHook CreateJMP(int hookLen, params BinElement[] code)
        {
            var hook = new BinHook(hookLen, null, new byte[1] { 0xE9 });
            foreach (BinElement element in code)
                hook.Add(element);

            return hook;
        }
    }
}
