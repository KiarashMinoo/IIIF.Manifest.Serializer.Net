using IIIF.Manifests.Serializer.Nodes.ContentNode.Image.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Image
{
    [JsonConverter(typeof(ImageJsonConverter))]
    public class Image : BaseContent<Image, ImageResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";

        [JsonProperty(MotivationJName)] public string Motivation => GetElementValue(x => x.Motivation)!;

        [JsonProperty(OnJName)] public string On => GetElementValue(x => x.On)!;

        public Image(string id, ImageResource resource, string on) : base(id, "oa:Annotation", resource)
        {
            SetElementValue(x => x.Motivation, "sc:painting");
            SetElementValue(x => x.On, on);
        }
    }
}