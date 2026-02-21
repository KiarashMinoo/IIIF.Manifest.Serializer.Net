using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.RenderingProperty
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(RenderingJsonConverter))]
    public class Rendering : FormattableItem<Rendering>
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)] public string Label => GetElementValue(x => x.Label)!;

        public Rendering(string id, string label) : base(id)
        {
            SetElementValue(x => x.Label, label);
        }
    }
}