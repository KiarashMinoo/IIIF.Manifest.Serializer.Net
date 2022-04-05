using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
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