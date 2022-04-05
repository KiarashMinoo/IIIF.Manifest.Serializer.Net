using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Properties
{
    [JsonConverter(typeof(MetadataJsonConverter))]
    public class Metadata : TrackableObject<Metadata>
    {
        public const string LabelJName = "label";
        public const string ValueJName = "value";

        private readonly List<MetadataValue> value = new List<MetadataValue>();


        [JsonProperty(LabelJName)]
        public string Label { get; }

        [JsonProperty(ValueJName)]
        public IReadOnlyCollection<MetadataValue> Value => value;

        public Metadata(string label, MetadataValue value)
        {
            Label = label;
            AddValue(value);
        }

        public Metadata(string label, string value) : this(label, new MetadataValue(value))
        {
        }

        public Metadata(string label, string value, string language) : this(label, new MetadataValue(value, language))
        {
        }

        public Metadata AddValue(MetadataValue value) => SetPropertyValue(a => a.value, a => a.Value, this.value.Attach(value));
        public Metadata AddValue(string value, string language) => AddValue(new MetadataValue(value, language));
        public Metadata AddValue(string value) => AddValue(new MetadataValue(value));

        public Metadata ResetValue(MetadataValue value)
        {
            this.value.Clear();
            return AddValue(value);
        }
        public Metadata ResetValue(string value, string language) => ResetValue(new MetadataValue(value, language));
        public Metadata ResetValue(string value) => ResetValue(new MetadataValue(value));
    }
}