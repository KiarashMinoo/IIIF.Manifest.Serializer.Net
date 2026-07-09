using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.MetadataProperty;

public class Metadata : TrackableObject<Metadata>
{
    public const string LabelJName = "label";
    public const string ValueJName = "value";

    [JsonProperty(LabelJName)]
    public string Label
    {
        get => GetElementValue(x => x.Label)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(ValueJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<MetadataValue.MetadataValue> Value
    {
        get => GetElementValue(x => x.Value) ?? [];
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    private Metadata(string label)
    {
        Label = label;
    }

    public Metadata(string label, MetadataValue.MetadataValue value) : this(label)
    {
        Value = [value];
    }

    public Metadata(string label, string value) : this(label, new MetadataValue.MetadataValue(value))
    {
    }

    public Metadata(string label, string value, string language) : this(label, new MetadataValue.MetadataValue(value, language))
    {
    }

    public Metadata AddValue(MetadataValue.MetadataValue value)
    {
        Value = Value.With(value);
        return this;
    }

    public Metadata AddValue(string value, string language)
    {
        return AddValue(new MetadataValue.MetadataValue(value, language));
    }

    public Metadata AddValue(string value)
    {
        return AddValue(new MetadataValue.MetadataValue(value));
    }

    public Metadata ResetValue(MetadataValue.MetadataValue value)
    {
        Value = [value];
        return this;
    }

    public Metadata ResetValue(string value, string language)
    {
        return ResetValue(new MetadataValue.MetadataValue(value, language));
    }

    public Metadata ResetValue(string value)
    {
        return ResetValue(new MetadataValue.MetadataValue(value));
    }
}