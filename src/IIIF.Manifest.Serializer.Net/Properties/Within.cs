using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties;

[PresentationAPI("2.0")]
public class Within(string id) : BaseItem<Within>(id)
{
    public const string LabelJName = "label";

    [JsonProperty(LabelJName)]
    public string? Label
    {
        get => GetElementValue(x => x.Label);
        private set => SetElementValue(value);
    }

    public Within SetLabel(string label)
    {
        Label = label;
        return this;
    }
}