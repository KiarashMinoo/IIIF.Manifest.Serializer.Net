using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Nodes
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