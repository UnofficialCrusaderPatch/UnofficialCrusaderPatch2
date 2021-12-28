using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace UCP.Model
{
    public class ChangeUIConfig
    {
        IEnumerable<string> compatibility { get; }
        string identifier { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        SelectionType selectionType { get; }
        SelectionParameter selectionParameters { get; }
        object defaultValue { get; set; }
        Dictionary<string, string> description { get; }
        Dictionary<string, string> detailedDescription { get; }
    }
}