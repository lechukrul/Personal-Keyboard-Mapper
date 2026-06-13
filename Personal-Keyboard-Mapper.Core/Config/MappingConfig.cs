using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Personal_Keyboard_Mapper.Core.Config
{
    public class MappingConfig
    {
        [JsonPropertyName("CombinationSize")]
        public int CombinationSize { get; set; }

        [JsonPropertyName("Combinations")]
        public List<CombinationEntry> Combinations { get; set; }
    }

    public class CombinationEntry
    {
        [JsonPropertyName("FirstKey")]
        public int FirstKey { get; set; }

        [JsonPropertyName("SecondKey")]
        public int SecondKey { get; set; }

        [JsonPropertyName("Action")]
        public ActionEntry Action { get; set; }
    }

    public class ActionEntry
    {
        [JsonPropertyName("Type")]
        public string Type { get; set; }

        [JsonPropertyName("OutputVirtualKeys")]
        public List<string> OutputVirtualKeys { get; set; }
    }
}
