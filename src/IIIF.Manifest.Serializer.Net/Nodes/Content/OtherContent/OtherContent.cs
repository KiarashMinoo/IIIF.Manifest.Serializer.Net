using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Content.OtherContent
{
    [JsonConverter(typeof(OtherContentJsonConverter))]
    public class OtherContent : BaseContent<OtherContent>
    {
        public OtherContent(string id) : base(id, "sc:AnnotationList")
        {
        }
    }
}