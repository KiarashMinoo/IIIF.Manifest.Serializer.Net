using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Contents.Segment.Resource
{
    public class SegmentResource(string id, string type) : BaseResource<SegmentResource>(id, type)
    {
        public const string FullJName = "full";

        [JsonProperty(FullJName)]
        public BaseResource? Full
        {
            get => GetElementValue(x => x.Full);
            private set => SetElementValue(value);
        }

        public SegmentResource SetFull(BaseResource resource)
        {
            Full = resource;
            return this;
        }
    }
}