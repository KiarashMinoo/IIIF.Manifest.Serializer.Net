using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Metadata
{
    [JsonConverter(typeof(MetadataJsonConverter))]
    public class Metadata : TrackableObject<Metadata>
    {
        public const string LabelJName = "label";
        public const string ValueJName = "value";

        private readonly List<MetadataValue.MetadataValue> value = new List<MetadataValue.MetadataValue>();


        [JsonProperty(LabelJName)]
        public string Label { get; }

        [JsonProperty(ValueJName)]
        public IReadOnlyCollection<MetadataValue.MetadataValue> Value => value;

        public Metadata(string label, MetadataValue.MetadataValue value)
        {
            Label = label;
            AddValue(value);
        }

        public Metadata(string label, string value) : this(label, new MetadataValue.MetadataValue(value))
        {
        }

        public Metadata(string label, string value, string language) : this(label, new MetadataValue.MetadataValue(value, language))
        {
        }

        public Metadata AddValue(MetadataValue.MetadataValue value) => SetPropertyValue(a => a.value, a => a.Value, this.value.Attach(value));
        public Metadata AddValue(string value, string language) => AddValue(new MetadataValue.MetadataValue(value, language));
        public Metadata AddValue(string value) => AddValue(new MetadataValue.MetadataValue(value));

        public Metadata ResetValue(MetadataValue.MetadataValue value)
        {
            this.value.Clear();
            return AddValue(value);
        }
        public Metadata ResetValue(string value, string language) => ResetValue(new MetadataValue.MetadataValue(value, language));
        public Metadata ResetValue(string value) => ResetValue(new MetadataValue.MetadataValue(value));
    }
}