using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCP.Model;
using UCP.Patching;

namespace UCP.Patching
{
    class Mod_Change_Other_NoSiegeTentDeselection : Mod
    {
        public Mod_Change_Other_NoSiegeTentDeselection() : base("o_engineertent")
        {
            this.changeList = new List<string>
            {
                "o_engineertent",
            };
        }

        override protected Change CreateExtremeChange()
        {
            return Change;
        }

        override protected Change CreateChange()
        {
            // 0044612B
            // nop out: mov [selection], ebp = 0
            return BinBytes.Change("o_engineertent", ChangeType.Other, true, 0x90, 0x90, 0x90, 0x90, 0x90, 0x90);
        }
    }
}
