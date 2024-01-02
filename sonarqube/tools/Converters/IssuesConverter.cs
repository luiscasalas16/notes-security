using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace tools.Converters.Issues
{
    public partial class IssuesConverter
    {
        [JsonProperty("total")]
        public long Total { get; set; }

        [JsonProperty("p")]
        public long P { get; set; }

        [JsonProperty("ps")]
        public long Ps { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("effortTotal")]
        public long EffortTotal { get; set; }

        [JsonProperty("issues")]
        public List<Issue> Issues { get; set; }

        [JsonProperty("components")]
        public List<Component> Components { get; set; }

        [JsonProperty("facets")]
        public List<object> Facets { get; set; }
    }

    public partial class Component
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        [JsonProperty("qualifier")]
        public string Qualifier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("longName")]
        public string LongName { get; set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }
    }

    public partial class Issue
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("rule")]
        public string Rule { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("component")]
        public string Component { get; set; }

        [JsonProperty("project")]
        public string Project { get; set; }

        [JsonProperty("line")]
        public long Line { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("textRange")]
        public TextRange TextRange { get; set; }

        [JsonProperty("flows")]
        public List<Flow> Flows { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("effort")]
        public string Effort { get; set; }

        [JsonProperty("debt")]
        public string Debt { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("updateDate")]
        public DateTime UpdateDate { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("quickFixAvailable")]
        public bool QuickFixAvailable { get; set; }

        [JsonProperty("messageFormattings")]
        public List<object> MessageFormattings { get; set; }
    }

    public partial class Flow
    {
        [JsonProperty("locations")]
        public List<Location> Locations { get; set; }
    }

    public partial class Location
    {
        [JsonProperty("component")]
        public string Component { get; set; }

        [JsonProperty("textRange")]
        public TextRange TextRange { get; set; }

        [JsonProperty("msg", NullValueHandling = NullValueHandling.Ignore)]
        public string? Msg { get; set; }

        [JsonProperty("msgFormattings")]
        public List<object> MsgFormattings { get; set; }
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

    public partial class IssuesConverter
    {
        public static IssuesConverter FromJson(string json) => JsonConvert.DeserializeObject<IssuesConverter>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this IssuesConverter self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
