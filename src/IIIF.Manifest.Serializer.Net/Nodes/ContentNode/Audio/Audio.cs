using IIIF.Manifests.Serializer.Nodes.ContentNode.Audio.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Audio
{
    [JsonConverter(typeof(AudioJsonConverter))]
    public class Audio : BaseContent<Audio, AudioResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";

        [JsonProperty(MotivationJName)]
        public string Motivation { get; } = "sc:painting";

        [JsonProperty(OnJName)]
        public string On { get; private set; }

        public Audio(string id, AudioResource resource, string on) : base(id, "oa:Annotation", resource) => On = on;
    }
}