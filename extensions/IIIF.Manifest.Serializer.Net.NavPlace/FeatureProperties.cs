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
public class FeatureProperties : TrackableObject<FeatureProperties>
{
    public const string LabelJName = "label";

    [JsonProperty(LabelJName)]
    public IReadOnlyCollection<Label> Label
    {
        get => GetElementValue(x => x.Label) ?? [];
        private set => SetElementValue(value);
    }

    public FeatureProperties SetLabel(IReadOnlyCollection<Label> labels)
    {
        Label = labels;
        return this;
    }

    public FeatureProperties AddLabel(Label label)
    {
        Label = Label.With(label);
        return this;
    }

    public FeatureProperties RemoveLabel(Label label)
    {
        Label = Label.Without(label);
        return this;
    }
}