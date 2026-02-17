using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Image
{
    [JsonConverter(typeof(ImageJsonConverter))]
    public class Image : BaseContent<Image, ImageResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";
        public const string TextGranularityJName = "textGranularity";

        [JsonProperty(MotivationJName)]
        public string Motivation { get; } = "sc:painting";

        [JsonProperty(OnJName)]
        public string On { get; private set; }

        [TextGranularityExtension("1.0")]
        [JsonProperty(TextGranularityJName)]
        public TextGranularity TextGranularity { get; private set; }

        public Image(string id, ImageResource resource, string on) : base(id, "oa:Annotation", resource) => On = on;

        public Image SetTextGranularity(TextGranularity textGranularity) => SetPropertyValue(a => a.TextGranularity, textGranularity);
    }
}