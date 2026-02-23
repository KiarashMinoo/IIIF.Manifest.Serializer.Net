using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.ProviderProperty;

/// <summary>
/// IIIF Provider property - describes the organization providing the resource.
/// </summary>
[PresentationAPI("2.0")]
[method: JsonConstructor]
public class Provider(string id) : FormattableItem<Provider>(id, "Agent")
{
    public const string LabelJName = "label";

    [JsonProperty(LabelJName)]
    public string? Label
    {
        get => GetElementValue(x => x.Label);
        private set => SetElementValue(value);
    }

    public Provider(string id, string label) : this(id)
    {
        Label = label;
    }
}