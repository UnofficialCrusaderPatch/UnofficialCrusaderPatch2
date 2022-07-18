using System;
using System.Globalization;

namespace UCP.Patching
{
    public class ValueHeader : DefaultHeader
    {
        public  double OriginalValue { get; }

        public double SuggestedValue { get; }

        public ValueHeader(string descrIdent, bool isEnabled, double oriVal, double suggested) : base(descrIdent, isEnabled)
        {
            OriginalValue = oriVal;
            SuggestedValue = suggested;
        }

        public override void SetParent(Change change)
        {
            base.SetParent(change);
            Value = IsEnabled ? SuggestedValue : OriginalValue;
        }

        public  double Value { get; private set; }

        public override bool IsEnabled
        {
            get => base.IsEnabled;
            set
            {
                if (IsEnabled == value)
                {
                    return;
                }

                base.IsEnabled = value;
                if (IsEnabled)
                {
                    // Cheking floating-point with tolerance.
                    if (Math.Abs(Value - OriginalValue) < 0.00001)
                    {
                        SetValue(SuggestedValue);
                    }
                }
                else
                {
                    SetValue(OriginalValue);
                }
            }
        }

        protected event Action OnValueChange;
        protected void SetValue(double value)
        {
            try
            {
                if (Math.Abs(Value - value) < 0.00001)
                {
                    return;
                }

                Value = value;

                bool enable = Math.Abs(value - OriginalValue) > 0.00001;
                if (enable != IsEnabled)
                {
                    SetEnabled(enable);
                }
                else
                {
                    Configuration.Save(Parent.TitleIdent);
                }

                OnValueChange?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Show(e, "ay");
            }
        }

        public override string GetValueString()
        {
            return $"{IsEnabled};{Value.ToString(CultureInfo.InvariantCulture)}";
        }

        public override void LoadValueString(string valueStr)
        {
            int index = valueStr.IndexOf(';');
            if (index < 0)
            {
                return;
            }

            base.LoadValueString(valueStr.Remove(index));

            valueStr = valueStr.Substring(index + 1).Trim();
            if (Double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                Value = result;
            }
        }
    }
}
