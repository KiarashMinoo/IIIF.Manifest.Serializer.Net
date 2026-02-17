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

        [JsonProperty(MotivationJName)]
        public string Motivation { get; } = "sc:painting";

        [JsonProperty(OnJName)]
        public string On { get; private set; }

        public Video(string id, VideoResource resource, string on) : base(id, "oa:Annotation", resource) => On = on;
    }
}