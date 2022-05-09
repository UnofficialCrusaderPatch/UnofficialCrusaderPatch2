using System.Collections.Generic;

namespace UCP.Model
{
    public class ChangeConfiguration
    {
        public string Identifier { get; set; }
        public Dictionary<string, double?> SubChanges { get; set; }

        int hashCode;

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Identifier.Equals(((ChangeConfiguration)obj).Identifier);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            if (hashCode != 0)
            {
                return hashCode;
            }

            int h = 0;
            for (int i = 0; i < Identifier.Length; i++)
            {
                h = 31 * h + Identifier[i];
            }
            hashCode = h;
            return hashCode;
        }
    }
}