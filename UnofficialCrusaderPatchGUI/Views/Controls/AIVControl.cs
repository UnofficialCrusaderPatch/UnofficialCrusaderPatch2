using System;

namespace UCP.Views.Controls
{
    class AIVControl
    {
        internal string Identifier { get; set; }
        internal Func<bool> Enabled { get; set; }
        internal Func<double?> Value { get; set; }
        internal Action<bool> SetEnabled { get; private set; }

        internal AIVControl withIdentifier(string identifier)
        {
            this.Identifier = identifier;
            return this;
        }

        internal AIVControl withEnabled(Func<bool> enabled)
        {
            this.Enabled = enabled;
            return this;
        }

        internal AIVControl withValue(Func<double?> value)
        {
            this.Value = value;
            return this;
        }

        internal AIVControl withSetEnabled(Action<bool> setter)
        {
            this.SetEnabled = setter;
            return this;
        }
    }
}
