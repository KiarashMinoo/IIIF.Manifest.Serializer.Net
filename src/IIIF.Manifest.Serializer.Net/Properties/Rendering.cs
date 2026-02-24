using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
public class Rendering : FormattableItem<Rendering>
{
    public const string LabelJName = "label";

    [JsonProperty(LabelJName)]
    public string Label
    {
        get => GetElementValue(x => x.Label)!;
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public Rendering(string id, string label) : base(id)
    {
        SetElementValue(x => x.Label, label);
    }
}