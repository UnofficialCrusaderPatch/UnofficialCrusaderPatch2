using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCP.AICharacters
{    
    /*
     * READER / WRITER ATTRIBUTES
     */

    /// <summary>
    /// Use after renames
    /// </summary>
    public class RWNames : Attribute
    {
        string[] names;
        public IEnumerable<string> Names => names;

        public RWNames(params string[] names)
        {
            this.names = names;
        }
    }

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
