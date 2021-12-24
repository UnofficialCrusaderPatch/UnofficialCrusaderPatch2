using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace UCP.Model
{
    public class UCPConfig
    {
        public string schema;
        public string hash = BitConverter.ToString(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes(Environment.MachineName)));
        public string Path;
        public string AIV;
        public string StartTroop;
        public string StartResource;
        public List<ChangeConfiguration> GenericMods;
        public object AIC;

        public UCPConfig withAIV(string AIV)
        {
            this.AIV = AIV;
            return this;
        }

        public UCPConfig withStartTroop(string startTroop)
        {
            this.StartTroop = startTroop;
            return this;
        }

        public UCPConfig withStartResource(string startResource)
        {
            this.StartResource = startResource;
            return this;
        }

        public UCPConfig withGenericMods(List<ChangeConfiguration> genericMods)
        {
            this.GenericMods = genericMods;
            return this;
        }

        public UCPConfig withAIC(object aic)
        {
            this.AIC = aic;
            return this;
        }

        public UCPConfig withPath(string path)
        {
            this.Path = path;
            return this;
        }

        public UCPConfig withSchema(string schema)
        {
            this.schema = schema;
            return this;
        }

        public UCPConfig withHash(string hash)
        {
            this.hash = hash;
            return this;
        }
    }
}
