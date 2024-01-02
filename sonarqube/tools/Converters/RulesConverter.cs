using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace tools.Converters.Rules
{
    public partial class RulesConverter
    {
        [JsonProperty("rule")]
        public Rule Rule { get; set; }

        [JsonProperty("actives")]
        public List<object> Actives { get; set; }
    }

    public partial class Rule
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("repo")]
        public string Repo { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

        [JsonProperty("htmlDesc")]
        public string HtmlDesc { get; set; }

        [JsonProperty("mdDesc")]
        public string MdDesc { get; set; }

        [JsonProperty("severity")]
        public string Severity { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("isTemplate")]
        public bool IsTemplate { get; set; }

        [JsonProperty("tags")]
        public List<object> Tags { get; set; }

        [JsonProperty("sysTags")]
        public List<string> SysTags { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("langName")]
        public string LangName { get; set; }

        [JsonProperty("params")]
        public List<Param> Params { get; set; }

        [JsonProperty("defaultDebtRemFnType")]
        public string DefaultDebtRemFnType { get; set; }

        [JsonProperty("defaultDebtRemFnOffset")]
        public string DefaultDebtRemFnOffset { get; set; }

        [JsonProperty("debtOverloaded")]
        public bool DebtOverloaded { get; set; }

        [JsonProperty("debtRemFnType")]
        public string DebtRemFnType { get; set; }

        [JsonProperty("debtRemFnOffset")]
        public string DebtRemFnOffset { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("defaultRemFnType")]
        public string DefaultRemFnType { get; set; }

        [JsonProperty("defaultRemFnBaseEffort")]
        public string DefaultRemFnBaseEffort { get; set; }

        [JsonProperty("remFnType")]
        public string RemFnType { get; set; }

        [JsonProperty("remFnBaseEffort")]
        public string RemFnBaseEffort { get; set; }

        [JsonProperty("remFnOverloaded")]
        public bool RemFnOverloaded { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("isExternal")]
        public bool IsExternal { get; set; }

        [JsonProperty("descriptionSections")]
        public List<DescriptionSection> DescriptionSections { get; set; }

        [JsonProperty("educationPrinciples")]
        public List<object> EducationPrinciples { get; set; }
    }

    public partial class DescriptionSection
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public partial class Param
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("htmlDesc")]
        public string HtmlDesc { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class RulesConverter
    {
        public static RulesConverter FromJson(string json) => JsonConvert.DeserializeObject<RulesConverter>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this RulesConverter self) => JsonConvert.SerializeObject(self, Converter.Settings);
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