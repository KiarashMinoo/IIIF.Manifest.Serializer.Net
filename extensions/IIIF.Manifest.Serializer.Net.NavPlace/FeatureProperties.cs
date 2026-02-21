using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// Properties for a geographic feature.
/// Per the navPlace spec, the value of properties is a JSON object with zero or more properties.
/// Terms used in properties should be described by registered IIIF API extensions or local linked data contexts.
/// </summary>
[JsonConverter(typeof(FeaturePropertiesJsonConverter))]
public class FeatureProperties : TrackableObject<FeatureProperties>
{
    public const string LabelJName = "label";

    [JsonProperty(LabelJName)] public IReadOnlyCollection<Label> Label => GetElementValue(x => x.Label) ?? [];

    public FeatureProperties SetLabel(Label[] labels) => SetElementValue(a => a.Label, _ => [..labels]);
    public FeatureProperties AddLabel(Label label) => SetElementValue(a => a.Label, labels => labels.With(label));
    public FeatureProperties RemoveLabel(Label label) => SetElementValue(a => a.Label, labels => labels.Without(label));
}