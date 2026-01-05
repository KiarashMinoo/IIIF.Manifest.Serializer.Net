using IIIF.Manifests.Serializer.Shared.FormatableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties.Rendering
{
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