using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Content.Segment.Resource
{
    public class SegmentResource : BaseResource<SegmentResource>
    {
        public const string FullJName = "full";

        [JsonProperty(FullJName)]
        public BaseResource Full { get; private set; }

        public SegmentResource(string id, string type) : base(id, type)
        {
        }

        public SegmentResource SetFull(BaseResource resource) => SetPropertyValue(a => a.Full, resource);
    }
}