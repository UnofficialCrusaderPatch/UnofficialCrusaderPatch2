using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCP.AICharacters
{
    /// <summary>
    /// Use to add a comment to a field
    /// </summary>
    public class RWComment : Attribute
    {
        string comment;
        public string Comment => comment;

        public RWComment(string comment)
        {
            this.comment = comment;
        }
    }
}
