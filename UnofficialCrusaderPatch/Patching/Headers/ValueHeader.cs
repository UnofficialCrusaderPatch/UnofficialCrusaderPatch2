using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class ValueHeader : DefaultHeader
    {
        double oriVal, suggested;
        public double OriginalValue => oriVal;
        public double SuggestedValue => suggested;

        public ValueHeader(string descrIdent, bool isEnabled, double oriVal, double suggested) : base(descrIdent, isEnabled)
        {
            this.value = oriVal;
            this.oriVal = oriVal;
            this.suggested = suggested;
        }

        double value;
        public double Value => value;

        public override bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                if (this.IsEnabled == value)
                    return;

                base.IsEnabled = value;
                if (IsEnabled)
                {
                    if (this.value == oriVal)
                        this.SetValue(suggested);
                }
                else
                {
                    this.SetValue(oriVal);
                }
            }
        }

        protected event Action OnValueChange;
        protected void SetValue(double value)
        {
            if (this.value == value)
                return;

            this.value = value;
            SetEnabled(value != oriVal);

            this.OnValueChange?.Invoke();
        }
    }
}
