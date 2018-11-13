using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public abstract class ChangeEdit : ChangeElement
    {
        public abstract EditResult Activate(ChangeArgs args);
    }
}
