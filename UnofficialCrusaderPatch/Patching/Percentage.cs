using System;

namespace UCP.Patching
{
    /// <summary>
    /// Percentage handler which can be given limits for subdividing
    /// </summary>
    public class Percentage
    {
        private         SetHandler handler;
        public delegate void       SetHandler(double percent);
        public Percentage(SetHandler handler)
        {
            this.handler = handler;
        }

        private double nextLimit;
        public double NextLimit
        {
            get => nextLimit;
            set
            {
                LastLimit = Total;
                nextLimit = value;
                if (nextLimit < LastLimit)
                {
                    throw new Exception("value < lastLimit");
                }
            }
        }

        public  double LastLimit { get; private set; }

        public  double Total { get; private set; }

        public void SetTotal(double value)
        {
            Total = value;
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
