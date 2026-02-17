using IIIF.Manifests.Serializer.Nodes.ContentNode.Segment.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Segment
{
    public class Segment : BaseContent<Segment, SegmentResource>
    {
        public const string MotivationJName = "motivation";
        public const string OnJName = "on";
        public const string SelectorJName = "selector";


        [JsonProperty(MotivationJName)]
        public string Motivation { get; } = "sc:painting";

        [JsonProperty(OnJName)]
        public string On { get; }

        [JsonProperty(SelectorJName)]
        public Selector.Selector Selector { get; private set; }


        public Segment(string id, SegmentResource resource, string on) : base(id, "oa:Annotation", resource) => On = on;

        public Segment SetSelector(Selector.Selector selector) => SetPropertyValue(a => a.Selector, selector);
    }
}