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


        [JsonProperty(MotivationJName)] public string Motivation => GetElementValue(x => x.Motivation)!;

        [JsonProperty(OnJName)] public string On => GetElementValue(x => x.On)!;

        [JsonProperty(SelectorJName)] public Selector.Selector? Selector => GetElementValue(x => x.Selector);


        public Segment(string id, SegmentResource resource, string on) : base(id, "oa:Annotation", resource)
        {
            SetElementValue(x => x.Motivation, "sc:painting");
            SetElementValue(x => x.On, on);
        }

        public Segment SetSelector(Selector.Selector selector) => SetElementValue(a => a.Selector, selector);
    }
}