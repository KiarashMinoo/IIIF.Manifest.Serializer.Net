using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.MetadataProperty
{
    [JsonConverter(typeof(MetadataJsonConverter))]
    public class Metadata : TrackableObject<Metadata>
    {
        public const string LabelJName = "label";
        public const string ValueJName = "value";

        [JsonProperty(LabelJName)] public string Label => GetElementValue(x => x.Label)!;

        [JsonProperty(ValueJName)] public IReadOnlyCollection<MetadataValue.MetadataValue> Value => GetElementValue(x => x.Value) ?? [];

        public Metadata(string label, MetadataValue.MetadataValue value)
        {
            SetElementValue(x => x.Label, label);
            AddValue(value);
        }

        public Metadata(string label, string value) : this(label, new MetadataValue.MetadataValue(value))
        {
        }

        public Metadata(string label, string value, string language) : this(label, new MetadataValue.MetadataValue(value, language))
        {
        }

        public Metadata AddValue(MetadataValue.MetadataValue value) => SetElementValue(a => a.Value, collection => collection.With(value));
        public Metadata AddValue(string value, string language) => AddValue(new MetadataValue.MetadataValue(value, language));
        public Metadata AddValue(string value) => AddValue(new MetadataValue.MetadataValue(value));

        public Metadata ResetValue(MetadataValue.MetadataValue value)
        {
            SetElementValue(x => x.Value, []);
            return AddValue(value);
        }

        public Metadata ResetValue(string value, string language) => ResetValue(new MetadataValue.MetadataValue(value, language));
        public Metadata ResetValue(string value) => ResetValue(new MetadataValue.MetadataValue(value));
    }
}