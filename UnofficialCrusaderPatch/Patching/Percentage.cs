using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UCP.Patching
{
    /// <summary>
    /// Percentage handler which can be given limits for subdividing
    /// </summary>
    class Percentage
    {        
        SetHandler handler;
        public delegate void SetHandler(double percent);
        public Percentage(SetHandler handler)
        {
            this.handler = handler;
        }

        double nextLimit;
        public double NextLimit
        {
            get { return this.nextLimit; }
            set
            {
                this.lastLimit = total;
                this.nextLimit = value;
                if (nextLimit < lastLimit)
                    throw new Exception("value < lastLimit");
            }
        }

        double lastLimit;
        public double LastLimit { get { return lastLimit; } }

        double total;
        public double Total { get { return total; } }
        public void SetTotal(double value)
        {
            this.total = value;
            handler.Invoke(value);
        }

        public void Set(double value)
        {
            SetTotal((NextLimit - LastLimit) * value + LastLimit);
        }
    }
}
