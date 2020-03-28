using System;

namespace UCP.Patching
{
    /// <summary>
    /// Percentage handler which can be given limits for subdividing
    /// </summary>
    public class Percentage
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
            if (handler != null)
            {
                handler.Invoke(value);
            }
        }

        public void Set(double value)
        {
            SetTotal((NextLimit - LastLimit) * value + LastLimit);
        }
    }
}
