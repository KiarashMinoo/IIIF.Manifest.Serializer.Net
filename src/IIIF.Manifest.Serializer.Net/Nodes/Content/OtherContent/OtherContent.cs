using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
{
    [JsonConverter(typeof(OtherContentJsonConverter))]
    public class OtherContent : BaseContent<OtherContent>
    {
        public OtherContent(string id) : base(id, "sc:AnnotationList")
        {
        }
    }
}