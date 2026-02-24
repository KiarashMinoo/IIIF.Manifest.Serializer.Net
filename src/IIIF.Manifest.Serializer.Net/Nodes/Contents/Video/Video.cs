using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Video
{
    public class Video : BaseContent<Video, VideoResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";

        [JsonProperty(MotivationJName)]
        public string Motivation
        {
            get => GetElementValue(x => x.Motivation) ?? "sc:painting";
            private set => SetElementValue(value);
        }

        [JsonProperty(OnJName)]
        public string On
        {
            get => GetElementValue(x => x.On)!;
            private set => SetElementValue(value);
        }

        public Video(string id, VideoResource resource, string on) : base(id, "oa:Annotation", resource)
        {
            Motivation = "sc:painting";
            On = on;
        }
    }
}