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
    public const string SummaryJName = "summary";

    [JsonProperty(LabelJName)] public IReadOnlyCollection<Label> Label => GetElementValue(x => x.Label) ?? [];

    [JsonProperty(SummaryJName)] public string? Summary => GetElementValue(x => x.Summary);

    public FeatureProperties()
    {
    }

    public FeatureProperties SetLabel(Label[] labels) => SetElementValue(a => a.Label, _ => [..labels]);
    public FeatureProperties AddLabel(Label label) => SetElementValue(a => a.Label, labels => labels.With(label));
    public FeatureProperties RemoveLabel(Label label) => SetElementValue(a => a.Label, labels => labels.Without(label));

    /// <summary>
    /// Set the summary for this feature properties.
    /// </summary>
    public FeatureProperties SetSummary(string summary) => SetElementValue(x => x.Summary, summary);
}