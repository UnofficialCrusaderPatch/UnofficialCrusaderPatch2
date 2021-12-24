using System.Collections.Generic;

namespace UCP.Model
{
    public class ChangeUIConfig
    {
        public ChangeUIConfig(
            IEnumerable<string> compatibility,
            string identifier,
            SelectionType selectionType,
            SelectionParameter selectionParameters,
            object defaultValue,
            Dictionary<string, string> description)
        {
            this.Compatibility = compatibility;
            this.Identifier = identifier;
            this.SelectionType = selectionType;
            this.SelectionParameters = selectionParameters;
            this.DefaultValue = defaultValue;
            this.Description = description;
        }

        IEnumerable<string> Compatibility { get; }
        string Identifier { get; }
        SelectionType SelectionType { get; }
        SelectionParameter SelectionParameters { get; }
        object DefaultValue { get; set; }
        Dictionary<string, string> Description { get; }
    }
}