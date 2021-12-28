using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace UCP.Model
{
    public class ChangeBackendConfig
    {
        public IEnumerable<string> compatibility { get; set; }
        public string identifier { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SelectionType selectionType { get; set; }
        public SelectionParameter selectionParameters { get; set; }
        public object defaultValue { get; set; }
        public Dictionary<string, string> description { get; set; }
        public Dictionary<string, string> detailedDescription { get; set;  }
    }
}