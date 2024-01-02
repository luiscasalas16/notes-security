using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace tools.Converters.Hotspots
{
    public partial class HotspotsConverter
    {
        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("hotspots")]
        public List<Hotspot> Hotspots { get; set; }

        [JsonProperty("components")]
        public List<Component> Components { get; set; }
    }

    public partial class Component
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("qualifier")]
        public string Qualifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("longName")]
        public string LongName { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }
    }

    public partial class Hotspot
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("component")]
        public string Component { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("securityCategory")]
        public string SecurityCategory { get; set; }

        [JsonProperty("vulnerabilityProbability")]
        public string VulnerabilityProbability { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("line")]
        public long Line { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("creationDate")]
        public string CreationDate { get; set; }

        [JsonProperty("updateDate")]
        public string UpdateDate { get; set; }

        [JsonProperty("textRange")]
        public TextRange TextRange { get; set; }

        [JsonProperty("flows")]
        public List<object> Flows { get; set; }

        [JsonProperty("ruleKey")]
        public string RuleKey { get; set; }

        [JsonProperty("messageFormattings")]
        public List<object> MessageFormattings { get; set; }
    }

    public partial class TextRange
    {
        [JsonProperty("startLine")]
        public long StartLine { get; set; }

        [JsonProperty("endLine")]
        public long EndLine { get; set; }

        [JsonProperty("startOffset")]
        public long StartOffset { get; set; }

        [JsonProperty("endOffset")]
        public long EndOffset { get; set; }
    }

    public partial class Paging
    {
        [JsonProperty("pageIndex")]
        public long PageIndex { get; set; }

        [JsonProperty("pageSize")]
        public long PageSize { get; set; }

        [JsonProperty("total")]
        public long Total { get; set; }
    }

    public partial class HotspotsConverter
    {
        public static HotspotsConverter FromJson(string json) => JsonConvert.DeserializeObject<HotspotsConverter>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this HotspotsConverter self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
