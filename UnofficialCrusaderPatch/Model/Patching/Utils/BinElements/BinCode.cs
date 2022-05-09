using System.Collections.Generic;

namespace UCP.Patching
{
    class BinCode : BinBytes
    {
        static readonly Dictionary<string, byte> RegisterOffsets = new Dictionary<string, byte>()
        {
            { "eax", 0 }, { "ecx", 1 },{ "edx", 2 },{ "ebx", 3 },{ "esp", 4 },{ "ebp", 5 },{ "esi", 6 }, { "edi", 7 }
        };

        public BinCode(string code)
        {
            code = code.ToLowerInvariant();

            // cmd
            int index = code.IndexOf(' ');
            string cmd = code.Remove(index);

            // args
            code = code.Substring(index + 1);
            string[] args = code.Split(',');
            for (int i = 0; i < args.Length; i++)
                args[i] = args[i].Trim();

            this.byteBuf = CmdFuncs[cmd].Invoke(args);
        }

        delegate byte[] CmdFuncHandler(string[] args);
        static readonly Dictionary<string, CmdFuncHandler> CmdFuncs = new Dictionary<string, CmdFuncHandler>()
        {
            { "push", FuncPush },
           // { "push", FuncSub },
        };
       
        static byte[] FuncPush(string[] args)
        {
            byte reg = RegisterOffsets[args[0]];
            return new byte[] { (byte)(0x50 + reg) };
        }

        
    }
}
