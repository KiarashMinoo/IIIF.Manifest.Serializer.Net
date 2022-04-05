using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    [JsonConverter(typeof(ImageJsonConverter))]
    public class Image : BaseContent<Image, ImageResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";

        [JsonProperty(MotivationJName)]
        public string Motivation { get; } = "sc:painting";

        [JsonProperty(OnJName)]
        public string On { get; private set; }

        public Image(string id, ImageResource resource, string on) : base(id, "oa:Annotation", resource) => On = on;
    }
}