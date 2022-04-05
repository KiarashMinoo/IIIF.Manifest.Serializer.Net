using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(MetadataValueJsonConverter))]
    public class MetadataValue : TrackableObject<MetadataValue>
    {
        public const string ValueJName = "@value";
        public const string LanguageJName = "@language";


        [JsonProperty(ValueJName)]
        public string Value { get; private set; }

        [JsonProperty(LanguageJName)]
        public string Language { get; }

        public MetadataValue(string value)
        {
            Value = value;
        }

        public MetadataValue(string value, string language) : this(value)
        {
            Language = language;
        }

        public MetadataValue SetValue(string value) => SetPropertyValue(a => a.Value, value);
    }
}