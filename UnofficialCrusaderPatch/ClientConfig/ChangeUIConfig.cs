using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace UCP.Model
{
    public class ChangeUIConfig
    {
        public IEnumerable<string> compatibility { get; set; }
        public string identifier { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SelectionType selectionType { get; set; }
        public Dictionary<string, dynamic> selectionParameters { get; set; }
        public object defaultValue { get; set; }
        public string description { get; set; }
        public string detailedDescription { get; set;  }
    }
}