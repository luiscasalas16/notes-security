using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace tools.Converters.Projects
{
    public partial class ProjectsConverter
    {
        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("components")]
        public List<Component> Components { get; set; }
    }

    public partial class Component
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("qualifier")]
        public Qualifier Qualifier { get; set; }

        [JsonProperty("visibility")]
        public Visibility Visibility { get; set; }

        [JsonProperty("lastAnalysisDate")]
        public string LastAnalysisDate { get; set; }
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

    public enum Qualifier { Trk };

    public enum Visibility { Public };

    public partial class ProjectsConverter
    {
        public static ProjectsConverter FromJson(string json) => JsonConvert.DeserializeObject<ProjectsConverter>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this ProjectsConverter self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                QualifierConverter.Singleton,
                VisibilityConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class QualifierConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Qualifier) || t == typeof(Qualifier?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "TRK")
            {
                return Qualifier.Trk;
            }
            throw new Exception("Cannot unmarshal type Qualifier");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Qualifier)untypedValue;
            if (value == Qualifier.Trk)
            {
                serializer.Serialize(writer, "TRK");
                return;
            }
            throw new Exception("Cannot marshal type Qualifier");
        }

        public static readonly QualifierConverter Singleton = new QualifierConverter();
    }

    internal class VisibilityConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Visibility) || t == typeof(Visibility?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "public")
            {
                return Visibility.Public;
            }
            throw new Exception("Cannot unmarshal type Visibility");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Visibility)untypedValue;
            if (value == Visibility.Public)
            {
                serializer.Serialize(writer, "public");
                return;
            }
            throw new Exception("Cannot marshal type Visibility");
        }

        public static readonly VisibilityConverter Singleton = new VisibilityConverter();
    }
}
