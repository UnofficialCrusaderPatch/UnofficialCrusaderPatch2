using System;

namespace UCP.Model
{
    class SubControlConfig
    {
        internal string Identifier { get; set; }
        internal Func<bool> Enabled { get; set; }
        internal Func<double?> Value { get; set; }
        internal Action<double> SetValue { get; private set; }
        internal Action<bool> SetEnabled { get; private set; }

        internal SubControlConfig withIdentifier(string identifier)
        {
            this.Identifier = identifier;
            return this;
        }

        internal SubControlConfig withEnabled(Func<bool> enabled)
        {
            this.Enabled = enabled;
            return this;
        }

        internal SubControlConfig withValue(Func<double?> value)
        {
            this.Value = value;
            return this;
        }

        internal SubControlConfig withSetEnabled(Action<bool> setter)
        {
            this.SetEnabled = setter;
            return this;
        }

        internal SubControlConfig withSetter(Action<double> setter)
        {
            this.SetValue = setter;
            return this;
        }
    }
}
