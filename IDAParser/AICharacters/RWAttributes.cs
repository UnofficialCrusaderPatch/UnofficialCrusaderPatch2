using System;
using System.Collections.Generic;

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
        private string[]            names;
        public  IEnumerable<string> Names => names;

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
        public  string Comment { get; }

        public RWComment(string comment)
        {
            Comment = comment;
        }
    }
}
