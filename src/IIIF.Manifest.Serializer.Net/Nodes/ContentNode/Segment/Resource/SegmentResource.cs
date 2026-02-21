using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.ContentNode.Segment.Resource
{
    public class SegmentResource(string id, string type) : BaseResource<SegmentResource>(id, type)
    {
        public const string FullJName = "full";

        [JsonProperty(FullJName)] public BaseResource? Full => GetElementValue(x => x.Full);

        public SegmentResource SetFull(BaseResource resource) => SetElementValue(a => a.Full, resource);
    }
}