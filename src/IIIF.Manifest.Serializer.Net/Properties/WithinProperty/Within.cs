using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.WithinProperty
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(WithinJsonConverter))]
    public class Within(string id) : BaseItem<Within>(id)
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)] public string? Label => GetElementValue(x => x.Label);

        public Within SetLabel(string label) => SetElementValue(a => a.Label, label);
    }
}