using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnofficialCrusaderPatch
{
    public class ValueHeader : ChangeHeader
    {
        double oriVal, suggested;
        public double OriginalValue => oriVal;
        public double SuggestedValue => suggested;

        public ValueHeader(string descrIdent, double oriVal, double suggested) : base(descrIdent)
        {
            this.value = oriVal;
            this.oriVal = oriVal;
            this.suggested = suggested;
        }

        double value;
        public double Value => value;

        public void SetValueEdits()
        {
            foreach (var edit in EditList)
            {
                if (!(edit is BinaryEdit be))
                    continue;

                foreach (var ele in be)
                {
                    if (ele is BinValue bv)
                        bv.Set(this.value);
                }
            }
        }

        public override bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                if (this.IsEnabled == value)
                    return;

                base.IsEnabled = value;
                this.SetValue(value ? suggested : oriVal);
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
