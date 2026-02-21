using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.MetadataProperty.MetadataValue
{
    [JsonConverter(typeof(MetadataValueJsonConverter))]
    public class MetadataValue : TrackableObject<MetadataValue>
    {
        public const string ValueJName = "@value";
        public const string LanguageJName = "@language";


        [JsonProperty(ValueJName)] public string Value => GetElementValue(x => x.Value)!;

        [JsonProperty(LanguageJName)] public string? Language => GetElementValue(x => x.Language);

        public MetadataValue(string value)
        {
            SetElementValue(x => x.Value, value);
        }

        public MetadataValue(string value, string language) : this(value)
        {
            SetElementValue(x => x.Language, language);
        }

        public MetadataValue SetValue(string value) => SetElementValue(a => a.Value, value);
    }
}