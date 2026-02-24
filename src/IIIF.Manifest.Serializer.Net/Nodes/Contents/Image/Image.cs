using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Image
{
    public class Image : BaseContent<Image, ImageResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";

        [JsonProperty(MotivationJName)]
        public string Motivation
        {
            get => GetElementValue(x => x.Motivation)!;
            private set => SetElementValue(value);
        }

        [JsonProperty(OnJName)]
        public string On
        {
            get => GetElementValue(x => x.On)!;
            private set => SetElementValue(value);
        }

        public Image(string id, ImageResource resource, string on) : base(id, "oa:Annotation", resource)
        {
            Motivation = "sc:painting";
            On = on;
        }
    }
}