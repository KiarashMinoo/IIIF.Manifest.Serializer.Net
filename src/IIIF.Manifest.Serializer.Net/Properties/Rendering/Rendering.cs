using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Rendering
{
    [PresentationAPI("2.0")]
    [JsonConverter(typeof(RenderingJsonConverter))]
    public class Rendering : FormatableItem<Rendering>
    {
        public const string LabelJName = "label";

        [JsonProperty(LabelJName)]
        public string Label { get; }

        public Rendering(string id, string label) : base(id)
        {
            Label = label;
        }
    }
}