using IIIF.Manifests.Serializer.Nodes.ContentNode.Video.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Video
{
    [JsonConverter(typeof(VideoJsonConverter))]
    public class Video : BaseContent<Video, VideoResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";

        [JsonProperty(MotivationJName)] public string Motivation => GetElementValue(x => x.Motivation) ?? "sc:painting";

        [JsonProperty(OnJName)] public string On => GetElementValue(x => x.On)!;

        public Video(string id, VideoResource resource, string on) : base(id, "oa:Annotation", resource)
        {
            SetElementValue(x => x.Motivation, "sc:painting");
            SetElementValue(x => x.On, on);
        }
    }
}