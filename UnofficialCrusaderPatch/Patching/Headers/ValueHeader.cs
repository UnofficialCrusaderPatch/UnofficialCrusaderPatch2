using System;
using System.Globalization;

namespace UCP.Patching
{
    public class ValueHeader : DefaultHeader
    {
        double oriVal, suggested;
        public double OriginalValue => oriVal;
        public double SuggestedValue => suggested;

        public ValueHeader(string descrIdent, bool isEnabled, double oriVal, double suggested) : base(descrIdent, isEnabled)
        {
            this.oriVal = oriVal;
            this.suggested = suggested;
        }

        public override void SetParent(Change change)
        {
            base.SetParent(change);
            this.value = this.IsEnabled ? suggested : oriVal;
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
            try
            {
                if (this.value == value)
                    return;

                this.value = value;

                bool enable = value != oriVal;
                if (enable != this.IsEnabled)
                {
                    SetEnabled(enable);
                }
                else
                {
                    Configuration.Save(this.Parent.TitleIdent);
                }

                this.OnValueChange?.Invoke();
            }
            catch (Exception e)
            {
                Debug.Show(e, "ay");
            }
        }

        public override string GetValueString()
        {
            return string.Format("{0};{1}", this.IsEnabled, this.value.ToString(CultureInfo.InvariantCulture));
        }

        public override void LoadValueString(string valueStr)
        {
            int index = valueStr.IndexOf(';');
            if (index < 0) return;

            base.LoadValueString(valueStr.Remove(index));

            valueStr = valueStr.Substring(index + 1).Trim();
            if (Double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                this.value = result;
        }
    }
}
