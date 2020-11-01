using System;
using System.Collections.Generic;

namespace UCP.Model
{
    class ControlConfig
    {
        internal string Identifier { get; set; }
        internal Func<bool> Enabled { get; set; }
        internal List<SubControlConfig> SubControlConfigList { get; set; }

        internal ControlConfig withIdentifier(string identifier)
        {
            this.Identifier = identifier;
            return this;
        }

        internal ControlConfig withEnabled(Func<bool> enabled)
        {
            this.Enabled = () => { 
                return (enabled() || (SubControlConfigList != null && SubControlConfigList.Exists(x => x.Enabled() == true))); 
            };
            return this;
        }

        internal ControlConfig withSubControlConfig(List<SubControlConfig> config)
        {
            this.SubControlConfigList = config;
            return this;
        }
    }
}
