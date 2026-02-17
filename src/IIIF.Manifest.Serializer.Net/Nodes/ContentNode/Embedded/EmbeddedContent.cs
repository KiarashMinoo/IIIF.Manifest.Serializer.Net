using IIIF.Manifests.Serializer.Nodes.Content.Embedded.Resource;
using IIIF.Manifests.Serializer.Shared.Content;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes.Content.Embedded
{
    public class EmbeddedContent : BaseContent<EmbeddedContent, EmbeddedContentResource>
    {
        public const string OnJName = "on";

        [JsonProperty(OnJName)]
        public string On { get; }

        public EmbeddedContent(string id, EmbeddedContentResource resource, string on) : base(id, "oa:Annotation", resource)
        {
            On = on;
        }
    }
}